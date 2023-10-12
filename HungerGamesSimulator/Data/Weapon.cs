namespace HungerGamesSimulator.Data
{
  public class Weapon : IWeapon
  {
    public string? Name { get; }
    public int NumberOfDice { get; }
    public int TypeOfDice { get; }
    public DamageType DamageType { get; }
    public bool IsRanged { get; }

    public Weapon( string? name, int numberOfDice, int typeOfDice, DamageType damageType, bool isRanged )
    {
      Name = name;
      NumberOfDice = numberOfDice;
      TypeOfDice = typeOfDice;
      DamageType = damageType;
      IsRanged = isRanged;
    }

    public static int RollWeaponDamage( IWeapon weapon )
    {
      int sum = 0;
      for ( int i = 0; i < weapon.NumberOfDice; i++ )
      {
        sum += Random.Shared.Next( 1, weapon.TypeOfDice + 1 );
      }
      return sum;
    }
  }
}
