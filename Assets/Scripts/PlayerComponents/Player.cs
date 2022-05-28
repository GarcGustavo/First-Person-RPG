using System.Collections;
using System.Linq;
using Base_Classes;
using DG.Tweening;
using DungeonArchitect.UI;
using MoreMountains.Feedbacks;
using Scriptable_Objects;
using UnityEngine;
using Random = System.Random;

namespace PlayerComponents
{
    //TODO refactor to use data fields directly from scriptable object
    public class Player : GridUnit
    {
        [SerializeField] private PlayerData _data;
        [SerializeField] private MMFeedbacks _damageFeedbacks;
        [SerializeField] private MMFeedbacks _healFeedbacks;
        private ParticleSystem _particles;
        private GameManager _manager;
        private UIManager _uiManager;
        public WeaponData weapon;
        public SkillData[] skillsData;
        public float _maxHealth = 100;
        public float _health = 100;
        public float _mana = 100;
        public float _exp = 0;

        //Stats
        public int _level = 1;
        public int _strength;
        public int _intelligence;
        public int _agility;
        public int _luck;

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
        
        private const float _speed = 4f;
        private const float _turnSpeed = 4f;

        private Camera _cam;
        public Weapon activeWeapon;
        public Transform weaponParent;
        
        //public Weapon ActiveWeapon { get; set; }
        //public Transform WeaponParent { get; set; }
        public int ActionPoints { get; set; }
        private void Start()
        {
            _manager = GameManager.GetInstance();
            _uiManager = UIManager.GetInstance();
            _manager.playerDamage.AddListener(Damage);
            //_manager.playerAttack.AddListener(Attack);
            //_manager.playerPickUp.AddListener(PickUpWeapon);
            _alive = true;
        }
        public override void InitializeUnit(GridCell cell)
        {
            _data.currentCell = cell.gridPosition;
            cell.Occupy(this);
            _initialCell = cell.gridPosition;
            _currentCell = cell.gridPosition;
            //Status Info
            _unitName = _data.unitName;
            _unitDesc = _data.unitDesc;
            _health = _data.health;
            _mana = _data.mana;
            _exp = _data.exp;
            //Stats
            _level = _data.level;
            _strength = _data.strength;
            _intelligence = _data.intelligence;
            _agility = _data.agility;
            _luck = _data.luck;
            //Movement
            _currentDirection = GameManager.Direction.North;
            _centerOffset = _data.centerOffset;
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
            _cam = Camera.main;
            _particles = GetComponentInChildren<ParticleSystem>();
            activeWeapon = null;
            ActionPoints = _agility;
            _uiManager.UpdateWeapon(null);
        }
        
        private void CastSkill(Vector3Int target, SkillData skill)
        {
            //_manager.unitCast.Invoke(target, skill);
        }
        public void Heal(float hp)
        {
            _uiManager.LogAction.Invoke(_data.name + " healed "+ hp +" hp");
            _health += hp;
            _healFeedbacks?.PlayFeedbacks();
            if (!(_health > _maxHealth)) return;
            _health = _maxHealth;
            _manager.playerHeal.Invoke();
        }

        private void Damage(float dmg)
        {
            _uiManager.LogAction.Invoke(_data.name + " takes"+ dmg +" damage!");
            _health -= dmg;
            _cam.DOShakePosition(strength: 0.1f, duration: .2f, randomness: 45f, vibrato: 45, fadeOut: true);
            _damageFeedbacks?.PlayFeedbacks();
            if (!(_health <= 0)) return;
            _health = 0;
            _alive = false;
            _manager.playerDeath.Invoke();
        }
        private void PickUpWeapon(WeaponData newWeapon)
        {
            //weapon = newWeapon;
            //_uiManager.UpdateWeapon();
        }
    }
}
