using QFramework;
using UnityEngine;

// 1.请在菜单 编辑器扩展/Namespace Settings 里设置命名空间
// 2.命名空间更改后，生成代码之后，需要把逻辑代码文件（非 Designer）的命名空间手动更改
namespace Game
{
	public partial class MouseManager : ViewController
	{
		public static EasyEvent<Vector3> OnMouseClicked = new EasyEvent<Vector3>();
		private RaycastHit hitInfo;

		private void Update()
		{
			SetCursorTexture();
			HandleMouseClick();
		}

		// 设置指针贴图
		private void SetCursorTexture()
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

			if (Physics.Raycast(ray, out hitInfo))
			{
				// 切换鼠标贴图
				switch (hitInfo.collider.gameObject.tag)
				{
					case "Ground":
						Cursor.SetCursor(Target, new Vector2(16, 16), CursorMode.Auto);
						break;
				}
			}
		}

		private void HandleMouseClick()
		{
			if (Input.GetMouseButtonDown(0) && hitInfo.collider != null)
			{
				if (hitInfo.collider.gameObject.CompareTag("Ground"))
				{
					OnMouseClicked.Trigger(hitInfo.point);
				}
			}
		}
	}
}