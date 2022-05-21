using System;
using System.Collections;
using DG.Tweening;
using DungeonArchitect.Flow.Exec;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using PlayerComponents;
using Scriptable_Objects;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

namespace Base_Classes
{
	public class Enemy : GridUnit
	{
		[SerializeField] private EnemyData _data;
		private GameManager _manager;
		private UIManager _uiManager;
		private UnitGridMovement _unitGrid;
		private Player _player;
		public float _maxHealth;
		public float _health;
		public int _level;
		//public float _speed = 4f;
		//public float _turnSpeed = 4f;

		//Flags
		public bool _alive;
		public bool _fighting;
		public bool _exploring;
		public bool _spawned = false;

		//Statuses
		public bool _stunned = false;
		public bool _poisoned = false;
		public bool _confused = false;
		public bool _rage = false;
		public bool _frozen = false;
		public ParticleSystem _deathVFX;

		public MMHealthBar hpBar;
		public MMFeedbacks DamageFeedbacks;
		public MMFeedbacks AttackFeedbacks;

		// Start is called before the first frame update
		protected virtual void Start()
		{
			_manager = GameManager.GetInstance();
			_manager.unitDamage.AddListener(Damage);
			_manager.enemyTurn.AddListener(EnemyAction);
			_manager.enemyAttack.AddListener(Attack);
			//_manager.playerMoved.AddListener(LookAtCamera);
			//InitializeUnit();
		}

		private void Update()
		{
			LookAtCamera();
		}

		public void InitializeUnit()
		{
			//Debug.Log("Initializing unit " + name);
			//Status Info
			//_spriteRenderer = GetComponentInChildren<SpriteRenderer>();
			//_spriteRenderer.sprite = _data.unitSprite;
			_uiManager = UIManager.GetInstance();
			_unitName = _data.unitName;
			_unitDesc = _data.unitDesc;
			_health = _data.health;
			_maxHealth = _data.maxHealth;
			//Stats
			_level = _data.level;
			//Flags
			_alive = true;
			_fighting = false;
			_exploring = true;
			_spawned = true;
			//Statuses
			_stunned = false;
			_poisoned = false;
			_confused = false;
			_rage = false;
			_frozen = false;
			//_deathVFX = _data.deathVFX.GetComponent<ParticleSystem>();
			//_deathVFX = GetComponentInChildren<ParticleSystem>();

			_unitGrid = GetComponent<UnitGridMovement>();
			hpBar.UpdateBar(_health, 0, _maxHealth, _alive);
		}

		public void EnemyAction()
		{
			//Move towards player position
			if (_manager.turnCounter > 1 && gameObject.activeSelf)
			{
				StartCoroutine(_unitGrid.MoveToCell());
			}
		}
		public void Attack(Vector3Int unitCell, float dmg)
		{
			//var damage = weaponData.dmg;
			//Debug.Log("Enemy attacks: " + target + " for: " + dmg);
			//_manager.unitDamage.Invoke(target, dmg);
			if (_currentCell == unitCell)
			{
				AttackFeedbacks?.PlayFeedbacks();
				_uiManager.LogAction.Invoke(_data.name + " attacks!");
				_manager.playerDamage.Invoke(dmg);
				//_manager.GetPlayer().Damage(dmg);
			}
		}
		private void Damage(Vector3Int target, float dmg)
		{
			if (target == _currentCell && _alive)
			{
				DamageFeedbacks?.PlayFeedbacks();
				_health -= dmg;
				hpBar.UpdateBar(_health, 0, _maxHealth, _alive);
				if (!(_health <= 0)) return;
				var cell = _manager.GetDungeonCell(target);
				Instantiate(_deathVFX, cell.transform.position + Vector3.up/2, Quaternion.identity);
				//_deathVFX.Play(withChildren:true);
				_alive = false;
				cell.Free();
				gameObject.SetActive(_alive);
			}
		}
		private void LookAtCamera()
		{
			if (_alive)
			{
				//StartCoroutine(Rotate(_manager.GetPlayer().transform));
				//transform.DOLookAt(Camera.main.transform.position, .25f, AxisConstraint.Y, null);
				transform.LookAt(Camera.main.transform.position);
			}
		}

		private IEnumerator Rotate(Transform target)
		{
			transform.DOLookAt(target.position, .25f, AxisConstraint.Y, null);
			yield break;
		}
		
	}
}
