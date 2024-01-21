using System.ComponentModel.DataAnnotations;

namespace HungerGamesSimulator.Data;

public abstract class Actor : IActor
{
  private readonly int _stayInPartyDC = 4;
  private readonly int _lookForPartyDC = 16;
  private readonly int _lookForCombatDC = 14;

  // Form variables

  [Required]
  public string Name { get; set; }
  
  [Required]
  public int ArmourClass { get; set; } = 12;
  
  [Required]
  [Range( 1, int.MaxValue, ErrorMessage = "Strength modifier must be between -4 and 4" )]
  public int Speed { get; set; } = 1;
  
  [Range( -4, 4, ErrorMessage = "Strength modifier must be between -4 and 4" )]
  public int Strength { get; set; } = 2;

  [Range( -4, 4, ErrorMessage = "Dexerity modifier must be between -4 and 4" )]
  public int Dexerity { get; set; } = 1;

  [Range( -4, 4, ErrorMessage = "Charisma modifier must be between -4 and 4" )]
  public int Charisma { get; set; } = 0;

  [Range( -4, 4, ErrorMessage = "Wisdom modifier must be between -4 and 4" )]
  public int Wisdom { get; set; } = 0;

  public Guid ActorId { get; set; } = Guid.NewGuid();
  public Coord Location { get; set; }
  public int Health { get; set; } = 12;
  public Weapon Weapon { get; set; } = new Weapon();
  public Guid PartyId { get; set; } = Guid.Empty;

  public Actor( string name )
  {
    Name = name;
  }

  public void Reset()
  {
    Weapon = new Weapon();  // Assign to null or a default weapon
    Health = 12;
    PartyId = Guid.Empty;
  }

  public virtual ActorAction GetNextAction( SimulationSnapshot snapshot )
  {
    if ( Health <= 0 )
    {
      return ActorAction.Dead;
    }

    if ( IsInParty() )
    {
      if ( SimulationUtils.RollD20() + Charisma <= _stayInPartyDC )
      {
        return ActorAction.LeaveParty;
      }
    }

    if ( SimulationUtils.RollD20() + Charisma >= _lookForPartyDC )
    {
      return ActorAction.JoinParty;
    }

    if ( SimulationUtils.RollD20() + Strength >= _lookForCombatDC )
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

  public override string ToString()
  { 
    return Name;
  }
}