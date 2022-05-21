using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Base_Classes;
using DungeonArchitect;
using DungeonArchitect.Builders.Grid;
using DungeonArchitect.Builders.GridFlow;
using DungeonArchitect.Builders.Maze;
using DungeonArchitect.MiniMaps;
using Flockaroo;
using PlayerComponents;
using Scriptable_Objects;
using States;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using Random = System.Random;
using State = Base_Classes.State;

public class GameManager : MonoBehaviour
{
    //Game State
    private State state;
    public enum turnState
    {
        Player,
        Enemy
    }

    public turnState activeTurn;
    public int turnCounter;
    //[SerializeField] private int maxHeight = 25;
    //[SerializeField] private int maxWidth = 25;
    //[SerializeField] private bool uiEnabled;
    
    //Player State
    [SerializeField] private Player player;
    [SerializeField] private PlayerData playerData;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private bool playerSpawned = false;
    
    //Level generation
    [SerializeField] private Grid grid;
    [SerializeField] private Dungeon dungeon;
    //[SerializeField] private MazeDungeonConfig dungeonConfig;
    [SerializeField] private GridFlowDungeonConfig dungeonConfig;
    [SerializeField] private UIManager uiManager;
    //private bool rebuilding = false;
    public bool dungeonBuilt = false;
    //public int dungeonSizeX;
    //public int dungeonSizeY;

    //Level Data
    [SerializeField] private List<GridCell> gridCells;
    [SerializeField] private List<Enemy> enemies;
    [SerializeField] private Exit exit;
    
    //Movement grid directions
    public enum Direction { North = 0, East = 1, South = 2, West = 3};
    private PlayerGridMovement _movementGrid;
    
    // -------------------------Events-------------------------
    //General
    public UnityEvent initializeMovementGrid;
    public UnityEvent endRound;
    public UnityEvent newTurn;
    //Player
    public UnityEvent<float> playerDamage;
    public UnityEvent<GridCell> pickUpItem;
    public UnityEvent playerHeal;
    public UnityEvent playerDeath;
    //Enemy
    public UnityEvent enemyTurn;
    public UnityEvent<Vector3Int, float> enemyAttack;
    public UnityEvent<Vector3Int, float> unitDamage;
    public UnityEvent<Vector3Int, float> unitHeal;
    
    // -------------------------Singleton Setup-------------------------
    private static GameManager _instance;
    public float _turnCD = .2f;
    private bool _enemyMoving = false;

    public static GameManager GetInstance()
    {
        return _instance;
    }
    void Awake()
    {
        
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        } 
        else 
        {
            Destroy(this);
        }
        
        initializeMovementGrid ??= new UnityEvent();
        endRound ??= new UnityEvent();
        //playerAttack ??= new UnityEvent<Vector3Int, WeaponData>();
        playerDamage ??= new UnityEvent<float>();
        unitDamage ??= new UnityEvent<Vector3Int, float>();
        playerDeath ??= new UnityEvent();
        
        endRound.AddListener(ReloadLevel);
        //initializeMovementGrid ??= new UnityEvent();
        StartGame();
    }
    private void Update()
    {
        if (!playerSpawned) return;
        if (!player._alive) return;
        switch (activeTurn)
        {
            case turnState.Player:
                if (player.ActionPoints > 0 && !_enemyMoving) _movementGrid.GetMovementInput();
                break;
            case turnState.Enemy:
                if (!_enemyMoving)
                {
                    foreach (var enemy in enemies)
                    {
                        enemy.EnemyAction();
                    }
                    enemyTurn.Invoke();
                    UpdateTurn();
                    StartCoroutine(EnemyTurnDelay());
                }
                break;
        }
    }

    private IEnumerator EnemyTurnDelay()
    {
        _enemyMoving = true;
        yield return new WaitForSeconds(.2f);
        _enemyMoving = false;
    }

    // -------------------------Initializers-------------------------

    void StartGame()
    {
        activeTurn = turnState.Player;
        playerSpawned = false;
        StartCoroutine(nameof(InitializeLevel));
        SetState(new Exploring(this));
    }
    IEnumerator InitializeLevel() {
  
        dungeon.RandomizeSeed();
        dungeon.Build();
        
        yield return new WaitUntil(() => dungeonBuilt);

        turnCounter = 0;
        dungeonBuilt = false;
        grid = GetComponentInChildren<Grid>();
        dungeonConfig = transform.GetComponentInChildren<GridFlowDungeonConfig>();
        enemies = new List<Enemy>();
        GenerateNewMap();
        initializeMovementGrid.Invoke();
        newTurn.Invoke();
    }
    private void ReloadLevel()
    {
        StartCoroutine("Rebuild");
    }

    private IEnumerator Rebuild() 
    {
        playerSpawned = false;
        dungeon.RandomizeSeed();
        dungeon.RequestRebuild();
        
        yield return new WaitUntil(() => dungeonBuilt);

        turnCounter = 0;
        dungeonBuilt = false;
        grid = GetComponentInChildren<Grid>();
        dungeonConfig = transform.GetComponentInChildren<GridFlowDungeonConfig>();
        enemies = new List<Enemy>();
        GenerateNewMap();
        initializeMovementGrid.Invoke();
        newTurn.Invoke();
        uiManager.ReloadUI();
        
    }
    // -------------------------Generators-------------------------
    private void GenerateNewMap()
    {
        gridCells.Clear();
        var cellData = dungeon
            .GetComponent<PooledDungeonSceneProvider>()
            .itemParent
            .GetComponentsInChildren<DungeonSceneProviderData>();
        
        foreach (var item in cellData)
        {
            if (!item.GetComponent<GridCell>()) continue;
            var nextGridCell = item.GetComponent<GridCell>();
            nextGridCell.gridPosition = grid.WorldToCell(nextGridCell.transform.position);
            nextGridCell.occupant = null;
            nextGridCell.blocked = false;
            gridCells.Add(nextGridCell);
        }

        foreach (var item in cellData)
        {
            var itemTag = item.tag;
            switch (itemTag)
            {
                case "Player":
                    player = item.GetComponent<Player>();
                    var playerCell = GetDungeonCell(grid.WorldToCell(player.transform.position));
                    playerData.currentCell = playerCell.gridPosition;
                    player.InitializeUnit();
                    playerCell.Occupy(player);
                    player._initialCell = playerCell.gridPosition;
                    player._currentCell = playerCell.gridPosition;
                    player._currentDirection = Direction.North;
                    playerSpawned = true;
                    _movementGrid = player.GetComponent<PlayerGridMovement>();
                    break;
                case "Enemy":
                    var enemy = item.GetComponent<Enemy>();
                    var enemyCell = GetDungeonCell(grid.WorldToCell(enemy.transform.position));
                    enemies.Add(enemy);
                    enemy.InitializeUnit();
                    enemyCell.Occupy(enemy);
                    enemy._initialCell = enemyCell.gridPosition;
                    enemy._currentCell = enemyCell.gridPosition;
                    enemy._currentDirection = Direction.North;
                    enemy._alive = true;
                    break;
                case "Wall":
                    var wall = item.GetComponent<Wall>();
                    var wallCell = GetDungeonCell(grid.WorldToCell(wall.transform.position));
                    wall.InitializeUnit();
                    wallCell.Occupy(wall);
                    break;
                case "Item":
                    var itemDrop = item.GetComponent<Item>();
                    var itemCell = GetDungeonCell(grid.WorldToCell(itemDrop.transform.position));
                    itemDrop.InitializeUnit();
                    itemCell.Occupy(itemDrop);
                    itemDrop._initialCell = itemCell.gridPosition;
                    itemDrop._currentCell = itemCell.gridPosition;
                    break;
                case "Weapon":
                    var weaponDrop = item.GetComponent<Weapon>();
                    var weaponCell = GetDungeonCell(grid.WorldToCell(weaponDrop.transform.position));
                    weaponDrop.InitializeUnit();
                    weaponCell.Occupy(weaponDrop);
                    weaponDrop._initialCell = weaponCell.gridPosition;
                    weaponDrop._currentCell = weaponCell.gridPosition;
                    break;
                case "Exit":
                    exit = item.GetComponent<Exit>();
                    var exitCell = GetDungeonCell(grid.WorldToCell(exit.transform.position));
                    exit.InitializeUnit();
                    exitCell.Occupy(exit);
                    exit._initialCell = exitCell.gridPosition;
                    exit._currentCell = exitCell.gridPosition;
                    break;
            }
        }
    }
    
    // -------------------------GridCell Methods-------------------------
    public GridCell GetDungeonCell(Vector3Int targetPos)
    {
        foreach (var cell in gridCells)
        {
            //Position received needs to swap Z and Y values so that Z=0
            var target = targetPos;
            if (target.x == cell.gridPosition.x && target.y == cell.gridPosition.y)
            {
                //Debug.Log("Requesting cell: " + target + " Returned cell: " + cell.gridPosition);
                return cell;
            }
        }
        return null;
    }

    public void UpdateTurn()
    {
        switch (activeTurn)
        {
            case turnState.Player:
            {
                player.ActionPoints -= 1;
                if (player.ActionPoints <= 0)
                {
                    //Debug.Log("Enemy turn");
                    uiManager.LogAction.Invoke("Enemy turn");
                    activeTurn = turnState.Enemy;
                    turnCounter++;
                }
                newTurn.Invoke();
                break;
            }

            case turnState.Enemy:
            {
                player.ActionPoints = player._agility;
                //Debug.Log("Player turn");
                uiManager.LogAction.Invoke("Player turn");
                activeTurn = turnState.Player;
                newTurn.Invoke();
                break;
            }
            
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    public void EndRound()
    {
        endRound.Invoke();
    }
    
    public List<GridCell> GetGridCells()
    {
        return gridCells;
    }

    public void SetState(State newState)
    {
        state = newState;
        StartCoroutine(state.Enter());
    }

    public State GetState()
    {
        return state;
    }
    public Player GetPlayer()
    {
        return player;
    }

    public PlayerData GetPlayerData()
    {
        return playerData;
    }

    public int GetTurn()
    {
        return turnCounter;
    }
    public Dungeon GetDungeon()
    {
        return dungeon;
    }

    public List<Enemy> GetEnemies()
    {
        return enemies;
    }

    public Camera GetCamera()
    {
        return playerCamera;
    }

}
