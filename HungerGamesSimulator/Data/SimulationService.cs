using System.Diagnostics;

namespace HungerGamesSimulator.Data
{
  public class SimulationService
  {
    private Simulation _simulation;
    private IMessageCenter _messageCenter;

    public SimulationService( Simulation simulation, IMessageCenter messageCenter )
    {
      _simulation = simulation;
      _messageCenter = messageCenter;
    }

    public void Act()
    {
      _messageCenter.ClearMessages();
      _messageCenter.ClearCannonMessages();

      _messageCenter.AddMessage( $"Day {_simulation.Day}" );

      foreach ( var actor in _simulation.GetActors() )
      {
        actor.Act( this );
      }

      _simulation.IncreaseDay();
    }

    public void CombatRequest( IActor actor, IActor? otherActor )
    {
      if ( otherActor == null )
      {
        _messageCenter.AddMessage( $"{actor.Name} attempted to attack, but no other tribute was near" );
        return;
      }

      bool escaped = false;
      do
      {
        if ( otherActor.Health <= 0 )
        {
          break;
        }

        if ( actor.SimulateHit( otherActor ) )
        {
          otherActor.TakeDamage( SimulationHelper.CalculateDamage( actor ) );
        }

        escaped = otherActor.SimulateEscape( actor );
      }
      while ( escaped != true );

      if ( otherActor.Health <= 0 )
      {
        _messageCenter.AddMessage( $"{actor.Name} attacked {otherActor.Name} and killed them" );
        _messageCenter.AddCannonMessage( otherActor );
      }
      else if ( escaped )
      {
        _messageCenter.AddMessage( $"{actor.Name} attacked {otherActor.Name}, but  {otherActor.Name} successfully escaped" );
      }
      Debug.WriteLine( $"{actor.Name} was at positon {actor.Location}" );
    }

    public void LocationChangeRequest( IActor actor, Coord wishLocation )
    {
      if ( wishLocation.X > _simulation.Height )
      {
        wishLocation.X = _simulation.Width;
      }
      else if ( wishLocation.X < 0 )
      {
        wishLocation.X = 0;
      }

      if ( wishLocation.Y > _simulation.Height )
      {
        wishLocation.Y = _simulation.Height;
      }
      else if ( wishLocation.Y < 0 )
      {
        wishLocation.Y = 0;
      }

      _messageCenter.AddMessage( $"{actor.Name} moved from {actor.Location} to {wishLocation}" );
      actor.SetLocation( wishLocation );
    }

    public IActor? GetTributesSurrondingRequest( Coord origin, IActor toIgnore )
    {
      return _simulation.GetRandomActorInArea( origin, toIgnore );
    }
  }
}
