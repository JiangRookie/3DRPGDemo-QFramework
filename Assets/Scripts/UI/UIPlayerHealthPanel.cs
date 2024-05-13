using Game;

namespace QFramework.Example
{
	public class UIPlayerHealthPanelData : UIPanelData { }

	public partial class UIPlayerHealthPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIPlayerHealthPanelData ?? new UIPlayerHealthPanelData();

			// please add init code here
			PlayerData.CurHealth.RegisterWithInitValue(curLevel =>
			{
				float percent = (float)curLevel / PlayerData.MaxHealth.Value;
				HealthBarForeground.fillAmount = percent;
			}).UnRegisterWhenGameObjectDestroyed(gameObject);

			PlayerData.MaxHealth.RegisterWithInitValue(maxHealth =>
			{
				float percent = (float)PlayerData.CurHealth.Value / maxHealth;
				HealthBarForeground.fillAmount = percent;
			}).UnRegisterWhenGameObjectDestroyed(gameObject);

			PlayerData.CurExp.RegisterWithInitValue(curExp =>
			{
				float percent = (float)curExp / PlayerData.ExpToNextLevel.Value;
				ExpBarForeground.fillAmount = percent;
			}).UnRegisterWhenGameObjectDestroyed(gameObject);

			PlayerData.ExpToNextLevel.RegisterWithInitValue(expToNextLevel =>
			{
				float percent = (float)PlayerData.CurExp.Value / expToNextLevel;
				ExpBarForeground.fillAmount = percent;
			}).UnRegisterWhenGameObjectDestroyed(gameObject);

			PlayerData.CurLevel.RegisterWithInitValue(level =>
			{
				LevelText.text = $"Level {level:00}";
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
		}

		protected override void OnOpen(IUIData uiData = null) { }

		protected override void OnShow() { }

		protected override void OnHide() { }

		protected override void OnClose() { }
	}
}