using System.Drawing;
using System;

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
      actor.GiveWeapon( _weapons[ 0 ] );
      actor.SetLocation( new Coord( Width / 2, Height / 2 ) );
    }
  }

  public void IncreaseDay()
  {
    Day++;
  }

  public IActor? GetRandomActorInArea( Coord center, IActor toIgnore )
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

  public List<IActor>? GetAliveActors( Predicate<IActor>? predicate = null , IActor? toIgnore = null )
  {
    var actors =
        _actors
            .Where( actor => toIgnore != actor )
            .Where( actor => actor.Health > 0 )
            .Where( actor => predicate == null ? true : predicate( actor ) )
            .ToList();

    return ( actors.Count == 0 ) ? null : actors;
  }

  #region Getters/Setters

  #endregion

}
