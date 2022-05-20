using UnityEngine;

namespace Base_Classes
{
	public class GridCell : MonoBehaviour
	{
		public Vector3Int gridPosition;
		public GridUnit occupant = null;
		public bool blocked = false;
		public bool trapped = false;
		
		public void Free()
		{
			occupant = null;
			blocked = false;
		}
		
		public void Occupy(GridUnit newOccupant)
		{
			occupant = newOccupant;
			blocked = !newOccupant.CompareTag("Player");
		}
		
	}
}