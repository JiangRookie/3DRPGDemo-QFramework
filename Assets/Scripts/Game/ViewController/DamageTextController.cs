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

		public static void ShowDamage(Vector3 worldPosition, float amount)
		{
			if (instance == null)
			{
				Debug.LogError("DamageTextController instance is not set.");
				return;
			}

			// 将世界坐标转换为屏幕坐标
			Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);

			GameObject damageTextInstance = Instantiate(instance.FloatingText, instance.transform);
			damageTextInstance.GetComponent<Text>().text = amount.ToString();
			damageTextInstance.transform.position = screenPosition;

			// 使用DoTween实现移动和淡出效果
			Text textComponent = damageTextInstance.GetComponent<Text>();
			textComponent.DOFade(0, 1f); // 1秒内淡出
			damageTextInstance.transform.DOMoveY(screenPosition.y + 100, 1f).OnComplete(() =>
			{
				Destroy(damageTextInstance);
			});
		}
	}
}