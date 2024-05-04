using UnityEngine;

namespace Game
{
	public interface IPushable
	{
		void GetPushed(Vector3 pushedToPosition);
	}
}