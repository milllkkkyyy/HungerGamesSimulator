namespace HungerGamesSimulator.Data;

public class Simulation
{
  public int Width { get; set; } = 5;
  public int Height { get; set; } = 5;
  public int Day { get; internal set; } = 1;

  private List<IActor> _actors;
  private List<Weapon> _weapons;

  public Simulation( List<IActor> actors, List<Weapon> weapons )
  {
    _actors = actors;
    _weapons = weapons;
    foreach ( var actor in actors )
    {
      // TO:DO Remove to instead handle a dictionary of string to weapon
      actor.Location = new Coord( Width / 2, Height / 2 );
    }
  }

  public void IncreaseDay()
  {
    Day++;
  }

  public IActor? GetRandomActorInArea( Coord center, IActor? toIgnore = null, Predicate<IActor>? predicate = null )
  {
    var inArea =
        _actors
            .Where( actor => toIgnore != actor )
            .Where( actor => center.X - 1 <= actor.Location.X && actor.Location.X <= center.X + 1 )
            .Where( actor => center.Y - 1 <= actor.Location.Y && actor.Location.Y <= center.Y + 1 )
            .Where( actor => predicate == null ? true : predicate( actor ) )
            .Where( actor => actor.Health > 0 )
            .ToList();

    return ( inArea.Count == 0 ) ? null : inArea[ Random.Shared.Next( inArea.Count ) ];
  }

  public List<IActor>? GetActors( Predicate<IActor>? predicate = null )
  {
    return _actors
            .Where( actor => predicate == null ? true : predicate( actor ) )
            .ToList();
  }

  public List<IActor>? GetAliveActors( Predicate<IActor>? predicate = null, IActor? toIgnore = null )
  {
    var actors =
        _actors
            .Where( actor => toIgnore != actor )
            .Where( actor => actor.Health > 0 )
            .Where( actor => predicate == null ? true : predicate( actor ) )
            .ToList();

    return ( actors.Count == 0 ) ? null : actors;
  }

  public SimulationSnapshot GetSimulationSnapshot()
  {
    return new SimulationSnapshot
    {
      WorldWidth = Width,
      WorldHeight = Height,
      ActorsAround = false
    };
  }

}

public record SimulationSnapshot
{
  public int WorldWidth { get; set; }
  public int WorldHeight { get; set; }
  public bool ActorsAround { get; set; }
  public bool SuddenDeath { get; set; }
}