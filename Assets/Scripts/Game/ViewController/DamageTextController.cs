using DG.Tweening;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
	public partial class DamageTextController : ViewController
	{
		private static DamageTextController instance;

		private void Awake()
		{
			instance = this;
		}

		public static void ShowDamage(Vector3 worldPos, float damage)
		{
			if (!instance)
			{
				Debug.LogError("DamageTextController instance is not set.");
				return;
			}

			// 将世界坐标转换为屏幕坐标
			Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);

			instance.FloatingText
			   .InstantiateWithParent(instance.transform)
			   .Position(screenPos)
			   .Self(self =>
				{
					var textComponent = self.GetComponent<Text>();
					textComponent.text = damage.ToString();

					// 使用DoTween实现移动和淡出效果
					textComponent.DOFade(0, 1f); // 1秒内淡出
					self.transform.DOMoveY(screenPos.y + 100, 1f).OnComplete(() =>
					{
						Destroy(self);
					});
				});
		}
	}
}