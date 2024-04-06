namespace HungerGamesSimulator.MessageCenter
{
    // created to esnure better type saftey
    public interface IBuildable 
    {        
    }

    public class BuilderObject
    {
        public readonly ContextType[]? ContextType;

        public readonly IBuildable Input;

        public BuilderObject( IBuildable input, ContextType[]? inputContext = null )
        {
            ContextType = inputContext;
            Input = input;
        }
    }
}
