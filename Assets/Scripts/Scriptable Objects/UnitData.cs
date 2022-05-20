using UnityEngine;

namespace Scriptable_Objects
{
	[UnityEngine.CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/Unit", order = 0)]
	public class UnitData : UnityEngine.ScriptableObject
	{
		//public GameObject prefab;
		//public Sprite unitSprite;
		//public float maxHealth = 100;
		//public float health = 100;

		//Status Info
		public string unitName = "unit";
		public string unitDesc = "description";
		//Movement
		public GameManager.Direction currentDirection = GameManager.Direction.North;
		public Vector3 centerOffset = new Vector3(.5f, 1f, .5f);
		public Vector3Int currentCell;
	}
}