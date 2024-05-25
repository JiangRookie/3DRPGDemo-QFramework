using System.Collections.Generic;

namespace Game
{
	public class GameEndObserverManager
	{
		private List<IGameEndObserver> _GameEndObservers = new List<IGameEndObserver>();

		public void AddObserver(IGameEndObserver observer)
		{
			if (!_GameEndObservers.Contains(observer))
			{
				_GameEndObservers.Add(observer);
			}
		}

		public void RemoveObserver(IGameEndObserver observer)
		{
			if (_GameEndObservers.Contains(observer))
			{
				_GameEndObservers.Remove(observer);
			}
		}

		public void GameEndNotify()
		{
			foreach (var observer in _GameEndObservers)
			{
				observer.GameEndNotify();
			}
		}
	}
}