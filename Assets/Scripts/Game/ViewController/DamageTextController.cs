using DG.Tweening;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
	public partial class DamageTextController : ViewController
	{
		private static DamageTextController s_Instance;

		private void Awake()
		{
			s_Instance = this;
		}

		public static void ShowDamage(Vector3 worldPos, float damage)
		{
			Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);
			float cacheDamage = damage;
			s_Instance.FloatingText
			   .InstantiateWithParent(s_Instance.transform)
			   .Position(screenPos)
			   .Self(self =>
				{
					var textComponent = self.GetComponent<Text>();
					textComponent.text = cacheDamage.ToString();

					// 使用DoTween实现移动和淡出效果
					textComponent.DOFade(0, 0.5f); // 1秒内淡出
					self.transform
					   .DOMoveY(screenPos.y + 100, 0.5f)
					   .OnComplete(() => Destroy(self));
				});
		}
	}
}