namespace HungerGamesSimulator.Data.Metadata
{
    public class TributeMetaData : IMetaData
    {
        public string? Name { get; set; }

        public TributeMetaData(string name)
        {
            Name = name;
        }

    }
}
