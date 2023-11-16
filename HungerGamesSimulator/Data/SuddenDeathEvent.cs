namespace HungerGamesSimulator.Data
{
  /// <summary>
  /// Event that simulates suddent death. All actors will attack each other.
  /// </summary>
  public class SuddenDeathEvent
  {
    private List<IActor> actorsInEvent;
    private IMessageCenter messageCenter;
    private bool eventFinished = false;

    public SuddenDeathEvent( List<IActor> actorsInEvent, IMessageCenter messageCenter )
    {
      this.actorsInEvent = actorsInEvent;
      this.messageCenter = messageCenter;
    }

    /// <summary>
    /// Simulate a sudden death event
    /// </summary>
    /// <returns>the actor who wins the sudden death event</returns>
    public IActor Simulate()
    {
      messageCenter.AddMessage( $"A Sudden Death Event started with {SimulationUtils.GetConcatenatedActorNames(actorsInEvent)}" );

      while ( !eventFinished )
      {
        foreach ( var actor in actorsInEvent )
        {
          if ( actor.Health < 1 )
          {
            continue;
          }

          var otherActor = GetRandomActor( actor );

          if ( actor.SimulateHit( otherActor ) )
          {
            otherActor.TakeDamage( SimulationUtils.CalculateDamage( actor ) );
            messageCenter.AddMessage( $"{actor.Name} hit {otherActor.Name} with {actor.Weapon.Name}" );
          }
          else
          {
            messageCenter.AddMessage( $"{actor.Name} missed their attack on {otherActor.Name}" );
          }

          if ( otherActor.Health < 1 )
          {
            messageCenter.AddMessage( $"{actor.Name} slayed {otherActor.Name}" );
            messageCenter.AddCannonMessage( otherActor );
          }

        }

        UpdateEventFinished();
        CombatUtils.Shuffle( actorsInEvent );

      }

      return actorsInEvent.First( actor => actor.Health > 0 );
    }

    private void UpdateEventFinished()
    {
      eventFinished = actorsInEvent.Count( actor => actor.Health > 0 ) == 1;
    }

    private IActor GetRandomActor( IActor toIgnore )
    {
      var aliveActors = actorsInEvent.Where( actor => actor.Health > 0 ).Where( actor => actor != toIgnore ).ToList();
      var otherActor = aliveActors[ Random.Shared.Next( aliveActors.Count() ) ];
      return otherActor;
    }
  }
}
