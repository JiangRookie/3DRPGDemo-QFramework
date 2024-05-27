using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:1aa6e1b9-2352-45ba-a5cf-8be2a5abd589
	public partial class UIPlayerHealthPanel
	{
		public const string Name = "UIPlayerHealthPanel";
		
		[SerializeField]
		public UnityEngine.UI.Text LevelText;
		[SerializeField]
		public UnityEngine.UI.Image HealthBarForeground;
		[SerializeField]
		public UnityEngine.UI.Text HPText;
		[SerializeField]
		public UnityEngine.UI.Image ExpBarForeground;
		[SerializeField]
		public UnityEngine.UI.Text ExpText;
		[SerializeField]
		public UnityEngine.UI.Text MinDamageValue;
		[SerializeField]
		public UnityEngine.UI.Text MaxDamageValue;
		
		private UIPlayerHealthPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			LevelText = null;
			HealthBarForeground = null;
			HPText = null;
			ExpBarForeground = null;
			ExpText = null;
			MinDamageValue = null;
			MaxDamageValue = null;
			
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
