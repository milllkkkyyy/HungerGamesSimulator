namespace HungerGamesSimulator.MessageCenter
{
    public class GameStringFactory
    {
        IReadOnlyList<GameString> _gameStrings;

        public GameStringFactory( IReadOnlyList<GameString> gameStrings ) 
        {
            _gameStrings = gameStrings;
        }

        public GameStringBuilder CreateStringBuilder()
        {
            return new GameStringBuilder( _gameStrings );
        }
    }
}
