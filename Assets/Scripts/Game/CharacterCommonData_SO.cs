using UnityEngine;

namespace Game
{
	[CreateAssetMenu(fileName = "CharacterCommonData", menuName = "SO/CreateCharacterCommonData")]
	public class CharacterCommonData_SO : ScriptableObject
	{
		public int MaxHealth;
		public int CurHealth;
		public int BaseDefense;
		public int CurDefense;
		public int Exp;
	}
}