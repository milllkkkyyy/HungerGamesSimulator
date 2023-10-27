namespace HungerGamesSimulator.Data
{

  public enum DamageType
  {
    Bludgeoning,
    Piercing,
    Slashing
  }

  public struct Weapon
  {
    public string? Name { get; }
    public int NumberOfDice { get; }
    public int TypeOfDice { get; }
    public DamageType DamageType { get; }
    public bool IsRanged { get; }

    public Weapon()
    {
      Name = "Hands";
      NumberOfDice = 1;
      TypeOfDice = 4;
      DamageType = DamageType.Bludgeoning;
      IsRanged = false;
    }

    public Weapon( string? name, int numberOfDice, int typeOfDice, DamageType damageType, bool isRanged )
    {
      Name = name;
      NumberOfDice = numberOfDice;
      TypeOfDice = typeOfDice;
      DamageType = damageType;
      IsRanged = isRanged;
    }

    public static int RollWeaponDamage( Weapon weapon )
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
