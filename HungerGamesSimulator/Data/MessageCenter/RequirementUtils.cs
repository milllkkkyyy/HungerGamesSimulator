using HungerGamesSimulator.Data;

namespace HungerGamesSimulator.MessageCenter
{
    public class RequirementUtils
    {
        public static bool IsInParty(IActor actor)
        {
            return actor.IsInParty();
        }

        public static bool HasWeapon(IActor actor, string? weapon = null)
        {
            return weapon == null ? actor.Weapon.Name != new Weapon().Name : actor.Weapon.Name != weapon;
        }

        public static bool IsDead(IActor actor)
        {
            return actor.IsDead();
        }

        public static bool HasAdvantage(CornucopiaTribute tribute)
        {
            return tribute.HasAdvantage;
        }
    }
}
