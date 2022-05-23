using System.Collections;
using DG.Tweening;
using Managers;
using PlayerComponents;
using Scriptable_Objects;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace Base_Classes
{
	public class Item : GridUnit
	{
		[SerializeField] private ItemData _data;
		[SerializeField] private Sprite itemHUD;
		[SerializeField] private Sprite itemSprite;
		[SerializeField] private SpriteRenderer _worldDisplay;
		private GameManager _manager;
		private InventoryManager _inventoryManager;
		private UIManager _uiManager;
		private Player _player;
		
		public override void InitializeUnit(GridCell cell)
		{
			_initialCell = cell.gridPosition;
			_currentCell = cell.gridPosition;
			cell.Occupy(this);
			
			_manager = GameManager.GetInstance();
			_uiManager = UIManager.GetInstance();
			_inventoryManager = InventoryManager.GetInstance();
			_manager.pickUpItem.AddListener(PickUp);
			_worldDisplay = GetComponentInChildren<SpriteRenderer>();
			_worldDisplay.sprite = itemSprite;
		}
		private void PickUp(GridCell cell)
		{
			if (cell.gridPosition != _currentCell) return;
			if (_data.itemType == ItemData.ItemType.Consumable)
				Consume();
			if (_data.itemType == ItemData.ItemType.Equipment)
				_inventoryManager.items.Add(_data);
			Debug.Log(_data.name + " added to inventory");
			
			//_worldDisplay.sprite = null;
			_worldDisplay.gameObject.SetActive(false);
		}
		private void Consume()
		{
			if (_data.consumableType == ItemData.ConsumableType.Heal)
			{
				Debug.Log(_data.name + " added to inventory");
				_manager.GetPlayer().Heal(1f);
			}
			
			//_worldDisplay.sprite = null;
			_worldDisplay.gameObject.SetActive(false);
		}
	}
}