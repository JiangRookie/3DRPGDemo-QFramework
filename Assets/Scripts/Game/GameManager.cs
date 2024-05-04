using QFramework;

namespace Game
{
	public class GameManager : MonoSingleton<GameManager>
	{
		private ObserverManager observerManager = new ObserverManager();

		public void AddObserver(IEndGameObserver observer)
		{
			observerManager.AddObserver(observer);
		}

		public void RemoveObserver(IEndGameObserver observer)
		{
			observerManager.RemoveObserver(observer);
		}

		public void NotifyObservers()
		{
			observerManager.NotifyObservers();
		}
	}
}