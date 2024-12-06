    using System;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class PlayerController : MonoBehaviour{
        [Header("Speed")]
        public float moveSpeed;
        public float fastMoveSpeed;
        public float acceleration;
        [Space(20)]
        [Header("Camera Height")]
        public float cameraHeight;
        public float maximumHeight;
        public float cameraSmoothing = 0.3f;

        [Header("Zoom")]
        
        // General settings
        private Transform _spawnPoint;
        private ViewMode _viewMode;
        private PlayerControls _playerControls;
        
        // Speed controls 
        private float _currentSpeed;
        private bool _isAccelerating = false;
        private Vector3 _targetPosition;
        private Vector3 _velocity;

        private Terrain _terrain;
        
        //Height Settings
        private float _currentHeight;
        private float _minimumHeight;
        
        private void OnEnable() {
            if (_playerControls == null) {
                _playerControls = new PlayerControls();
            }
            _playerControls.Gameplay.Enable();
        }
        
        void Start() {
            // Initially the view mode should be default
            _viewMode = ViewMode.DEFAULT;
            
            // Initialize camera speed to moveSpeed
            _currentSpeed = moveSpeed;
            
            // Get terrain to extract bounding box
            _terrain = GameObject.FindGameObjectWithTag("Terrain").GetComponent<Terrain>();
            
            // Initilize minimum height to be 80% of cameraHeight
            _minimumHeight = cameraHeight * 0.8f;
            _currentHeight = cameraHeight;
            
            // Initialize spawn point
            _spawnPoint = GameObject.FindWithTag("Respawn").GetComponent<Transform>();
            transform.position = new Vector3(_spawnPoint.transform.position.x, _currentHeight, _spawnPoint.transform.position.z);
            _targetPosition = transform.position;
        }

        // Update is called once per frame
        void Update() {
            CalculateMovePosition();

            float targetSpeed = _isAccelerating ? fastMoveSpeed : moveSpeed;
            Accelerate(targetSpeed);
        }

        private void FixedUpdate() {
            CalculateCameraHeight();
        }

        private void LateUpdate() {
            //TODO: Check if _targetPosition is close instead of equal
            transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, cameraSmoothing);
        }

        private void OnDisable() {
            _playerControls.Gameplay.Disable();
        }

        private void CalculateMovePosition() {
            Vector2 moveVector = _playerControls.Gameplay.Move.ReadValue<Vector2>();
            // Calculate the target position based on the input
            _targetPosition += new Vector3(moveVector.x, 0, moveVector.y).normalized * _currentSpeed * Time.deltaTime;
            _targetPosition.y = _currentHeight;
            
            // Get the bounds of the terrain
            Bounds terrainBounds = _terrain.terrainData.bounds;

            // Transform the bounds to world space
            Vector3 terrainCenter= _terrain.transform.position + terrainBounds.center;
            Vector3 terrainSize= terrainBounds.size;
            
            // Clamp Player movement to be within the terrain bounds
            ClampMovement(terrainSize, terrainCenter);
                        
            _isAccelerating = _playerControls.Gameplay.FastMove.ReadValue<float>() > 0;
        }

        private void ClampMovement(Vector3 terrainSize, Vector3 terrainCenter) {
            Vector3 start = terrainCenter - (terrainSize / 2);
            Vector3 end = terrainCenter + (terrainSize / 2);
            Debug.Log(terrainCenter + " " + terrainSize + start + " " + end + " " + _targetPosition);
            if (_targetPosition.x < start.x) {
                _targetPosition.x = start.x;
            }

            if (_targetPosition.z < start.z) {
                _targetPosition.z = start.z;
            }

            if (_targetPosition.x > end.x) {
                _targetPosition.x = end.x;
            }

            if (_targetPosition.z > end.z) {
                _targetPosition.z = end.z;
            }
        }
        
        
        private void Accelerate(float targetSpeed) {
            // Either accelerate or decelerate to the target speed
            if (targetSpeed > _currentSpeed) {
                _currentSpeed += acceleration * Time.deltaTime;
                // Cap current speed to target speed
                if (_currentSpeed > targetSpeed) {
                    _currentSpeed = targetSpeed;
                }
            }
            else {
                _currentSpeed -= acceleration * Time.deltaTime;
                if (_currentSpeed < targetSpeed) {
                    _currentSpeed = targetSpeed;
                }
            }
        }

        private void CalculateCameraHeight() {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, cameraHeight)) {
                if (hit.distance < _minimumHeight) {
                    _currentHeight += _minimumHeight - hit.distance;
                }
            }
            else {
                _currentHeight = cameraHeight;
            }
        }
        
    }