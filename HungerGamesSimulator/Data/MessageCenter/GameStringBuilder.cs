using System.Text;

namespace HungerGamesSimulator.MessageCenter
{
    public class GameStringBuilder : Queue<BuilderContainer>
    {
        private IReadOnlyList<GameString> _gameStrings;

        public GameStringBuilder(IReadOnlyList<GameString> gameStrings) : base()
        {
            _gameStrings = gameStrings;
        }
         
        public void QueueInformation( ContextType[] contexts, params BuilderObject[] inputs)
        {
            Enqueue(new BuilderContainer(contexts, inputs));
        }

        /// <summary>
        /// Removes items queued in string builder and converts infromation to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder stringBuilder = new();
            while (Count != 0)
            {
                var builder = Dequeue();

                // find all valid game strings 
                List<GameString> validGameStrings = new();
                foreach (var game in _gameStrings)
                {
                    var inputs = game.IsInputValid(builder.Inputs);
                    var contexts = game.IsContextValid(builder.Contexts);
                    if (inputs && contexts)
                    {
                        validGameStrings.Add(game);
                    }
                }

                if (validGameStrings.Count == 0)
                {
                    throw new NotImplementedException( $"No valid game string for {String.Join(",", builder.Contexts)}" );
                }

                // pick a random game string
                var gameString = validGameStrings[Random.Shared.Next(validGameStrings.Count)];

                // see if the string is able to be parsed
                if (gameString.TryToString(builder.Inputs, out var finalString))
                {
                    stringBuilder.Append(finalString + "\n");
                }
            }

            return stringBuilder.ToString();
        }
    }
    public class BuilderContainer
    {
        public ContextType[] Contexts { get; }
        public BuilderObject[] Inputs { get; }

        public BuilderContainer(ContextType[] contexts, BuilderObject[] inputs)
        {
            Contexts = contexts;
            Inputs = inputs;
        }
    }
}
