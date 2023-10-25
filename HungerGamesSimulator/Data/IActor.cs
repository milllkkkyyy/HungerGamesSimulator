namespace HungerGamesSimulator.Data;

public enum ActorStates
{
  Attacking,
  Moving,
  Dead,
  JoinParty,
  LeaveParty
}

public interface IActor
{
  public string Name { get; }
  public Guid ActorId { get; }
  public Guid PartyId { get; set; }
  public Coord Location { get; }
  public int Speed { get; }
  public int ArmourClass { get; }
  public int Strength { get; }
  public int Dexerity { get; }
  public int Health { get; }
  public IWeapon Weapon { get; }
  public ActorStates GetNewState();

  public Coord SimulateMove();
  /// <summary>
  /// Simulate hiting another actor
  /// </summary>
  /// <param name="actor"></param>
  /// <returns>if this actor hit the other actor</returns>
  public bool SimulateHit( IActor actor );
  public void SetLocation( Coord location );
  public void GiveWeapon( IWeapon weapon );
  public bool SimulateEscape( IActor actor );
  public void TakeDamage( int damage );
  public bool IsDead();
  public bool IsInParty();
}