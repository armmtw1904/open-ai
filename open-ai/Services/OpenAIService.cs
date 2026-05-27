using Azure.AI.OpenAI;
using Microsoft.Extensions.AI;
using open_ai.Models;
using System.ClientModel;


namespace open_ai.Services
{
    public class OpenAIService : IAIService
    {
        const string deploymentName = "YOUR_DEPLOYMENT_NAME";
        const string endpoint = "YOUR_OPENAI_ENDPOINT";
        const string apiKey = "YOUR_API_KEY";
        const string systemPrompt = @"You are a highly accurate multilingual language detection engine.
 
            Your responsibilities:
            - Detect all languages present in the input text.
            - Determine whether the text contains a single language or multiple languages.
            - Split the text into contiguous segments where each segment belongs to exactly one language.
            - Assign one language per segment (no overlap).
            - Provide a confidence score (0 to 1) for each segment and each language.
            - Calculate percentage contribution of each language in the entire text.
            - Ensure percentage values across all languages add up to exactly 100.
 
            Rules:
            1. Output MUST be valid JSON only (no explanations, no markdown, no extra text).
            2. Each segment must preserve original text exactly (including punctuation and spacing).
            3. Segments should be meaningful (prefer sentence or phrase boundaries where possible).
            4. Confidence must reflect certainty:
               - High confidence (>0.9) for clear languages
               - Medium (0.6–0.9) for mixed or short text
               - Low (<0.6) for ambiguous content
            5. If only one language is present:
               - Set ""isSingleLanguage"" = true
               - Return only one language entry
               - Return one segment covering the full text
            6. Always include:
               - language name
               - ISO 639-1 code
            7. Provide a short explanation describing why languages were detected.
 

            OUTPUT FORMAT (STRICT)
            {
              ""isSingleLanguage"": boolean,
              ""segments"": [
                {
                  ""segment"": string,
                  ""language"": string,
                  ""iso639_1"": string,
                  ""confidence"": number
                }
              ],
              ""languages"": [
                {
                  ""language"": string,
                  ""iso639_1"": string,
                  ""confidence"": number,
                  ""percentage"": integer
                }
              ],
              ""explanation"": string
            }

            Strictly follow the output schema provided.";

        public async Task<ClassifyLanguagesResult> ClassifyLanguagesAsync(string message_input= "AI Response")
        {
            Console.WriteLine("openai service");

            IChatClient chatClient = new AzureOpenAIClient(new Uri(endpoint), new ApiKeyCredential(apiKey))
                .GetChatClient(deploymentName)
                .AsIChatClient();

            // Call Azure OpenAI API with timeout
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));

            // Build messages
            var messages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.System,systemPrompt),
                new ChatMessage(ChatRole.User,message_input)
            };

            var response = "";
            await foreach (ChatResponseUpdate item in chatClient.GetStreamingResponseAsync(messages))
            {
                response += item.Text;
            }

            var resp = System.Text.Json.JsonSerializer.Deserialize<ClassifyLanguagesResult>(response);

            return resp;
        }
    }
}
