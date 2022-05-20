using UnityEngine;

namespace Scriptable_Objects
{
	[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Player", order = 1)]
	public class PlayerData : UnitData
	{
		//Status
		public float health = 100;
		public float mana = 100;
		public float exp = 0;
		//Stats
		public float maxHealth = 100;
		public float maxMana = 100;
		public int level = 1;
		public int strength;
		public int intelligence;
		public int agility;
		public int luck;
		//Flags
		public bool alive;
		public bool fighting;
		public bool exploring;
		public bool spawned = false;
		//Statuses
		public bool stunned = false;
		public bool poisoned = false;
		public bool confused = false;
		public bool rage = false;
		public bool frozen = false;
		//Skills
		public SkillData[] skills;
		public ItemData[] inventory;
		public WeaponData weapon;
	}
}
