using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
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
	[SerializeField] private TMP_Text turnCounter;
	[SerializeField] private TMP_Text currentCell;
	[SerializeField] private TMP_Text hp;
	[SerializeField] private TMP_Text mp;
	[SerializeField] private TMP_Text ap;
	[SerializeField] private GameObject gameOver;
	[SerializeField] private CanvasRenderer damageFlash;
	[SerializeField] private Transform skillsPanel;
	[SerializeField] private Image weaponDisplay;
	private GameManager manager;
	private Player player;
	//private PlayerData playerData;
	private int turn;
	
	//public UnityEvent newTurn;
	//public UnityEvent playerMoved;

	//Singleton Setup
	private static UIManager _instance;
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
		manager = GameManager.GetInstance();
		
		
		//manager.newTurn ??= new UnityEvent();
		//manager.playerMoved ??= new UnityEvent();
		//manager.playerMoved.AddListener(UpdateCell);
		manager.enemyTurn.AddListener(UpdateTurnCounter);
		manager.newTurn.AddListener(UpdateHealth);
		manager.newTurn.AddListener(UpdateMana);
		manager.newTurn.AddListener(UpdateAP);
		manager.playerDeath.AddListener(GameOverScreen);
		//manager.playerAttack.AddListener(PlayerAttack);
		//damage.AddListener(UpdateHealth);
		//heal.AddListener(UpdateHealth);
		//levelUp.AddListener(UpdateLevel);
	}

	public void ReloadUI()
	{
		gameOver.SetActive(false);
		damageFlash.SetAlpha(0f);
		weaponDisplay.canvasRenderer.SetAlpha(0f);
	}
	public void PlayerAttack()
	{
		//play attack animation, remove the arguments or create overload
	}

	public void UpdateCell()
	{
		player = manager.GetPlayer();
		if (player != null) currentCell.text = "Cell: (" 
		                                       + player._currentCell.x + ", " 
		                                       + player._currentCell.y + ")";
	}

	void UpdateTurnCounter()
	{
		turn = manager.GetTurn();
		turnCounter.text = "Turn: " + turn;//.ToString();
	}
	
	void UpdateAP()
	{
		  ap.text = "AP: " + player.ActionPoints;//.ToString();
	}

	void UpdateHealth()
	{
		player = manager.GetPlayer();
		if (player != null) hp.text = "HP: " + player._health;
	}

	void UpdateMana()
	{
		player = manager.GetPlayer();
		if (player != null) mp.text = "MP: " + player._mana;
	}

	public void UpdateWeapon(Sprite weaponHUD)
	{
		//weaponDisplay = weaponHUD;
		if (player.weapon != null)
		{
			weaponDisplay.sprite = weaponHUD;
			weaponDisplay.canvasRenderer.SetAlpha(1f);
		}
		else
		{
			weaponDisplay.canvasRenderer.SetAlpha(0f);
		}
	}

	public void UpdateSkills()
	{
		foreach (var skill in manager.GetPlayerData().skills)
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
