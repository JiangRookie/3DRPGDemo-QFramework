using QFramework;

namespace Game
{
	public class PlayerData
	{
		public static BindableProperty<int> MaxHealth = new BindableProperty<int>(100);
		public static BindableProperty<int> CurHealth = new BindableProperty<int>(100);
		public static BindableProperty<int> BaseDefense = new BindableProperty<int>(2);
		public static BindableProperty<int> CurDefense = new BindableProperty<int>(2);
		public static BindableProperty<float> AttackRange = new BindableProperty<float>(2);
		public static BindableProperty<float> SkillRange = new BindableProperty<float>(0f);
		public static BindableProperty<float> CoolDown = new BindableProperty<float>(0.7f);
		public static BindableProperty<int> MinDamage = new BindableProperty<int>(4);
		public static BindableProperty<int> MaxDamage = new BindableProperty<int>(6);
		public static BindableProperty<float> CriticalHitBonusPercentage = new BindableProperty<float>(2f);
		public static BindableProperty<float> CriticalHitRate = new BindableProperty<float>(0.2f);
		public static BindableProperty<bool> IsCritical = new BindableProperty<bool>(false);
	}
}