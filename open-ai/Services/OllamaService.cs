using Microsoft.Extensions.AI;
using OllamaSharp;
using open_ai.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace open_ai.Services
{
    public class OllamaService : IAIService
    {
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
            1. Donot transform the input text in any way (no translation, no transliteration, no normalization). Just analyze it as-is and detect the languages present.
            2. Output MUST be valid JSON only (no explanations, no markdown, no extra text).
            3. Each segment must preserve original text exactly (including punctuation and spacing).
            4. Segments should be meaningful (prefer sentence or phrase boundaries where possible).
            5. Confidence must reflect certainty:
               - High confidence (>0.9) for clear languages
               - Medium (0.6–0.9) for mixed or short text
               - Low (<0.6) for ambiguous content
            6. If only one language is present:
               - Set ""isSingleLanguage"" = true
               - Return only one language entry
               - Return one segment covering the full text
            7. Always include:
               - language name
               - ISO 639-1 code
            8. Provide a short explanation describing why languages were detected.
 

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
        public async Task<ClassifyLanguagesResult> ClassifyLanguagesAsync(string message_input)
        {
            Console.WriteLine("ollama service");

            IChatClient chatClient = new OllamaApiClient(new Uri("http://localhost:11434/"), "phi3:mini");

            List<ChatMessage> chatHistory = new();
            chatHistory.Add(new ChatMessage(ChatRole.System, systemPrompt));
            chatHistory.Add(new ChatMessage(ChatRole.User, message_input));

            var response = "";
            await foreach (ChatResponseUpdate item in
                chatClient.GetStreamingResponseAsync(chatHistory))
            {
                response += item.Text;
            }


            var resp = System.Text.Json.JsonSerializer.Deserialize<ClassifyLanguagesResult>(response);

            return resp;
        }
    }
}
