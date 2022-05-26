using System.Collections;
using System.Linq;
using Base_Classes;
using DG.Tweening;
using DungeonArchitect.UI.Widgets.GraphEditors;
using Scriptable_Objects;
using UnityEngine;

namespace PlayerComponents
{
    public class PlayerGridMovement : UnitGridMovement
    {
        //Singletons and components
        private GameManager _playerManager;
        private UIManager _playerUIManager;
        private Camera _cam;
        private Player _player;
        
        private bool _isMoving;
        private Vector3Int _targetCell;
        private Vector3Int _currentCell;
        private GameManager.Direction _currentDirection;

        private const float _speed = 4f;
        private const float _turnSpeed = 4f;
        private const float _headBob = 0.1f;
        private readonly Vector3 _centerOffset = new Vector3(.5f, 1f, .5f);

        void Start()
        {
            _playerManager = GameManager.GetInstance();
            _playerUIManager = UIManager.GetInstance();
            _cam = Camera.main;
            _playerManager.initializeMovementGrid.AddListener(InitializeMovement);
        }
        private void InitializeMovement()
        {
            _player = _playerManager.GetPlayer();
            _currentCell = _player._initialCell;
            _targetCell = _currentCell;
            _currentDirection = _player._currentDirection;
            //Cursor.lockState = CursorLockMode.Locked;
            UpdatePlayerPosition();
        }

        private void UpdatePlayerPosition()
        {
            _player._currentCell = _currentCell;
            _player._currentDirection = _currentDirection;
            //_playerManager.playerMoved.Invoke();
            _playerUIManager.UpdateCell();
        }

        public void GetMovementInput()
        {
            if (_isMoving) return;
            
            if (Input.GetButton("Up"))
            {
                _isMoving = true;
                StartCoroutine(MovePlayerToCell());
            }
            else if (Input.GetButtonDown("Down"))
            {
                var direction = ((int)_currentDirection + 2) % 4;
                _currentDirection = (GameManager.Direction) direction;
                _player._currentDirection = _currentDirection;
                //Debug.Log("Direction: " + _currentDirection);
                
                _isMoving = true;
                StartCoroutine(RotatePlayer(Vector3.down * 180));
            }
            else if (Input.GetButtonDown("Left"))
            {
                var direction = ((int) _currentDirection + 3) % 4;

                _currentDirection = (GameManager.Direction) direction;
                _player._currentDirection = _currentDirection;
                //Debug.Log("Direction: " + _currentDirection);
                _isMoving = true;
                StartCoroutine(RotatePlayer(Vector3.down * 90));
            }
            else if (Input.GetButtonDown("Right"))
            {
                var direction = ((int)_currentDirection + 1)%4;
                
                _currentDirection = (GameManager.Direction) direction;
                _player._currentDirection = _currentDirection;
                //Debug.Log("Direction: " + _currentDirection);
                _isMoving = true;
                StartCoroutine(RotatePlayer(Vector3.up * 90));
            }
            else if (Input.GetButton("Fire1"))
            {
                var attackTargetCell = _currentDirection switch
                {
                    GameManager.Direction.North => _currentCell + Vector3Int.up,
                    GameManager.Direction.East => _currentCell + Vector3Int.right,
                    GameManager.Direction.South => _currentCell + Vector3Int.down,
                    GameManager.Direction.West => _currentCell + Vector3Int.left,
                    _ => _currentCell
                };
                _isMoving = true;
                StartCoroutine(PlayerAttack(attackTargetCell));
            }
            else if (Input.GetButtonDown("Fire2"))
            {
                
            }
        }
        private IEnumerator RotatePlayer(Vector3 rotation)
        {
            if (!_isMoving) yield break;
            
            transform.DORotate(transform.eulerAngles + rotation, 1 / _turnSpeed, RotateMode.Fast);
            yield return new WaitForSeconds(_playerManager._turnCD);
            _isMoving = false;
        }
    
        private IEnumerator PlayerAttack(Vector3Int attackTargetCell)
        {
            if (!_isMoving) yield break;
            
            _playerUIManager.LogAction.Invoke("Attacked cell " + attackTargetCell.x + ", " + attackTargetCell.y);
            _player.ActiveWeapon?.Attack(attackTargetCell);
            _playerManager.UpdateTurn();
            yield return new WaitForSeconds(_playerManager._turnCD);
            _isMoving = false;
        }
    
        private IEnumerator MovePlayerToCell()
        {
            if (!_isMoving) yield break;
            
            _targetCell = _currentDirection switch
            {
                GameManager.Direction.North => _currentCell + Vector3Int.up,
                GameManager.Direction.East => _currentCell + Vector3Int.right,
                GameManager.Direction.South => _currentCell + Vector3Int.down,
                GameManager.Direction.West => _currentCell + Vector3Int.left,
                _ => _currentCell
            };
            var cell = _playerManager.GetDungeonCell(_targetCell);
            CheckCell(cell);
            UpdatePlayerPosition();
            _playerUIManager.LogAction.Invoke("Moved to cell " + _currentCell.x + ", " + _currentCell.y);
            _playerManager.UpdateTurn();
            yield return new WaitForSeconds(_playerManager._turnCD);
            _isMoving = false;
        }
        private void CheckCell(GridCell cell)
        {
            if (cell != null)
            {
                var body_position = new Vector3Int(_targetCell.x, _targetCell.z, _targetCell.y);
                if (cell.blocked)
                {
                    //Debug.Log("Cell is blocked!");
                    var cellTag = cell.occupant.tag;
                    switch (cellTag)
                    {
                        case "Enemy":
                            _cam.DOShakePosition(strength: 0.1f, duration: .2f, randomness: 45f, vibrato: 45, fadeOut: true);
                            _cam.transform.DOLocalMove(Vector3.zero, .1f, false);
                            break;
                        case "Item":
                            _playerManager.pickUpItem.Invoke(cell);
                            transform.DOMove(body_position + _centerOffset,
                                1 / _speed, false);
                            _playerManager.GetDungeonCell(_currentCell).Free();
                            _currentCell = _targetCell;
                            cell.Occupy(_player);
                            break;
                        case "Key":
                            _playerManager.pickUpItem.Invoke(cell);
                            transform.DOMove(body_position + _centerOffset,
                                1 / _speed, false);
                            _playerManager.GetDungeonCell(_currentCell).Free();
                            _currentCell = _targetCell;
                            cell.Occupy(_player);
                            break;
                        case "Weapon":
                            _playerManager.pickUpItem.Invoke(cell);
                            transform.DOMove(body_position + _centerOffset,
                                1 / _speed, false);
                            _playerManager.GetDungeonCell(_currentCell).Free();
                            _currentCell = _targetCell;
                            cell.Occupy(_player);
                            break;
                        case "Door":
                            _cam.DOShakePosition(strength: 0.1f, duration: .2f, randomness: 45f, vibrato: 45, fadeOut: true);
                            _cam.transform.DOLocalMove(Vector3.zero, .1f, false);
                            break;
                        case "Exit":
                            _playerManager.endRound.Invoke();
                            break;
                        default:
                            _cam.DOShakePosition(strength: 0.1f, duration: .2f, randomness: 45f, vibrato: 45, fadeOut: true);
                            _cam.transform.DOLocalMove(Vector3.zero, .1f, false);
                            break;
                    }
                }
                else
                {
                    transform.DOMove(body_position + _centerOffset, 1 / _speed, false);
                    _playerManager.GetDungeonCell(_currentCell).Free();
                    _currentCell = _targetCell;
                    cell.Occupy(_player);
                }
                
            }
            else
            {
                _cam.DOShakePosition(strength: 0.1f, duration: .2f, randomness: 45f, vibrato: 45, fadeOut: true);
                _cam.transform.DOLocalMove(Vector3.zero, .1f, false);
            }
        }
        
        //Not used due to disorientation
        private void HeadBob()
        {
            if (_cam == null) return;
            var camPos = _cam.transform.position;
            
            var sequence = DOTween.Sequence()
                .Append(_cam.transform.DOMoveY(camPos.y - _headBob/4, 1.5f, false).SetEase(Ease.InOutSine))
                .Append(_cam.transform.DOMoveY(camPos.y, 1, false).SetEase(Ease.InOutSine));
            sequence.SetLoops(-1, LoopType.Yoyo);
        }

    }
}
