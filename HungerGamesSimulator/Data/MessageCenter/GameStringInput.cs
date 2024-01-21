namespace HungerGamesSimulator.MessageCenter
{
    public class GameStringInputArgs
    {
        public required Type Type { get; set; }
        public Requirement[]? Requirements { get; set; }
    }

    public class GameStringInputMatches : Dictionary<int, int>
    {
        public new void Add(int desginerStringIndex, int givenIndex)
        {
            base.Add(desginerStringIndex, givenIndex);
        }

        public new int this[int desginerStringIndex]
        {
            get { return base[desginerStringIndex]; }
            set { base[desginerStringIndex] = value; }
        }
    }

    public class GameStringInput
    {
        readonly GameStringInputArgs[] _validity;

        public GameStringInput(GameStringInputArgs[] args)
        {
            _validity = args;
        }

        public bool IsInputValid(object[] inputs)
        {
            var matches = GetMatches(inputs);
            return matches.Count == _validity.Length;
        }

        /// <summary>
        /// Checks if the input is valid for the gamestring
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public bool TryGetMatches( object[] inputs, out GameStringInputMatches matches )
        {
            matches = GetMatches(inputs);
            if (matches.Count != _validity.Length)
            {
                matches = new GameStringInputMatches();
                return false;
            }

            return true;
        }

        public GameStringInputMatches GetMatches(object[] inputs)
        {
            GameStringInputMatches matches = new();

            if (inputs.Length != _validity.Length)
                return matches;

            for (int i = 0; i < _validity.Length; i++)
            {
                for (int j = 0; j < inputs.Length; j++)
                {

                    if (matches.ContainsKey(i) || matches.ContainsValue(j))
                        continue;

                    if (_validity[i].Type != inputs[j].GetType())
                        continue;

                    if (!DoesPassRequirements(_validity[i].Requirements, inputs[j]))
                        continue;

                    matches.Add( desginerStringIndex: i, givenIndex: j);
                    break;
                }
            }

            return matches;

            bool DoesPassRequirements(Requirement[]? requirements, object input)
            {
                if (requirements is not null)
                {
                    foreach (var requirement in requirements)
                    {
                        if (!requirement.CheckRequirement(input))
                        {
                            return false;
                        }
                    }
                }

                return true;
            }
        }
    }
}
