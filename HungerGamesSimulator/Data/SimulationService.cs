using System;
using System.Diagnostics;
using System.Linq;

namespace HungerGamesSimulator.Data
{
  public class SimulationService
  {
    private Simulation _simulation;
    private IMessageCenter _messageCenter;
    private Combat _combat;
    private PartyService _partyService;

    public SimulationService( Simulation simulation, IMessageCenter messageCenter )
    {
      _simulation = simulation;
      _messageCenter = messageCenter;
      _combat = new Combat();
      _partyService = new PartyService();
    }

    public void Act()
    {
      _messageCenter.ClearMessages();
      _messageCenter.ClearCannonMessages();

      _messageCenter.AddMessage( $"Day {_simulation.Day}" );

      foreach ( var actor in _simulation.GetActors() )
      {
        CombatRequest( actor );
      }

      _simulation.IncreaseDay();
    }

    /// <summary>
    /// Initializes the data to request combat 
    /// </summary>
    /// <param name="actor"></param>
    public void CombatRequest( IActor actor )
    {
      var otherActor = GetTributesSurrondingRequest( actor.Location, actor );
      if ( otherActor == null )
      {
        // combat cannot be processed without a tribute
        _messageCenter.AddMessage( $"{actor.Name} searched for a tribute to attack, but couldn't find any" );
        return;
      }
      
      // TO:DO add retrieval of other party members
      var fighters = new List<IActor>() { actor };
      var defenders = new List<IActor>() { otherActor };

      // create the request and simulate combat
      var request = new CombatRequest( fighters, defenders );
      var response = _combat.Simulate( request );

      GenerateCombatDescriptions( response, fighters, defenders );
    }

    /// <summary>
    /// Creates the dynamic messages that can result from the summary of the combat experience
    /// </summary>
    /// <param name="combatResponse"></param>
    /// <param name="fighters"></param>
    /// <param name="defenders"></param>
    public void GenerateCombatDescriptions( CombatResponse combatResponse, List<IActor> fighters, List<IActor> defenders )
    {
      // manage the response 
      var concatedFighterNames = String.Join( ", ", fighters.Select( fighter => fighter.Name ) );
      var concatedDefenderNames = String.Join( ", ", defenders.Select( defender => defender.Name ) );

      if ( combatResponse.defendersDied )
      {
        var deadDefenders = defenders.Where( defender => defender.IsDead() );
        var deadDefenderNames = String.Join( ", ", deadDefenders.Select( defender => defender.Name ) );

        _messageCenter.AddMessage( $"{concatedFighterNames} attacked {concatedDefenderNames} and killed {deadDefenderNames}" );
        foreach ( var deadActor in deadDefenders )
        {
          _messageCenter.AddCannonMessage( deadActor );
        }
      }
      else if ( combatResponse.escaped )
      {
        _messageCenter.AddMessage( $"{concatedFighterNames} attacked {concatedDefenderNames}. {concatedDefenderNames} barely escaped" );
      }
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
