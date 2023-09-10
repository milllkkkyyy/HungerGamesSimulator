namespace HungerGamesSimulator.Data;

public enum ActorStates
{
    Attacking,
    Moving
}

public interface IActor 
{
    public string Name { get; }
    public Coord Location { get; }
    public int Speed { get; }
    public ActorStates GetState();
    public Coord SimulateMove();
    public void SimulateAttack( IActor actor );
    public void SetLocation(Coord location);
    public void SetSpeed(int speed);
}