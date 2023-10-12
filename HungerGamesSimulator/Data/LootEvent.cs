namespace HungerGamesSimulator.Data
{ 
  public interface ILootEvent
  {
    public string? Title { get; }

    public List<IWeapon> Weapons { get; }
  }
}
