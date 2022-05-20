using Base_Classes;
using MoreMountains.Feedbacks;
using Scriptable_Objects;
using UnityEngine;

namespace PlayerComponents
{
	public class Weapon : GridUnit
	{
		[SerializeField] private WeaponData _weaponData;
		[SerializeField] private Sprite weaponHUD;
		[SerializeField] private Sprite worldSprite;
		[SerializeField] private MMFeedbacks _attackFeedbacks;
		private GameManager _manager;
		private UIManager _uiManager;
		private SpriteRenderer _worldDisplay;

		public void InitializeUnit()
		{
			_manager = GameManager.GetInstance();
			_uiManager = UIManager.GetInstance();
			_manager.pickUpItem.AddListener(PickUp);
			_worldDisplay = GetComponentInChildren<SpriteRenderer>();
			_worldDisplay.sprite = worldSprite;
		}

		private void PickUp(GridCell cell)
		{
			if (cell.gridPosition != _currentCell) return;
			var currentWeapon = _manager.GetPlayer().ActiveWeapon;
			if (currentWeapon != null)
			{
				//implement throw weapon function to unequip old weapon and remove from parent
				//_manager.GetPlayer().ActiveWeapon.throwWeapon();
			}

			transform.SetParent(_manager.GetPlayer().WeaponParent);
			_manager.GetPlayer().ActiveWeapon = this;
			//_manager.GetPlayer().weapon = _weaponData;
			_uiManager.UpdateWeapon(weaponHUD);
			_worldDisplay.sprite = null;
		}

		public void Attack(Vector3Int target)
		{
			var player = _manager.GetPlayer();
			var damage = _weaponData.dmg;
			// Bit shift the index of the layer (8) to get a bit mask
			//Collide against any layer other than layer 8
			var layerMask = 1 << 8;
			layerMask = ~layerMask;
			var range = 10f;
			var forwardDirection = transform.TransformDirection(Vector3.forward);
			RaycastHit hit;
			// Does the ray intersect any objects excluding the player layer
			if (Physics.Raycast(transform.position, forwardDirection, out hit, range, layerMask))
			{
				Debug.DrawRay(transform.position, forwardDirection * hit.distance, Color.yellow);
				Debug.Log("Did Hit");

				//TODO if hit tag is enemy or other valid target get its position in grid space
				//use position in grid space to get cellGrid object
				//use cellGrid.occupant to trigger damage function or other enemy related functions
			}
			else
			{
				Debug.DrawRay(transform.position, forwardDirection * 10, Color.white);
				Debug.Log("Did not Hit");
			}

			//_particles.BoundParticleSystem = weaponData.vfx;
			//if(!_attackFeedbacks.Feedbacks.Contains(_particles)) 
			//_attackFeedbacks.Feedbacks.Add(_particles);
			switch (_weaponData.elementType)
			{
				case WeaponData.Element.Physical:
					damage += player._strength;
					break;
				case WeaponData.Element.Bullet:
					damage += player._agility;
					break;
				case WeaponData.Element.Psy:
					damage += player._intelligence;
					break;
				case WeaponData.Element.Magic:
					damage += player._intelligence;
					break;
			}

			if (Random.Range(0, 100) < player._luck) damage *= 2;
			//Instantiate(particles, _cam.transform);
			_attackFeedbacks?.PlayFeedbacks();
			_manager.unitDamage.Invoke(target, damage);
			Debug.Log("Attacking with: " + _weaponData.name);
		}
	}
}