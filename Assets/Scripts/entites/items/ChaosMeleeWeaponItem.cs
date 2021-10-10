namespace services
{
    public class ChaosMeleeWeaponItem: ChoasItem
    {
        public ChaosMeleeWeaponItem(): base()
        {
            InitStat(ChaosEntityStatType.Attack, 5);
            InitStat(ChaosEntityStatType.MeleeRange, 1);
            InitStat(ChaosEntityStatType.Windup, 1);
            InitStat(ChaosEntityStatType.Cooldown, 2);
        }
    }
}