using QFramework;
using QFramework.Example;

namespace Game
{
	public partial class GameStartController : ViewController
	{
		void Start()
		{
			UIKit.OpenPanel<UIGameStartPanel>();
		}
	}
}