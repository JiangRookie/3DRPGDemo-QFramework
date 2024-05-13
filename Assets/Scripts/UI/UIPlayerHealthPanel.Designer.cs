using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:80a71601-2c65-4103-8932-5fd259796449
	public partial class UIPlayerHealthPanel
	{
		public const string Name = "UIPlayerHealthPanel";
		
		[SerializeField]
		public UnityEngine.UI.Text LevelText;
		[SerializeField]
		public UnityEngine.UI.Image HealthBarForeground;
		[SerializeField]
		public UnityEngine.UI.Image ExpBarForeground;
		
		private UIPlayerHealthPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			LevelText = null;
			HealthBarForeground = null;
			ExpBarForeground = null;
			
			mData = null;
		}
		
		public UIPlayerHealthPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIPlayerHealthPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIPlayerHealthPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
