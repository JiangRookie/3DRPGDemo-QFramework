using System.Collections.Generic;

namespace Game
{
	public class EndGameObserverManager
	{
		private List<IEndGameObserver> _EndGameObservers = new List<IEndGameObserver>();

		public void AddObserver(IEndGameObserver observer)
		{
			if (!_EndGameObservers.Contains(observer))
			{
				_EndGameObservers.Add(observer);
			}
		}

		public void RemoveObserver(IEndGameObserver observer)
		{
			if (_EndGameObservers.Contains(observer))
			{
				_EndGameObservers.Remove(observer);
			}
		}

		public void NotifyObservers()
		{
			foreach (var observer in _EndGameObservers)
			{
				observer.EndNotify();
			}
		}
	}
}