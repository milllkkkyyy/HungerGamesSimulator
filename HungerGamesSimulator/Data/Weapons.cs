namespace HungerGamesSimulator.Data
{
  public class Hand : IWeapon
  {
    public string Name { get; } = $"Hand";

    public int NumberOfDice { get; } = 1;

    public int TypeOfDice { get; } = 4;

    public DamageType DamageType { get; } = DamageType.Bludgeoning;
  }
}
