using QFramework;
using UnityEngine;

namespace Game
{
	public class Global : Architecture<Global>
	{
		[RuntimeInitializeOnLoadMethod]
		public static void AutoInit()
		{
			ResKit.Init();
			UIKit.Root.SetResolution(1920, 1080, 1);
			PlayerData.Load();

			// 重置血量，用于测试
			PlayerData.CurHealth.Value = PlayerData.MaxHealth.Value;
			PlayerData.RegisterSaveEvent();
			_ = Interface; // 初始化所有的 model 和 system
		}

		protected override void Init() { }
	}
}