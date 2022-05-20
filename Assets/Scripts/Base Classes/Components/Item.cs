using System.Collections;
using DG.Tweening;
using Managers;
using PlayerComponents;
using Scriptable_Objects;
using UnityEngine;
using UnityEngine.UI;

namespace Base_Classes
{
	public class Item : GridUnit
	{
		[SerializeField] private ItemData _data;
		[SerializeField] private Sprite itemHUD;
		[SerializeField] private Sprite itemSprite;
		private GameManager _manager;
		private InventoryManager _inventoryManager;
		private UIManager _uiManager;
		private Player _player;
		
		public void InitializeUnit()
		{
			_manager = GameManager.GetInstance();
			_uiManager = UIManager.GetInstance();
			_inventoryManager = InventoryManager.GetInstance();
			_manager.pickUpItem.AddListener(PickUp);
			GetComponentInChildren<SpriteRenderer>().sprite = itemSprite;
		}
		private void PickUp(GridCell cell)
		{
			if (cell.gridPosition != _currentCell) return;
			
			if (_data != null)
			{
				if (_data.itemType == ItemData.ItemType.Consumable) return;
				Consume();
				if (_data.itemType == ItemData.ItemType.Equipment) return;
				_inventoryManager.items.Add(_data);
				Debug.Log(_data.name + " added to inventory");
			}
			
			gameObject.SetActive(false);
		}
		private void Consume()
		{
			if (_data.consumableType == ItemData.ConsumableType.Heal)
			{
				Debug.Log(_data.name + " added to inventory");
				_player.Heal(1f);
			}
			
			gameObject.SetActive(false);
		}
	}
}