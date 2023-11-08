namespace HungerGamesSimulator.Data
{
    public static class CombatUtils
    {
        public static void Shuffle<T>( this Queue<T> queue )
        {
            var list = queue.ToList();
            Shuffle(list);

            queue.Clear();
            foreach ( var actor in list )
            {
                queue.Enqueue( actor );
            }
        }

        public static void Shuffle<T>( this List<T> list )
        {
            int n = list.Count;
            while ( n > 1 )
            {
                n--;
                int k = Random.Shared.Next( n + 1 );
                T value = list[ k ];
                list[ k ] = list[ n ];
                list[ n ] = value;
            }
        }
    }
}
