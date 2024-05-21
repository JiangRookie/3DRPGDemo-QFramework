using System;
using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace QFramework.Example
{
	// Generate Id:3485b6a1-6042-4696-87d6-3d881de25609
	public partial class UIGameEndPanel
	{
		public const string Name = "UIGameEndPanel";
		
		[SerializeField]
		public UnityEngine.UI.Text Title;
		[SerializeField]
		public UnityEngine.UI.Button Btn_Return;
		
		private UIGameEndPanelData mPrivateData = null;
		
		protected override void ClearUIComponents()
		{
			Title = null;
			Btn_Return = null;
			
			mData = null;
		}
		
		public UIGameEndPanelData Data
		{
			get
			{
				return mData;
			}
		}
		
		UIGameEndPanelData mData
		{
			get
			{
				return mPrivateData ?? (mPrivateData = new UIGameEndPanelData());
			}
			set
			{
				mUIData = value;
				mPrivateData = value;
			}
		}
	}
}
