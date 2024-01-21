using System.Text.Json.Serialization;

namespace HungerGamesSimulator.Data
{

    public enum DamageType
    {
        Bludgeoning,
        Piercing,
        Slashing
    }

    public struct Weapon
    {
        [JsonInclude]
        public string? Name { get; private set; }
        [JsonInclude]
        public int NumberOfDice { get; private set; }
        [JsonInclude]
        public int TypeOfDice { get; private set; }

        [JsonInclude]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DamageType DamageType { get; private set;  }

        [JsonInclude]
        public bool IsRanged { get; private set; }

        public Weapon()
        {
            Name = "Hands";
            NumberOfDice = 1;
            TypeOfDice = 4;
            DamageType = DamageType.Bludgeoning;
            IsRanged = false;
        }

        public Weapon(string? name, int numberOfDice, int typeOfDice, DamageType damageType, bool isRanged)
        {
            Name = name;
            NumberOfDice = numberOfDice;
            TypeOfDice = typeOfDice;
            DamageType = damageType;
            IsRanged = isRanged;
        }

        public static int RollWeaponDamage(Weapon weapon)
        {
            int sum = 0;
            for (int i = 0; i < weapon.NumberOfDice; i++)
            {
                sum += Random.Shared.Next(1, weapon.TypeOfDice + 1);
            }
            return sum;
        }

        public override string ToString()
        {
            return Name ?? "Null";
        }
    }
}
