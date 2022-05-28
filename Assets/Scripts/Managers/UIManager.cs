using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using DG.Tweening;
using PlayerComponents;
using Scriptable_Objects;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UI;
public class UIManager : MonoBehaviour
{
	[SerializeField] private CombatLog _combatLog;
	[SerializeField] private TMP_Text _nextLog;
	[SerializeField] private TMP_Text turnCounter;
	[SerializeField] private TMP_Text currentCell;
	[SerializeField] private TMP_Text hp;
	[SerializeField] private TMP_Text mp;
	[SerializeField] private TMP_Text ap;
	[SerializeField] private GameObject gameOver;
	[SerializeField] private CanvasRenderer damageFlash;
	[SerializeField] private Transform skillsPanel;
	[SerializeField] private Sprite defaultHUD;
	[SerializeField] private Image weaponDisplay;
	private GameManager _manager;
	private Player _player;
	private int _turn;
	
	//public UnityEvent newTurn;
	//public UnityEvent playerMoved;

	//Singleton Setup
	private static UIManager _instance;
	[DoNotSerialize] public UnityEvent<string> LogAction;

	private void Awake()
	{
		if (_instance == null)
		{
			_instance = this;
			DontDestroyOnLoad(this.gameObject);
		} 
		else 
		{
			Destroy(this);
		}
		ReloadUI();
	}

	public static UIManager GetInstance()
	{
		return _instance;
	}
	private void Start()
	{
		_manager = GameManager.GetInstance();
		
		
		//manager.newTurn ??= new UnityEvent();
		//manager.playerMoved ??= new UnityEvent();
		//manager.playerMoved.AddListener(UpdateCell);
		_manager.enemyTurn.AddListener(UpdateTurnCounter);
		_manager.newTurn.AddListener(UpdateHealth);
		_manager.newTurn.AddListener(UpdateMana);
		_manager.newTurn.AddListener(UpdateAP);
		_manager.playerDeath.AddListener(GameOverScreen);
		//manager.playerAttack.AddListener(PlayerAttack);
		//damage.AddListener(UpdateHealth);
		//heal.AddListener(UpdateHealth);
		//levelUp.AddListener(UpdateLevel);
	}

	public void ReloadUI()
	{
		gameOver.SetActive(false);
		damageFlash.SetAlpha(0f);
		//weaponDisplay.canvasRenderer.SetAlpha(0f);
	}
	public void PlayerAttack()
	{
		//play attack animation, remove the arguments or create overload
	}

	public void UpdateCell()
	{
		_player = _manager.GetPlayer();
		if (_player != null) currentCell.text = "Cell: (" 
		                                       + _player._currentCell.x + ", " 
		                                       + _player._currentCell.y + ")";
	}

	void UpdateTurnCounter()
	{
		_turn = _manager.GetTurn();
		turnCounter.text = "Turn: " + _turn;//.ToString();
	}
	
	void UpdateAP()
	{
		  ap.text = "AP: " + _player.ActionPoints;//.ToString();
	}

	void UpdateHealth()
	{
		_player = _manager.GetPlayer();
		if (_player != null) hp.text = "HP: " + _player._health;
	}

	void UpdateMana()
	{
		_player = _manager.GetPlayer();
		if (_player != null) mp.text = "MP: " + _player._mana;
	}

	public void UpdateWeapon(Sprite weaponHUD)
	{
		weaponDisplay.transform.DOMoveY(130,.5f,false);
		weaponDisplay.sprite = (weaponHUD != null)? weaponHUD : defaultHUD;
		weaponDisplay.canvasRenderer.SetAlpha(1f);
	}

	public void UpdateSkills()
	{
		foreach (var skill in _manager.GetPlayerData().skills)
		{
			Instantiate(skill.display, skillsPanel);
		}
	}

	private void GameOverScreen()
	{
		damageFlash.SetAlpha(.4f);
		gameOver.SetActive(true);
	}
	
}
