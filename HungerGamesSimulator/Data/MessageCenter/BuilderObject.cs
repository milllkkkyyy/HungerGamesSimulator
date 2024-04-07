namespace HungerGamesSimulator.MessageCenter
{
    // created to esnure better type saftey
    public interface IBuildable 
    {
        public Type GetBuildableType();
    }

    public class BuilderObject
    {
        public readonly ContextType[] ContextType;

        public readonly IBuildable Input;

        public BuilderObject( IBuildable input, params ContextType[] inputContext )
        {
            ContextType = inputContext;
            Input = input;
        }

        public override string ToString()
        {
            return "Type, "  + Input.GetBuildableType() + " Contexts, " + (String.Join(",", ContextType));
        }
    }
}
