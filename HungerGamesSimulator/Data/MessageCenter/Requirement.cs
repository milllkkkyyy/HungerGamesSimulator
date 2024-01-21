using System.Reflection;

namespace HungerGamesSimulator.MessageCenter
{
    /// <summary>
    /// An object that contains requirements that should be meet for a valid use of a game string
    /// </summary>
    public class Requirement
    {
        private readonly MethodInfo _method;
        private readonly string? _additionalInput;
        private readonly bool _negated = false;

        public Requirement(MethodInfo method, bool negated, string? additionalInput = null )
        {
            _method = method;
            _additionalInput = additionalInput;
            _negated = negated;
        }

        /// <summary>
        /// Check to see if the requirement is met with the given inputs
        /// </summary>
        /// <param name="input">The object that needs to check for requirements</param>
        /// <returns>true, if the object meets the requirements, otherwise false </returns>
        /// <exception cref="Exception">Throws an exception is the method was not found</exception>
        public bool CheckRequirement( object input )
        {
            bool result;
            if (_additionalInput == null)
            {
                result = (bool)(_method.Invoke(null, new object[1] { input }) ?? false);
            }
            else
            {
                result = (bool)(_method.Invoke(null, new object[2] { input, _additionalInput }) ?? false);
            }

            if (_negated)
            {
                return !result;
            }

            return result;
        }
    }
}
