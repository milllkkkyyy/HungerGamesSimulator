namespace HungerGamesSimulator.Data
{
  public class PartyFinder
  {
    public Guid PartyToFind { get; set; } = Guid.Empty;

    public Predicate<IActor> FindParty
    {
      get => IsSameParty;
    }

    private bool IsSameParty( IActor actor )
    {
      return actor.IsInParty() && actor.PartyId == PartyToFind;
    }

  }
}
