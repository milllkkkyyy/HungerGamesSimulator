namespace HungerGamesSimulator.Data;

public abstract class Actor : IActor
{
    public string Name { get; }
    public Coord Location { get; private set; }
    public int Speed { get; private set; } = 1;

    protected Actor(string name)
    {
        Name = name;
    }

    public virtual ActorStates GetState()
    {
        var coin = Random.Shared.Next(0, 2);
        return coin == 0 ? ActorStates.Attacking : ActorStates.Moving;
    }
    
    public virtual Coord SimulateMove()
    {
        var direction = new Coord( Random.Shared.Next(-Speed, Speed + 1) , Random.Shared.Next(-Speed, Speed + 1) );
        return direction + Location;
    }

    public virtual void SimulateAttack(IActor actor)
    {
        return;
    }

    public void SetLocation(Coord location)
    {
        Location = location;
    }

    public void SetSpeed(int speed)
    {
        Speed = speed;
    }
}