using QFramework;
using QFramework.Example;

namespace Game
{
	public partial class GameSceneController : ViewController
	{
		void Start()
		{
			UIKit.OpenPanel<UIPlayerHealthPanel>();
		}
	}
}