using QFramework;

namespace Game
{
	public class GameManager : MonoSingleton<GameManager>
	{
		private EndGameObserverManager _EndGameObserverManager = new EndGameObserverManager();

		public void AddObserver(IEndGameObserver observer)
		{
			_EndGameObserverManager.AddObserver(observer);
		}

		public void RemoveObserver(IEndGameObserver observer)
		{
			_EndGameObserverManager.RemoveObserver(observer);
		}

		public void NotifyObservers()
		{
			_EndGameObserverManager.NotifyObservers();
		}
	}
}