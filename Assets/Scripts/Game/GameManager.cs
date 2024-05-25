using QFramework;

namespace Game
{
	public class GameManager : MonoSingleton<GameManager>
	{
		private GameEndObserverManager _GameEndObserverManager = new GameEndObserverManager();

		public void AddObserver(IGameEndObserver observer)
		{
			_GameEndObserverManager.AddObserver(observer);
		}

		public void RemoveObserver(IGameEndObserver observer)
		{
			_GameEndObserverManager.RemoveObserver(observer);
		}

		public void GameEndNotify()
		{
			_GameEndObserverManager.GameEndNotify();
		}
	}
}