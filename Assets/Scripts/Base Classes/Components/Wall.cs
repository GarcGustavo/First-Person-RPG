using UnityEngine;

namespace Base_Classes
{
	public class Wall : GridUnit
	{
		protected void Start()
		{
			//InitializeUnit();
		}
		public void InitializeUnit()
		{
			//Debug.Log("Initializing wall");
		}
		
		

		private void Damage(Vector3Int target, float dmg)
		{
			
		}

		private void LookAtPlayer()
		{
			
		}
		
	}
}