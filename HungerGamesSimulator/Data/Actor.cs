namespace HungerGamesSimulator.Data;

public abstract class Actor : IActor
{
  public string Name { get; }
  public Coord Location { get; private set; }
  public int Speed { get; private set; } = 1;
  public int ArmourClass { get; private set; } = 10;
  public int Strength { get; private set; } = 0;
  public IWeapon Weapon { get; private set; } = new Hand();
  public int Dexerity { get; private set; } = 0;
  public int Health { get; private set; } = 12;
  protected Actor( string name )
  {
    Name = name;
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
    return ( DiceHelper.RollD20() + Strength ) >= actor.ArmourClass;
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
    return ( DiceHelper.RollD20() + Dexerity ) >= ( DiceHelper.RollD20() + actor.Dexerity );
  }


  public void TakeDamage( int damage )
  {
    Health -= damage;
  }

}