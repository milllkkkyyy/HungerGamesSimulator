namespace HungerGamesSimulator.Data
{
  public interface IMessageCenter
  {
    public void AddMessage( string message );
    public void AddDeadActor( IActor actor );
    public List<string> GetCannonMessages();
    public void ClearCannonMessages();
    public void ClearMessages();
    public List<string> GetMessages();
  }
}