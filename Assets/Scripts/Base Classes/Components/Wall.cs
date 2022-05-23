using UnityEngine;

namespace Base_Classes
{
	public class Wall : GridUnit
	{
		protected void Start()
		{
			//InitializeUnit();
		}
		public override void InitializeUnit(GridCell cell)
		{
			cell.Occupy(this);
		}
		
		

		private void Damage(Vector3Int target, float dmg)
		{
			
		}

		private void LookAtPlayer()
		{
			
		}
		
	}
}