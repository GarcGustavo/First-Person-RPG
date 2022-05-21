using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using PlayerComponents;
using SharpNav;
using UnityEngine;

namespace Base_Classes
{
	public class UnitGridMovement : MonoBehaviour
	{
        //Singleton
        private GameManager _manager;
        private UIManager _uiManager;
        
        private GridUnit gridUnit;
        private float speed;
        private float turnSpeed;
        private Vector3 centerOffset;
        private Vector3Int currentCell;
        private Vector3Int targetCell;
        private GameManager.Direction currentDirection;
        private float _speed = 4f;
        private float _turnSpeed = 4f;
        
        private void Awake()
        {
            _manager = GameManager.GetInstance();
            _manager.initializeMovementGrid.AddListener(InitializeUnitMovement);
        }
        private void InitializeUnitMovement()
        {
            //Debug.Log("Initializing grid");
            _uiManager = UIManager.GetInstance();
            gridUnit = transform.GetComponent<GridUnit>();
            speed = _speed;
            turnSpeed = _turnSpeed;
            currentCell = gridUnit._initialCell;
            currentDirection = gridUnit._currentDirection;
            centerOffset = new Vector3(.5f, 1f, .5f);
        
            //Cursor.lockState = CursorLockMode.Locked;
            UpdateUnitPosition();
        }
    
        public IEnumerator MoveToCell()
        {
            //var origin = new Vector3Int(0, 0, 0);
            yield return new WaitForSeconds(.05f);
            var playerPosition = _manager.GetPlayer()._currentCell;
            var neighborCells = new List<Vector3Int>
            {
                currentCell + Vector3Int.up,
                currentCell + Vector3Int.right,
                currentCell + Vector3Int.down,
                currentCell + Vector3Int.left
            };
            var viableCells = new List<GridCell>();
            foreach (var cell in neighborCells)
            {
                //Debug.Log(currentCell+" neighborCell: " +  cell);
                var next = _manager.GetDungeonCell(cell);
                if(next != null && (!next.blocked||next.occupant.CompareTag("Player"))) viableCells.Add(next);
            }
            //Debug.Log("viableCells: " +  viableCells.Count);
            targetCell = currentCell;
            foreach (var nextCell in viableCells)
            {
                var lastDistance = Mathf.Sqrt(Mathf.Pow(playerPosition.x - targetCell.x, 2) 
                                              + Mathf.Pow(playerPosition.y - targetCell.y, 2));
                var nextDistance = Mathf.Sqrt(Mathf.Pow(playerPosition.x - nextCell.gridPosition.x, 2) 
                                              + Mathf.Pow(playerPosition.y - nextCell.gridPosition.y, 2));
                if (nextDistance >= lastDistance) continue;
                targetCell = nextCell.gridPosition;
            }

            var gridCell = _manager.GetDungeonCell(targetCell);
            var worldPosition = new Vector3Int(gridCell.gridPosition.x,
                gridCell.gridPosition.z,
                gridCell.gridPosition.y);
                
            if (gridCell.occupant == null)
            {
                //Debug.Log("Enemy at:" + currentCell + " is moving to cell: " + targetCell);
                transform.DOMove(worldPosition + centerOffset, 1 / _speed, false);
                _manager.GetDungeonCell(currentCell).Free();
                currentCell = gridCell.gridPosition;
                gridCell.Occupy(gridUnit);
                UpdateUnitPosition();
            }
            else
            {
                var cellTag = gridCell.occupant.tag;
                //Debug.Log("Target cell is occupied by: " + cellTag);
                switch (cellTag)
                {
                    case "Player":
                        var attack = 10f;
                        //transform.DOShakePosition(strength: 0.1f, duration: .2f, randomness: 45f, vibrato: 45, fadeOut: true);
                        //transform.transform.DOLocalMove(Vector3.zero, .1f, false);
                        _manager.enemyAttack.Invoke(currentCell, attack);
                        //_manager.GetEnemy(currentCell).Attack(currentCell, attack);
                        //yield return new WaitForSeconds(.2f);
                        //_manager.playerDamage.Invoke(10f);
                        break;
                    case "Wall":
                        //StartCoroutine(UnitAttack(gridCell.gridPosition));
                        break;
                    default:
                        //transform.DOShakePosition(strength: 0.1f, duration: .2f, randomness: 45f, vibrato: 45, fadeOut: true);
                        //transform.transform.DOLocalMove(Vector3.zero, .1f, false);
                        break;
                }
            }
            
            //var cell = _manager.GetDungeonCell(targetCell);
            yield break;
            //yield return new WaitForSeconds(.1f);
            //_manager.UpdateTurn();
        }

        private void UpdateUnitPosition()
        {
            //Debug.Log("New unit cell: " + currentCell);
            gridUnit._currentCell = currentCell;
            gridUnit._currentDirection = currentDirection;
            //_manager.unitMoved.Invoke(gridUnit);
        }
	}
}