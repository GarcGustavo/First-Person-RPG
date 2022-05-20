using UnityEngine;

namespace Scriptable_Objects
{
	[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Item", order = 0)]
	public class ItemData : UnitData
	{
		public GameObject itemPrefab;
		public Sprite display;
		public ParticleSystem vfx;
		public AudioClip sfx;
		public ItemType itemType;
		public ConsumableType consumableType;

		public enum ItemType
		{
			Equipment,
			Consumable
		}
		public enum ConsumableType
		{
			Heal,
			Damage,
			Points
		}
	}
}