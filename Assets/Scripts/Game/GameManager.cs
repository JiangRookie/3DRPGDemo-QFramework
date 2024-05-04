using QFramework;

namespace Game
{
	public class GameManager : MonoSingleton<GameManager>
	{
		public CharacterData PlayerData;
		private ObserverManager observerManager = new ObserverManager();

		public void RegisterPlayer(CharacterData playerData)
		{
			PlayerData = playerData;
		}

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