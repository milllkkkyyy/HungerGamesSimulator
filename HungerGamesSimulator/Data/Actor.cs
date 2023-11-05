namespace HungerGamesSimulator.Data;

public abstract class Actor : IActor
{
  private readonly int stayInPartyDC = 4;
  private readonly int lookForPartyDC = 16;
  private readonly int lookForCombatDC = 14;

  public string Name { get; }
  public Guid ActorId { get; } = Guid.Empty;
  public int ArmourClass { get; } = 12;
  public int Speed { get; } = 1;
  public int Strength { get; } = 2;
  public int Dexerity { get; } = 1;
  public int Charisma { get; } = 0;
  public int Wisdom { get; } = 0;
  public Coord Location { get; set; }
  public int Health { get; set; } = 12;
  public Weapon Weapon { get; set; } = new Weapon();
  public Guid PartyId { get; set; } = Guid.Empty;

  public Actor( string name )
  {
    Name = name;
  }

  public virtual ActorAction GetNextAction( SimulationSnapshot snapshot )
  {
    if ( Health <= 0 )
    {
      return ActorAction.Dead;
    }

    if ( IsInParty() )
    {
      if ( SimulationUtils.RollD20() + Charisma <= stayInPartyDC )
      {
        return ActorAction.LeaveParty;
      }
    }

    if ( SimulationUtils.RollD20() + Charisma >= lookForPartyDC )
    {
      return ActorAction.JoinParty;
    }

    if ( SimulationUtils.RollD20() + Strength >= lookForCombatDC )
    {
      return ActorAction.Attacking;
    }

    return ActorAction.Moving;
  }

  public virtual Coord SimulateMove( SimulationSnapshot _ )
  {
    var direction = new Coord( Random.Shared.Next( -Speed, Speed + 1 ), Random.Shared.Next( -Speed, Speed + 1 ) );
    return direction + Location;
  }

  public virtual bool SimulateHit( IActor otherActor )
  {
    return ( SimulationUtils.RollD20() + Strength ) >= otherActor.ArmourClass;
  }

  public bool SimulateEscape( IActor otherActor )
  {
    return ( SimulationUtils.RollD20() + Dexerity ) >= ( SimulationUtils.RollD20() + otherActor.Dexerity );
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