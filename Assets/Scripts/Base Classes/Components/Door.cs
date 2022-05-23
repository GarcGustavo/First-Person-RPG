using UnityEngine;

namespace Base_Classes.Components
{
	public class Door : GridUnit
	{
		[SerializeField] private int _doorNumber;
		public bool _locked;
		private GameManager _manager;
		private GridCell _cell;
		
		public override void InitializeUnit(GridCell cell)
		{
			_initialCell = cell.gridPosition;
			_currentCell = cell.gridPosition;
			cell.Occupy(this);
			
			_manager = GameManager.GetInstance();
			_manager.unlockDoor.AddListener(Unlock);
			_cell = cell;
			_locked = true;
		}

		private void Unlock(int key_number)
		{
			if(key_number != _doorNumber) return;
			_locked = false;
			_cell.Free();
			gameObject.SetActive(false);
		}
	}
}