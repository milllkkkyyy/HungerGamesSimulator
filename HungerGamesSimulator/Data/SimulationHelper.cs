namespace HungerGamesSimulator.Data
{
  public static class SimulationHelper
  {
    public static int RollD20()
    {
      return Random.Shared.Next( 1, 21 );
    }

    public static int CalculateDamage( IWeapon weapon )
    {
      int sum = 0;
      for ( int i = 0; i < weapon.NumberOfDice; i++ )
      {
        sum += Random.Shared.Next( 1 , weapon.TypeOfDice + 1 ); 
      }
      return sum;
    }
  }
}
