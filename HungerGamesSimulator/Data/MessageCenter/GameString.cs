using System.Text.RegularExpressions;

namespace HungerGamesSimulator.MessageCenter
{
    public partial class GameString
    {
        private readonly ContextType[] _contexts;
        private readonly GameStringInput _gameStringInput;
        private readonly string[] _texts;
        public TextType TextType { get; }

        public GameString(GameStringInput gameStringInput, string[] texts, TextType textType, ContextType[] contexts)
        {
            _gameStringInput = gameStringInput;
            _texts = texts;
            TextType = textType;
            _contexts = contexts;
        }

        /// <summary>
        /// Check if contexts are valid for this GameString
        /// </summary>
        /// <param name="contexts"></param>
        /// <returns></returns>
        public bool IsContextValid(ContextType[] contexts)
        {
            return !contexts.Except(_contexts).Any();
        }

        public bool IsInputValid(BuilderObject[] inputs)
        {
            return _gameStringInput.IsInputValid(inputs);
        }

        /// <summary>
        /// Attempt to convert the GameString to a string. returns true if the conversion is possible, otherwise false
        /// </summary>
        /// <param name="input"></param>
        /// <param name="gameString">null if the output is false, otherwise the converted string</param>
        /// <returns></returns>
        public bool TryToString(BuilderObject[] input, out string gameString )
        {
            if (_gameStringInput.TryGetMatches(input, out var matches ))
            {
                gameString = ToString( matches, input, _texts[Random.Shared.Next(_texts.Length)]);
                return true;
            }

            gameString = string.Empty;
            return false;
        }

        /// <summary>
        /// Converts the designer inputs into dynamic strings during runtime
        /// </summary>
        /// <param name="inputs">the dynamic inputs provided by services</param>
        /// <param name="text">the designer text that needs to be manipulated</param>
        /// <returns>a string which is formatted as the designers wanted</returns>
        private string ToString( GameStringInputMatches inputMatches , BuilderObject[] inputs, string text )
        {
            Regex indexFinder = IndexFinder();
            Regex blockFinder = BlockFinder();

            MatchCollection blocks = blockFinder.Matches(text);
            foreach ( Match block in blocks.Cast<Match>())
            {
                // find the index for which input the designer is requesting
                var stringIdx = indexFinder.Match(block.Value);
                if (stringIdx != null && int.TryParse( stringIdx.Value, out int index )) 
                {
                    int actualIndex = inputMatches[index];
                    if (TryGetPropertyValue(match: block.Value, index: actualIndex, out var propertyString))
                    {
                        text = text.Replace(block.Value, propertyString);
                    }
                    else
                    {
                        text = text.Replace(block.Value, inputs[actualIndex].Input.ToString());
                    }
                }
            }

            return text;

            // check to see if the designer used any properties using reflection
            bool TryGetPropertyValue( string match, int index, out string toString )
            {
                var propertyInfo = inputs[index].Input.GetType().GetProperties();
                foreach (var property in propertyInfo)
                {
                    if (!match.Contains(property.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    toString = property.GetValue(inputs[index].Input, null)?.ToString() ?? "";
                    return true;
                }

                toString = "";
                return false;
            }
        }

        /// <summary>
        /// Finds the index variables in a string
        /// </summary>
        /// <returns></returns>
        [GeneratedRegex("(\\d)")]
        private static partial Regex IndexFinder();

        /// <summary>
        /// Finds an input block in a designer string
        /// </summary>
        /// <returns></returns>
        [GeneratedRegex("\\{(.*?)\\}")]
        private static partial Regex BlockFinder();
    }
}
