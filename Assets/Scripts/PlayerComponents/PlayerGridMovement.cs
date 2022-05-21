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
        private Vector3 _centerOffset;
        private Vector3Int _targetCell;
        private Vector3Int _currentCell;
        private GameManager.Direction _currentDirection;

        private const float _speed = 4f;
        private const float _turnSpeed = 4f;
        private const float _headBob = 0.1f;
        
        private void Awake()
        {
        }

        // Start is called before the first frame update
        void Start()
        {
            _playerManager = GameManager.GetInstance();
            _playerUIManager = UIManager.GetInstance();
            _cam = Camera.main;
            _playerManager.initializeMovementGrid.AddListener(InitializeMovement);
            //InitializeMovement();
        }
        private void InitializeMovement()
        {
            //Debug.Log("Initializing grid");

            _player = _playerManager.GetPlayer();
            _currentCell = _player._initialCell;
            _targetCell = _currentCell;
            _currentDirection = _player._currentDirection;
            _centerOffset = new Vector3(.5f, 1f, .5f);
            
            //Cursor.lockState = CursorLockMode.Locked;
            UpdatePlayerPosition();
            //HeadBob();
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
            if (Input.GetButton("Up") && !_isMoving)
            {
                _isMoving = true;
                StartCoroutine(MovePlayerToCell());
            }
            else if (Input.GetButtonDown("Down") && !_isMoving)
            {
                var direction = ((int)_currentDirection + 2) % 4;
                _currentDirection = (GameManager.Direction) direction;
                _player._currentDirection = _currentDirection;
                //Debug.Log("Direction: " + _currentDirection);
                
                _isMoving = true;
                StartCoroutine(RotatePlayer(Vector3.down * 180));
            }
            else if (Input.GetButtonDown("Left") && !_isMoving)
            {
                var direction = ((int) _currentDirection + 3) % 4;

                _currentDirection = (GameManager.Direction) direction;
                _player._currentDirection = _currentDirection;
                //Debug.Log("Direction: " + _currentDirection);
                _isMoving = true;
                StartCoroutine(RotatePlayer(Vector3.down * 90));
            }
            else if (Input.GetButtonDown("Right") && !_isMoving)
            {
                var direction = ((int)_currentDirection + 1)%4;
                
                _currentDirection = (GameManager.Direction) direction;
                _player._currentDirection = _currentDirection;
                //Debug.Log("Direction: " + _currentDirection);
                _isMoving = true;
                StartCoroutine(RotatePlayer(Vector3.up * 90));
            }
            else if (Input.GetButton("Fire1") && !_isMoving)
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
                //_uiManager.InvokePlayerAttack();
                StartCoroutine(PlayerAttack(attackTargetCell));
            }
            else if (Input.GetButtonDown("Fire2"))
            {
                
            }
        }
    
        private IEnumerator PlayerAttack(Vector3Int attackTargetCell)
        {
            if (_isMoving)
            {
                //_playerManager.playerAttack.Invoke(attackTargetCell, _player.weapon);
                _player.ActiveWeapon?.Attack(attackTargetCell);
                _playerUIManager.LogAction.Invoke("Attacked cell " + attackTargetCell.x + ", " + attackTargetCell.y);
                yield return new WaitForSeconds(_playerManager._turnCD);
                _playerManager.UpdateTurn();
                _isMoving = false;
            }
        }
    
        private IEnumerator MovePlayerToCell()
        {
            _targetCell = _currentDirection switch
            {
                GameManager.Direction.North => _currentCell + Vector3Int.up,
                GameManager.Direction.East => _currentCell + Vector3Int.right,
                GameManager.Direction.South => _currentCell + Vector3Int.down,
                GameManager.Direction.West => _currentCell + Vector3Int.left,
                _ => _currentCell
            };
            if (!_isMoving) yield break;
            var cell = _playerManager.GetDungeonCell(_targetCell);
            StartCoroutine(CheckCell(cell));
            UpdatePlayerPosition();
            _playerUIManager.LogAction.Invoke("Moved to cell " + _currentCell.x + ", " + _currentCell.y);
            yield return new WaitForSeconds(_playerManager._turnCD);
            _playerManager.UpdateTurn();
            _isMoving = false;
        }
        private IEnumerator CheckCell(GridCell cell)
        {
            if (cell != null)
            {
                var body_position = new Vector3Int(_targetCell.x, _targetCell.z, _targetCell.y) ;
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
                            //Debug.Log("Picked up item!");
                            //_playerManager.playerPickUp.Invoke(cell.occupant.GetComponent<WeaponData>());
                            _playerManager.pickUpItem.Invoke(cell);
                            transform.DOMove(body_position + _centerOffset, 1 / _speed, false);
                            _playerManager.GetDungeonCell(_currentCell).Free();
                            _currentCell = _targetCell;
                            cell.Occupy(_player);
                            break;
                        case "Weapon":
                            //Debug.Log("Picked up weapon!");
                            _playerManager.pickUpItem.Invoke(cell);
                            transform.DOMove(body_position + _centerOffset, 1 / _speed, false);
                            _playerManager.GetDungeonCell(_currentCell).Free();
                            _currentCell = _targetCell;
                            cell.Occupy(_player);
                            break;
                        case "Exit":
                            //Debug.Log("Exit");
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
                //Debug.Log("Null cell: " + _targetCell);
                _cam.DOShakePosition(strength: 0.1f, duration: .2f, randomness: 45f, vibrato: 45, fadeOut: true);
                _cam.transform.DOLocalMove(Vector3.zero, .1f, false);
            }
            yield break;
        }

        private void HeadBob()
        {
            if (_cam == null) return;
            var camPos = _cam.transform.position;
            
            var sequence = DOTween.Sequence()
                .Append(_cam.transform.DOMoveY(camPos.y - _headBob/4, 1.5f, false).SetEase(Ease.InOutSine))
                .Append(_cam.transform.DOMoveY(camPos.y, 1, false).SetEase(Ease.InOutSine));
            sequence.SetLoops(-1, LoopType.Yoyo);
        }

        private IEnumerator RotatePlayer(Vector3 rotation)
        {
            if (_isMoving)
            {
                transform.DORotate(transform.eulerAngles + rotation, 1 / _turnSpeed, RotateMode.Fast);
                yield return new WaitForSeconds(.2f);
                _isMoving = false;
            }
        }
    }
}
