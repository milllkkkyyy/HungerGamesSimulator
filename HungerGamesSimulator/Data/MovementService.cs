namespace HungerGamesSimulator.Data
{
  public class MovementService
  {
    public MovementResponse Move( MovementRequest request )
    {
      if ( !request.actorsToMove.Any() )
      {
        throw new ArgumentException( "There must be actors in order to move" );
      }

      var lastLocation = request.actorsToMove.First().Location;
      Coord wishLocation = request.actorsToMove.First().SimulateMove();

      // adjust location based on world information
      if ( wishLocation.X > request .WorldWidth )
      {
        wishLocation.X = request.WorldWidth;
      }
      else if ( wishLocation.X < 0 )
      {
        wishLocation.X = 0;
      }

      if ( wishLocation.Y > request.WorldHeight )
      {
        wishLocation.Y = request.WorldHeight;
      }
      else if ( wishLocation.Y < 0 )
      {
        wishLocation.Y = 0;
      }

      // set all actors to the same location
      foreach ( var actor in request.ActorsToMove )
      {
        actor.SetLocation( wishLocation );
      }

      return new MovementResponse( lastLocation, wishLocation );
    }

  }

  public record MovementRequest( List<IActor> actorsToMove, int worldWidth, int worldHeight )
  {
    public List<IActor> ActorsToMove { get; } = actorsToMove;

    public int WorldWidth { get; } = worldWidth;

    public int WorldHeight { get; } = worldHeight;
  }

  public record MovementResponse( Coord pastLocation, Coord newLocation )
  {
    public Coord PastLocation { get; } = pastLocation;
    public Coord NewLocation { get; } = newLocation;
  }

}
