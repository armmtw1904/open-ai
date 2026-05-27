using System.Text.Json.Serialization;

namespace open_ai.Models
{
public class ClassifyLanguagesResult
    {
        [JsonPropertyName("isSingleLanguage")]
        public bool IsSingleLanguage { get; set; }

        [JsonPropertyName("segments")]
        public List<LanguageSegment> Segments { get; set; } = new();

        [JsonPropertyName("languages")]
        public List<DetectedLanguage> Languages { get; set; } = new();

        [JsonPropertyName("explanation")]
        public string Explanation { get; set; } = string.Empty;
    }

    public class LanguageSegment
    {
        [JsonPropertyName("segment")]
        public string Segment { get; set; } = string.Empty;

        [JsonPropertyName("language")]
        public string Language { get; set; } = string.Empty;

        [JsonPropertyName("iso639_1")]
        public string Iso639_1 { get; set; } = string.Empty;

        [JsonPropertyName("confidence")]
        public double Confidence { get; set; }
    }

    public class DetectedLanguage
    {
        [JsonPropertyName("language")]
        public string Language { get; set; } = string.Empty;

        [JsonPropertyName("iso639_1")]
        public string Iso639_1 { get; set; } = string.Empty;

        [JsonPropertyName("confidence")]
        public double Confidence { get; set; }

        [JsonPropertyName("percentage")]
        public int Percentage { get; set; }
    }
}
