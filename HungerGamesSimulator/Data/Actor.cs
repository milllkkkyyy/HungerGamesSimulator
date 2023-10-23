namespace HungerGamesSimulator.Data;

public abstract class Actor : IActor
{
  public string Name { get; }
  public Coord Location { get; private set; }
  public int Speed { get; private set; } = 1;
  public int ArmourClass { get; private set; } = 10;
  public int Strength { get; private set; } = 0;
  public IWeapon Weapon { get; private set; }
  public int Dexerity { get; private set; } = 0;
  public int Health { get; private set; } = 12;
  public Guid ActorId { get; private set; } = Guid.NewGuid();

  protected Actor( string name )
  {
    Name = name;
  }

  public void Reset()
  {
    Location = default;
    Speed = 1;
    ArmourClass = 10;
    Strength = 0;
    Weapon = default;  // Assign to null or a default weapon
    Dexerity = 0;
    Health = 12;
    ActorId = Guid.NewGuid();
  }

  public virtual ActorStates GetState()
  {
    if ( Health <= 0 )
    {
      return ActorStates.Dead;
    }

    var coin = Random.Shared.Next( 0, 2 );
    return coin == 0 ? ActorStates.Attacking : ActorStates.Moving;
  }

  public virtual Coord SimulateMove()
  {
    var direction = new Coord( Random.Shared.Next( -Speed, Speed + 1 ), Random.Shared.Next( -Speed, Speed + 1 ) );
    return direction + Location;
  }

  public virtual bool SimulateHit( IActor actor )
  {
    return ( SimulationUtil.RollD20() + Strength ) >= actor.ArmourClass;
  }

  public void SetLocation( Coord location )
  {
    Location = location;
  }

  public void GiveWeapon( IWeapon weapon )
  {
    Weapon = weapon;
  }

  public bool SimulateEscape( IActor actor )
  {
    return ( SimulationUtil.RollD20() + Dexerity ) >= ( SimulationUtil.RollD20() + actor.Dexerity );
  }


  public void TakeDamage( int damage )
  {
    Health -= damage;
  }

  public bool IsDead()
  {
    return this.Health <= 0;
  }
}