using System.Text.Json.Serialization;

namespace HungerGamesSimulator.MessageCenter
{
    public class DesignerString
    {
        [JsonRequired]
        public required ContextType[] Contexts { get; set; }

        [JsonRequired]
        public required Input[] Inputs { get; set; }

        public TextType TextType { get; set; }

        [JsonRequired]
        public required string[] Texts { get; set; }
    }

    public class Input
    {
        [JsonRequired]
        public required string Type { get; set; }
        public string[]? Requirements { get; set; }
    }

}
