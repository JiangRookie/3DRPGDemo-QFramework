using System.Collections;
using QFramework;
using QFramework.Example;
using UnityEngine;

namespace Game
{
	public partial class GameSceneController : ViewController
	{
		private float _DelayShowGameEndPanelTime = 3.0f;

		void Start()
		{
			UIKit.OpenPanel<UIPlayerHealthPanel>();

			PlayerData.CurHealth.RegisterWithInitValue(value =>
			{
				if (value <= 0)
				{
					Debug.Log("Game Over!");
					StartCoroutine(DelayOpenGameEndPanel());
				}
			}).UnRegisterWhenCurrentSceneUnloaded();
		}

		private IEnumerator DelayOpenGameEndPanel()
		{
			yield return new WaitForSeconds(_DelayShowGameEndPanelTime);
			UIKit.OpenPanel<UIGameEndPanel>();
		}
	}
}