namespace HungerGamesSimulator.Data
{
  public static class CombatUtils
  {
    public static void Shuffle<T>( this Stack<T> stack )
    {
      var list = stack.ToList();
      stack.Clear();

      int n = list.Count;
      while ( n > 1 )
      {
        n--;
        int k = Random.Shared.Next( n + 1 );
        T value = list[ k ];
        list[ k ] = list[ n ];
        list[ n ] = value;
      }

      foreach ( var actor in list )
      {
        stack.Append( actor );
      }
    }
  }
}
