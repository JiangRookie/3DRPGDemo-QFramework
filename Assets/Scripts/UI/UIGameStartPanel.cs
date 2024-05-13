using UnityEngine.SceneManagement;

namespace QFramework.Example
{
	public class UIGameStartPanelData : UIPanelData { }

	public partial class UIGameStartPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIGameStartPanelData ?? new UIGameStartPanelData();

			// please add init code here
			Btn_GameStart.onClick.AddListener(() =>
			{
				CloseSelf();
				SceneManager.LoadScene("SampleScene");
			});
		}

		protected override void OnOpen(IUIData uiData = null) { }

		protected override void OnShow() { }

		protected override void OnHide() { }

		protected override void OnClose() { }
	}
}