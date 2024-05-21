using DG.Tweening;
using UnityEngine.SceneManagement;

namespace QFramework.Example
{
	public class UIGameEndPanelData : UIPanelData { }

	public partial class UIGameEndPanel : UIPanel
	{
		protected override void OnInit(IUIData uiData = null)
		{
			mData = uiData as UIGameEndPanelData ?? new UIGameEndPanelData();
			Btn_Return.onClick.AddListener(() =>
			{
				Hide();
				UIKit.ClosePanel<UIPlayerHealthPanel>();
				SceneManager.LoadScene("GameStart");
			});
		}

		protected override void OnOpen(IUIData uiData = null) { }

		protected override void OnShow()
		{
			Title.DOFade(1, 1).From(0);
		}

		protected override void OnHide() { }

		protected override void OnClose() { }
	}
}