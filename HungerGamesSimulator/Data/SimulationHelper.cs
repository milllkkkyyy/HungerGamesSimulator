namespace HungerGamesSimulator.Data
{
  public static class SimulationHelper
  {
    public static int RollD20()
    {
      return Random.Shared.Next( 1, 21 );
    }

    public static int CalculateDamage( IActor actor )
    {
      int damage = Weapon.RollWeaponDamage( actor.Weapon );
      return actor.Weapon.IsRanged ? damage + actor.Strength : damage + actor.Dexerity;
    }
  }
}
