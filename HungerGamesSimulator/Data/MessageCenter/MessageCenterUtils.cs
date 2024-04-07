using HungerGamesSimulator.Data;
using System.Reflection;

namespace HungerGamesSimulator.MessageCenter
{
    public static class MessageCenterUtils
    {
        public static Type StringToType(string id) => id.ToLower() switch
        {
            "tribute" => typeof(Tribute),
            "party" => typeof(Party),
            "int" => typeof(int),
            "cornucopiatribute" => typeof(CornucopiaTribute),
            _ => throw new NotImplementedException($"Not expected id value: {id}"),
        };

        public static MethodInfo StringToMethod(Type type, string funcName)
        {
            return type.GetMethod(funcName, BindingFlags.Public | BindingFlags.Static) ?? throw new NotImplementedException($"Not expected function name: {funcName}");
        }

        public static IReadOnlyList<GameString> DesignerStringToGameString(IReadOnlyList<DesignerString> designerStrings)
        {
            var result = new List<GameString>();

            foreach (var designerString in designerStrings)
            {
                GameStringInputArgs[] inputArgs = new GameStringInputArgs[designerString.Inputs.Length];

                for (int j = 0; j < designerString.Inputs.Length; j++)
                {
                    var designerStringInput = designerString.Inputs[j];

                    inputArgs[j] = new GameStringInputArgs()
                    {
                        Type = StringToType(designerStringInput.Type)
                    };

                    inputArgs[j].Contexts = designerStringInput.Contexts;

                    if (designerStringInput.Requirements == null)
                    {
                        continue;
                    }

                    Requirement[] requirements = new Requirement[designerStringInput.Requirements.Length];
                    for (int i = 0; i < requirements.Length; i++)
                    {
                        var requirement = designerStringInput.Requirements[i];

                        // remove negation
                        var past = requirement;
                        requirement = requirement.Trim('!');

                        // check if requirement has an additional parameter
                        if (!requirement.Contains('='))
                        {
                            requirements[i] = new Requirement(StringToMethod(typeof(RequirementUtils), requirement), past != requirement);
                        }
                        else
                        {
                            requirements[i] = new Requirement(StringToMethod(typeof(RequirementUtils), requirement[..requirement.IndexOf('=')]), past != requirement, requirement[(requirement.IndexOf('=') + 1)..]);
                        }
                    }

                    inputArgs[j].Requirements = requirements;
                }

                result.Add(new GameString(new GameStringInput(inputArgs), designerString.Texts, designerString.TextType, designerString.Contexts));
            }

            return result;
        }
    }
}
