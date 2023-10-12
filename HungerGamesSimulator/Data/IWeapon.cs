namespace HungerGamesSimulator.Data
{

  public enum DamageType
  {
    Bludgeoning,
    Piercing,
    Slashing
  }

  public interface IWeapon
  {
    public string? Name { get; }
    public int NumberOfDice { get; }
    public int TypeOfDice { get; }
    public  DamageType DamageType { get; }
    public bool IsRanged { get; }
  }
}
