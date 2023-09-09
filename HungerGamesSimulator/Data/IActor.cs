using System.Numerics;

namespace HungerGamesSimulator.Data;


public interface IActor 
{ 
    public string? Name { get; set; }
    public Coord Velocity { get; set; }
    public Coord Location { get; set; }
    public void Reset();
    public void Act( Game world );
    
    public bool IsDead();
}