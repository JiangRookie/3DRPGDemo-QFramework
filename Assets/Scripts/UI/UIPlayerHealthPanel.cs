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
			PlayerData.CurHealth.RegisterWithInitValue(_ =>
			{
				float percent = (float)PlayerData.CurHealth.Value / PlayerData.MaxHealth.Value;
				HealthBarForeground.fillAmount = percent;
				HPText.text = $"{PlayerData.MaxHealth.Value.ToString()}/{PlayerData.CurHealth.Value.ToString()}";
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
			
			PlayerData.MaxHealth.RegisterWithInitValue(_ =>
			{
				float percent = (float)PlayerData.CurHealth.Value / PlayerData.MaxHealth.Value;
				HealthBarForeground.fillAmount = percent;
				HPText.text = $"{PlayerData.MaxHealth.Value.ToString()}/{PlayerData.CurHealth.Value.ToString()}";
			}).UnRegisterWhenGameObjectDestroyed(gameObject);

			PlayerData.CurExp.RegisterWithInitValue(curExp =>
			{
				float percent = (float)curExp / PlayerData.ExpToNextLevel.Value;
				ExpBarForeground.fillAmount = percent;
				ExpText.text = $"{PlayerData.ExpToNextLevel.Value.ToString()}/{PlayerData.CurExp.Value.ToString()}";
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
			
			PlayerData.ExpToNextLevel.RegisterWithInitValue(curExp =>
			{
				float percent = (float)curExp / PlayerData.ExpToNextLevel.Value;
				ExpBarForeground.fillAmount = percent;
				ExpText.text = $"{PlayerData.ExpToNextLevel.Value.ToString()}/{PlayerData.CurExp.Value.ToString()}";
			}).UnRegisterWhenGameObjectDestroyed(gameObject);

			PlayerData.CurLevel.RegisterWithInitValue(level =>
			{
				LevelText.text = $"Level {level:00}";
			}).UnRegisterWhenGameObjectDestroyed(gameObject);

			PlayerData.MinDamage.RegisterWithInitValue(minDamage =>
			{
				MinDamageValue.text = PlayerData.MinDamage.Value.ToString();
			}).UnRegisterWhenGameObjectDestroyed(gameObject);

			PlayerData.MaxDamage.RegisterWithInitValue(maxDamage =>
			{
				MaxDamageValue.text = PlayerData.MaxDamage.Value.ToString();
			}).UnRegisterWhenGameObjectDestroyed(gameObject);
		}

		protected override void OnOpen(IUIData uiData = null) { }

		protected override void OnShow() { }

		protected override void OnHide() { }

		protected override void OnClose() { }
	}
}