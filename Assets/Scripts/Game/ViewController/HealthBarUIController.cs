using QFramework;
using UnityEngine;
using UnityEngine.UI;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace Game.UI
{
	public partial class HealthBarUIController : ViewController
	{
		[SerializeField] private bool _AlwaysVisible;
		[SerializeField] private float _HealthBarDisplayDuration;
		[SerializeField] private float _RemainingHealthBarDisplayTime = 3f;
		private Image _HealthBarImage;
		private Transform _HealthBarUITransform;
		private Transform _MainCameraTransform;

		private void Start()
		{
			SelfCharacterData.OnHealthChanged
			   .Register((curHealth, maxHealth) =>
				{
					if (curHealth <= 0)
					{
						Destroy(_HealthBarUITransform.gameObject);
					}
					_HealthBarUITransform.Show();
					_RemainingHealthBarDisplayTime = _HealthBarDisplayDuration;
					float sliderPercent = (float)curHealth / maxHealth;
					_HealthBarImage.fillAmount = sliderPercent;
				}).UnRegisterWhenGameObjectDestroyed(gameObject);
		}

		private void LateUpdate()
		{
			if (_HealthBarUITransform)
			{
				_HealthBarUITransform.position = HealthBarPointTransform.position;
				_HealthBarUITransform.forward = -_MainCameraTransform.forward;
				if (_RemainingHealthBarDisplayTime <= 0 && !_AlwaysVisible)
				{
					_HealthBarUITransform.Hide();
				}
				else
				{
					_RemainingHealthBarDisplayTime -= Time.deltaTime;
				}
			}
		}

		private void OnEnable()
		{
			_MainCameraTransform = Camera.main.transform;
			foreach (var canvas in FindObjectsOfType<Canvas>())
			{
				if (canvas.renderMode == RenderMode.WorldSpace)
				{
					_HealthBarUITransform = HealthBarPrefab
					   .Instantiate()
					   .Parent(canvas.transform).transform;
					_HealthBarImage = _HealthBarUITransform.GetChild(0).GetComponent<Image>();
					_HealthBarUITransform.gameObject.SetActive(_AlwaysVisible);
				}
			}
		}
	}
}