using QFramework;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace Game.UI
{
	public partial class PlayerHealthCanvas : ViewController
	{
		private void Start()
		{
			PlayerData.CurHealth.RegisterWithInitValue(curLevel =>
			{
				float percent = (float)curLevel / PlayerData.MaxHealth.Value;
				HealthBar.fillAmount = percent;
			}).UnRegisterWhenGameObjectDestroyed(gameObject);

			PlayerData.MaxHealth.RegisterWithInitValue(maxHealth =>
			{
				float percent = (float)PlayerData.CurHealth.Value / maxHealth;
				HealthBar.fillAmount = percent;
			}).UnRegisterWhenGameObjectDestroyed(gameObject);

			PlayerData.CurExp.RegisterWithInitValue(curExp =>
			{
				float percent = (float)curExp / PlayerData.ExpToNextLevel.Value;
				ExpBar.fillAmount = percent;
			}).UnRegisterWhenGameObjectDestroyed(gameObject);

			PlayerData.ExpToNextLevel.RegisterWithInitValue(expToNextLevel =>
			{
				float percent = (float)PlayerData.CurExp.Value / expToNextLevel;
				ExpBar.fillAmount = percent;
			}).UnRegisterWhenGameObjectDestroyed(gameObject);

			PlayerData.CurLevel.RegisterWithInitValue(level =>
			{
				LevelText.text = $"Level {level:00}";
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
		}
	}
}