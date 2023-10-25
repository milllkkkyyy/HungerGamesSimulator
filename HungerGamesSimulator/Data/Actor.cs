namespace HungerGamesSimulator.Data;

public abstract class Actor : IActor
{
  private static int stayInPartyDC = 10;

  private static int lookForPartyDC = 12;

  private static int lookForCombatDC = 10;

  public string Name { get; }
  public Coord Location { get; private set; }
  public int Speed { get; private set; } = 1;
  public int ArmourClass { get; private set; } = 10;
  public int Strength { get; private set; } = 0;
  public IWeapon Weapon { get; private set; }
  public int Dexerity { get; private set; } = 0;
  public int Health { get; private set; } = 12;
  public Guid ActorId { get; private set; } = Guid.NewGuid();
  public Guid PartyId { get; set; } = Guid.Empty;

  protected Actor( string name )
  {
    Name = name;
  }

  public virtual ActorStates GetNewState()
  {
    if ( Health <= 0 )
    {
      return ActorStates.Dead;
    }

    if ( IsInParty() )
    {
      if ( SimulationUtils.RollD20() < stayInPartyDC )
      {
        return ActorStates.LeaveParty;
      }
    }

    if ( SimulationUtils.RollD20() >= lookForPartyDC )
    {
      return ActorStates.JoinParty;
    }

    if ( SimulationUtils.RollD20() + Strength >= lookForCombatDC )
    {
      return ActorStates.Attacking;
    }

    return ActorStates.Moving;
  }

  public virtual Coord SimulateMove()
  {
    var direction = new Coord( Random.Shared.Next( -Speed, Speed + 1 ), Random.Shared.Next( -Speed, Speed + 1 ) );
    return direction + Location;
  }

  public virtual bool SimulateHit( IActor actor )
  {
    return ( SimulationUtils.RollD20() + Strength ) >= actor.ArmourClass;
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
    return ( SimulationUtils.RollD20() + Dexerity ) >= ( SimulationUtils.RollD20() + actor.Dexerity );
  }


  public void TakeDamage( int damage )
  {
    Health -= damage;
  }

  public bool IsDead()
  {
    return this.Health <= 0;
  }

  public bool IsInParty()
  {
    return PartyId != Guid.Empty;
  }
}