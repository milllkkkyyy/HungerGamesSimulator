namespace HungerGamesSimulator.Data;

public class Simulation
{
  private int _width = 5;

  private int _height = 5;

  private List<IActor> _actors;

  private int _day = 1;

  private readonly IMessageCenter _messageCenter;

  public Simulation( IMessageCenter messageCenter, List<IActor> actors )
  {
    this._messageCenter = messageCenter;

    _actors = actors;
    foreach ( var actor in _actors )
    {
      actor.SetLocation( new Coord( _width / 2, _height / 2 ) );
    }
  }

  public void Simulate()
  {
    // clear messages from past day
    _messageCenter.ClearMessages();
    _messageCenter.ClearCannonMessages();
    _messageCenter.AddMessage( $"Day: {_day}" );

    foreach ( var actor in _actors )
    {
      Act( actor, actor.GetState() );
    }

    _day++;
  }

  private void Act( IActor actor, ActorStates state )
  {
    switch ( state )
    {
      case ActorStates.Moving:
      HandleMove( actor );
      break;
      case ActorStates.Attacking:
      HandleAttack( actor );
      break;
      case ActorStates.Dead:
      break;
      default:
      throw new ArgumentOutOfRangeException( nameof( state ), state, null );
    }
  }

  private void HandleAttack( IActor actor )
  {
    var otherActor = GetRandomActorInArea( actor.Location, toIgnore: actor );
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
        otherActor.TakeDamage( DiceHelper.CalculateDamage( actor.Weapon ) );
      }

      escaped = otherActor.SimulateEscape( actor );
    }
    while ( escaped != true );

    if ( otherActor.Health <= 0 )
    {
      _messageCenter.AddMessage( $"{actor.Name} attacked {otherActor.Name} and killed them" );
      _messageCenter.AddDeadActor( otherActor );
    }
    else if ( escaped )
    {
      _messageCenter.AddMessage( $"{actor.Name} attacked {otherActor.Name}, but  {otherActor.Name} successfully escaped" );
    }
  }

  private void HandleMove( IActor actor )
  {
    var wishLocation = actor.SimulateMove();

    if ( wishLocation.X > _width )
    {
      wishLocation.X = _width;
    }
    else if ( wishLocation.X < 0 )
    {
      wishLocation.X = 0;
    }

    if ( wishLocation.Y > _height )
    {
      wishLocation.Y = _height;
    }
    else if ( wishLocation.Y < 0 )
    {
      wishLocation.Y = 0;
    }

    _messageCenter.AddMessage( $"{actor.Name} moved from {actor.Location} to {wishLocation}" );
    actor.SetLocation( wishLocation );
  }

  private IActor? GetRandomActorInArea( Coord center, IActor toIgnore )
  {
    var inArea =
        _actors
            .Where( actor => toIgnore != actor )
            .Where( actor => center.X - 1 <= actor.Location.X && actor.Location.X <= center.X + 1 )
            .Where( actor => center.Y - 1 <= actor.Location.Y && actor.Location.Y <= center.Y + 1 )
            .Where( actor => actor.Health > 0 )
            .ToList();

    return (inArea.Count == 0) ? null : inArea[ Random.Shared.Next( inArea.Count ) ];
  }

  #region Getters/Setters

  public void SetWidth( int width )
  {
    _width = width;
  }

  public void SetHeight( int height )
  {
    _height = height;
  }

  #endregion

}
