using UnityEngine;

namespace Game.SO
{
	[CreateAssetMenu(fileName = "CharacterCommonData", menuName = "SO/CreateCharacterCommonData")]
	public class CharacterBaseData_SO : ScriptableObject
	{
		public int MaxHealth;
		public int CurHealth;
		public int BaseDefense;
		public int CurDefense;
		public int Exp;
	}
}