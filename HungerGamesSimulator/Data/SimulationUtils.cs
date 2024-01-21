using System.Collections.Generic;

namespace HungerGamesSimulator.Data
{
    public static class SimulationUtils
    {
        public static int RollD20()
        {
            return Random.Shared.Next( 1, 21 );
        }

        public static int CalculateDamage( IActor actor )
        {
            int damage = Weapon.RollWeaponDamage( actor.Weapon );
            return actor.Weapon.IsRanged ? damage + actor.Strength : damage + actor.Dexerity;
        }

        public static bool FindPartyMembers( IActor actor, Guid partyId )
        {
            return actor.IsInParty() && actor.PartyId == partyId;
        }
    }
}
