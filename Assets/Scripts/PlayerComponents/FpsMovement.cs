using System.Collections.Generic;
using DungeonArchitect;
using Scriptable_Objects;
using UnityEngine;

namespace PlayerComponents
{
    public class FpsMovement : MonoBehaviour
    {
        [SerializeField] private PlayerData _playerData;
        [SerializeField] private float _speed;
        [SerializeField] private float _sprintSpeed;
        [SerializeField] private float _turnSpeed;
        [SerializeField] private float _gravity;
        [SerializeField] private float _mouseSensitivity;
        [SerializeField] private float _minCameraview, _maxCameraview;
        private CharacterController _charController;
        private Camera _camera;
        private Vector3 _moveVector;
        private float xRotation = 0f;
        private GameManager manager;
        private List<IntVector> _dungeonCells;

        private void Awake()
        {
            manager = GameManager.GetInstance();
        }

        // Start is called before the first frame update
        void Start()
        {
            _charController = GetComponent<CharacterController>();
            _camera = Camera.main;
            //_dungeonCells = manager.dungeonCells;
            _playerData = manager.GetPlayerData();
            //_speed = _playerData.speed;
            _sprintSpeed = 10f;//_playerData.sprintSpeed;
            //_turnSpeed = _playerData.turnSpeed;
            _gravity = 9.8f;//_playerData.gravity;
            _mouseSensitivity = 0;//_playerData.mouseSensitivity;
            _minCameraview = 0;//_playerData.minCameraview;
            _maxCameraview = 0;//_playerData.maxCameraview;

            if(_charController == null)
                Debug.Log("No Character Controller Attached to Player");

            Cursor.lockState = CursorLockMode.Locked;
        }

        // Update is called once per frame
        void Update()
        {
            Move();
            Look();
        }

        void Move()
        {
            var vertical = Input.GetAxisRaw("Vertical");
            var horizontal = Input.GetAxisRaw("Horizontal");
            var currentSpeed = Input.GetButton("Sprint")?_sprintSpeed:_speed;
        
            _moveVector = transform.forward * vertical + transform.right * horizontal;
            _moveVector = _moveVector.normalized * currentSpeed * Time.deltaTime;
            _moveVector.y += _gravity * Time.deltaTime;
        
            _charController.Move(_moveVector);
        }

        void Look()
        {
            //Get Mouse position Input
            float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;
            //Rotate the camera based on the Y input of the mouse
            xRotation -= mouseY;
            //clamp the camera rotation between 80 and -70 degrees
            xRotation = Mathf.Clamp(xRotation, _minCameraview, _maxCameraview);
            _camera.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            //Rotate the player based on the X input of the mouse
            transform.Rotate(Vector3.up * mouseX * _turnSpeed);
        }
    
    }
}
