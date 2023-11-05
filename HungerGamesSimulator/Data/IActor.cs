namespace HungerGamesSimulator.Data;

public enum ActorAction
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
    public int ArmourClass { get; }
    public int Speed { get; }
    public int Strength { get; }
    public int Dexerity { get; }
    public int Charisma { get; }
    public int Wisdom { get; }
    public Coord Location { get; set; }
    public int Health { get; set; }
    public Weapon Weapon { get; set; }
    public Guid PartyId { get; set; }

    public ActorAction GetNextAction( SimulationSnapshot snapshot );

    public Coord SimulateMove( SimulationSnapshot snapshot );

    public bool SimulateHit( IActor otherActor );

    public bool SimulateEscape( IActor otherActor );

    public void TakeDamage( int damage );

    public bool IsDead();

    public bool IsInParty();
}