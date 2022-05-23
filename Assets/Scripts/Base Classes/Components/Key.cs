using Managers;
using PlayerComponents;
using Scriptable_Objects;
using UnityEngine;

namespace Base_Classes.Components
{
	public class Key : GridUnit
	{
		[SerializeField] private int _keyNumber;
		[SerializeField] private SpriteRenderer _worldDisplay;
		private GameManager _manager;
		private UIManager _uiManager;
		private Player _player;
		
		public override void InitializeUnit(GridCell cell)
		{
			_initialCell = cell.gridPosition;
			_currentCell = cell.gridPosition;
			cell.Occupy(this);
			_manager = GameManager.GetInstance();
			_uiManager = UIManager.GetInstance();
			_manager.pickUpItem.AddListener(PickUp);
			_worldDisplay = GetComponentInChildren<SpriteRenderer>();
		}
		private void PickUp(GridCell cell)
		{
			if (cell.gridPosition != _currentCell) return;
			_uiManager.LogAction.Invoke("Unlocked door: " + _keyNumber);
			_manager.unlockDoor.Invoke(_keyNumber);
			_worldDisplay.gameObject.SetActive(false);
		}
	}
}