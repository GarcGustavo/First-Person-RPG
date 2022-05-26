using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using PlayerComponents;
using SharpNav;
using UnityEngine;
using Random = System.Random;

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
        //private List<Vector3Int> _walls;
        private List<Vector3Int> _optimalPath;
        public bool finishedMoving = true;
        
        private void Awake()
        {
            _manager = GameManager.GetInstance();
            _manager.initializeMovementGrid.AddListener(InitializeUnitMovement);
            //_walls = new List<Vector3Int>();
            _optimalPath = new List<Vector3Int>();
        }
        private void InitializeUnitMovement()
        {
            _uiManager = UIManager.GetInstance();
            gridUnit = transform.GetComponent<GridUnit>();
            speed = _speed;
            turnSpeed = _turnSpeed;
            currentCell = gridUnit._initialCell;
            currentDirection = gridUnit._currentDirection;
            centerOffset = new Vector3(.5f, 1f, .5f);
            UpdateUnitPosition();
        }
        
        private Stack<Vector3Int> _optimalPathStack;
        public float _distance = 0;
        public void CheckCell()
        {
            var player_position = _manager.GetPlayer()._currentCell;
            if (_distance <= 15)
                StartCoroutine(FindOptimalPath(player_position));
            _distance = GetDistance(currentCell, player_position);
            
            targetCell = _optimalPathStack.Any() ? _optimalPathStack.Pop() : currentCell;
            
            var grid_cell = _manager.GetDungeonCell(targetCell);
            if (!finishedMoving) 
                return;
            finishedMoving = false;
            if (grid_cell.occupant == null)
            {
                MoveToCell(grid_cell);
            }
            else
            {
                AttackCell(grid_cell);
            }
        }

        private float GetDistance(Vector3Int start_position, Vector3Int target_position)
        {
            var distance = Mathf.Abs(start_position.x - target_position.x) 
                                      + Mathf.Abs(start_position.y - target_position.y);
            return distance;
        }
        private bool Walkable(Vector3Int cell)
        {
            var grid_cell = _manager.GetDungeonCell(cell);
            if (grid_cell == null) return false;
            if (grid_cell.occupant != null)
                if (grid_cell.occupant.CompareTag("Enemy"))
                    return true;
            return !grid_cell.blocked;
        }

        private IEnumerator FindOptimalPath(Vector3Int player_position)
        {
            if (_distance <= 2)
            {
                //look for available neighboring cells
            }
            
            var search_cells = new List<Vector3Int>();
            var processed_cells = new List<Vector3Int>();
            //Used to track connections between neighbor cells while pathfinding
            var previous_cells = new Dictionary<Vector3Int, Vector3Int>();
            
            var start = currentCell;
            var h_score = GetDistance(currentCell, player_position); // n to target
            var g_score = GetDistance(start, currentCell); // start to n
            var f_score = h_score + g_score;
            
            search_cells.Add(start);
            while (search_cells.Any())
            {
                var current = search_cells[0];
                foreach (var cell in search_cells)
                {
                    var new_h = GetDistance(cell, player_position);
                    var new_g = GetDistance(start, cell);
                    var new_f = new_h + new_g;
                    if ( new_f < f_score || (new_h < h_score && new_f.Equals(f_score)) )
                    {
                        current = cell;
                        h_score = new_h;
                        f_score = new_f;
                    }
                }
                
                processed_cells.Add(current);
                search_cells.Remove(current);
                
                if (current == player_position)
                {
                    var current_path_cell = player_position;
                    var path_stack = new Stack<Vector3Int>();
                    var count = 100;
                    while (current_path_cell != start)
                    {
                        path_stack.Push(current_path_cell);
                        count--;
                        if (count < 0 || !previous_cells.ContainsKey(current_path_cell))
                            break;
                        current_path_cell = previous_cells[current_path_cell];
                    }
                    _optimalPathStack = path_stack;
                    yield return path_stack;
                }
                
                var neighbors = _manager.GetNeighborCells(current);
                foreach (var neighbor in neighbors)
                {
                    if (Walkable(neighbor) && !processed_cells.Contains(neighbor))
                    {
                        var tentative_g = g_score + GetDistance(current, neighbor);
                        var neighbor_g = GetDistance(neighbor, player_position);
                        var in_search = search_cells.Contains(neighbor);
                        if (!in_search || tentative_g < neighbor_g)
                        {
                            previous_cells[neighbor] = current;
                            g_score = tentative_g;
                            if (!in_search)
                            {
                                h_score = GetDistance(neighbor, player_position);
                                search_cells.Add(neighbor);
                            }
                        }
                    }
                }
            }

            //Will need to find a better way of handling unavailable paths
            //Debug.Log("returning null");
            var no_path = new Stack<Vector3Int>();
            _optimalPathStack = no_path;
            yield return no_path;
        }

        private void MoveToCell(GridCell cell)
        {
            var world_position = new Vector3Int(cell.gridPosition.x,
                cell.gridPosition.z,
                cell.gridPosition.y);
            if (!Walkable(cell.gridPosition) || cell.occupant != null)
            {
                _optimalPathStack.Push(cell.gridPosition);
                return;
            }
            transform.DOMove(world_position + centerOffset, 1 / _speed, false);
            _manager.GetDungeonCell(currentCell).Free();
            currentCell = cell.gridPosition;
            cell.Occupy(gridUnit);
            UpdateUnitPosition();
        }
        private void AttackCell(GridCell cell)
        {
            var cell_tag = cell.occupant.tag;
            switch (cell_tag)
            {
                case "Player":
                    _manager.enemyAttack.Invoke(currentCell);
                    _optimalPathStack.Push(cell.gridPosition);
                    break;
            }
        }

        private void UpdateUnitPosition()
        {
            gridUnit._currentCell = currentCell;
            gridUnit._currentDirection = currentDirection;
        }
	}
}