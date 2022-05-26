using UnityEngine;

namespace Scriptable_Objects
{
	[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Enemy", order = 2)]
	public class EnemyData : UnitData
	{
		//Status
		public float health = 100;
		public float mana = 100;
		public float exp = 0;
		//Stats
		//public float attack = 1;
		public float maxHealth = 100;
		public float maxMana = 100;
		public int attack = 1;
		//Stats
		public int level = 1;
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
		//public SkillData[] skills;
		//public GameObject deathVFX;
	}
}
