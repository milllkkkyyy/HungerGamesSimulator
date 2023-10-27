namespace HungerGamesSimulator.Data
{
  public class SimulationService
  {
    private Simulation _simulation;
    private IMessageCenter _messageCenter;
    private CombatService _combatService;
    private PartyService _partyService;
    private MovementService _movementService;
    private PartyFinder _partyFinder;

    public SimulationService( Simulation simulation, IMessageCenter messageCenter )
    {
      _simulation = simulation;
      _messageCenter = messageCenter;
      _combatService = new CombatService();
      _partyService = new PartyService();
      _movementService = new MovementService();
      _partyFinder = new PartyFinder();
    }

    public void Act()
    {
      _messageCenter.ClearMessages();
      _messageCenter.ClearCannonMessages();

      _messageCenter.AddMessage( $"Day {_simulation.Day}" );

      var aliveActors = _simulation.GetAliveActors();
      if ( aliveActors != null )
      {
        foreach ( var actor in aliveActors )
        {
          Act( actor );
        }
      }

      _simulation.IncreaseDay();
    }

    private void Act( IActor actor )
    {
      var state = actor.GetNextAction( _simulation.GetSimulationSnapshot() );
      switch ( state )
      {
        case ActorAction.Attacking:
          CombatRequest( actor );
          break;
        case ActorAction.Moving:
          MovementRequest( actor );
          break;
        default:
          _messageCenter.AddMessage( $"There is no way for the simulation to handle state ({state}) for {actor.Name}" );
          break;
      }
    }

    /// <summary>
    /// Initializes the data to request combat 
    /// </summary>
    /// <param name="actor"></param>
    private void CombatRequest( IActor actor )
    {
      var otherActor = GetTributesSurrondingRequest( actor.Location, actor );
      if ( otherActor == null )
      {
        // combat cannot be processed without a tribute
        _messageCenter.AddMessage( $"{actor.Name} searched for a tribute to attack, but couldn't find any" );
        return;
      }

      // retrieve other party members
      var fighters = GetParty( actor );
      var defenders = GetParty( otherActor );

      // create the request and simulate combat
      var request = new CombatRequest( fighters, defenders );
      var response = _combatService.Simulate( request );

      GenerateCombatDescriptions( response, fighters, defenders );
    }

    /// <summary>
    /// Creates the dynamic messages that can result from the summary of the combat experience
    /// </summary>
    /// <param name="combatResponse"></param>
    /// <param name="fighters"></param>
    /// <param name="defenders"></param>
    private void GenerateCombatDescriptions( CombatResponse combatResponse, List<IActor> fighters, List<IActor> defenders )
    {
      // manage the response 
      var concatedFighterNames = SimulationUtils.GetConcatenatedActorNames( fighters );
      var concatedDefenderNames = SimulationUtils.GetConcatenatedActorNames( defenders );

      if ( combatResponse.DefendersDied )
      {
        var deadDefenders = defenders.Where( defender => defender.IsDead() );
        var deadDefenderNames = SimulationUtils.GetConcatenatedActorNames( deadDefenders.ToList() );

        _messageCenter.AddMessage( $"{concatedFighterNames} attacked {concatedDefenderNames} and killed {deadDefenderNames}" );
        foreach ( var deadActor in deadDefenders )
        {
          _messageCenter.AddCannonMessage( deadActor );
        }
      }
      else if ( combatResponse.Escaped )
      {
        _messageCenter.AddMessage( $"{concatedFighterNames} attacked {concatedDefenderNames}. {concatedDefenderNames} barely escaped" );
      }
    }

    private void MovementRequest( IActor actor )
    {
      // retrieve other party members
      var party = GetParty( actor );

      MovementResponse response = _movementService.Move( new MovementRequest( party, _simulation.GetSimulationSnapshot() ) );

      var partyNames = SimulationUtils.GetConcatenatedActorNames( party );

      _messageCenter.AddMessage( $"{partyNames} moved from {response.PastLocation} to {response.NewLocation}" );
    }

    private List<IActor> GetParty( IActor actor )
    {
      _partyFinder.PartyToFind = actor.PartyId;
      var party = _simulation.GetAliveActors( _partyFinder.FindParty, actor ) ?? new List<IActor>();
      party.Add( actor );
      return party;
    }

    private IActor? GetTributesSurrondingRequest( Coord origin, IActor toIgnore )
    {
      return _simulation.GetRandomActorInArea( origin, toIgnore );
    }
  }
}
