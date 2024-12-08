    using UnityEngine;

    public class PlayerController : MonoBehaviour{
        [Header("Speed")]
        public float moveSpeed;
        public float fastMoveSpeed;
        public float acceleration;

        
        [Space(20)]
        [Header("Zoom")]
        public float zoomSpeed;
        public float defaultCameraHeight;
        public float mapViewHeight;
        public float macroViewHeight; 
        public float cameraSmoothing = 0.3f;
        
        
        // General settings
        private Transform _spawnPoint;
        private ViewMode _viewMode;
        private PlayerControls _playerControls;
        
        // Speed controls 
        private float _currentSpeed;
        private Vector3 _targetPosition;
        private Vector3 _velocity;

        private Terrain _terrain;
        
        //Height Settings
        private float _currentHeight;
        private float _targetHeight;
        
        // Control values
        private Vector2 _moveInputVector;
        private bool _isAccelerating;
        private Vector2 _cameraZoomInputVector;
        
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
            
            // Initialize minimum height to be 80% of cameraHeight
            _currentHeight = defaultCameraHeight;
            _targetHeight = _currentHeight;
            
            // Initialize spawn point
            _spawnPoint = GameObject.FindWithTag("Respawn").GetComponent<Transform>();
            transform.position = new Vector3(_spawnPoint.transform.position.x, _targetHeight, _spawnPoint.transform.position.z);
            _targetPosition = transform.position;
        }

        // Update is called once per frame
        void Update() {
            GetInput();
            if (_moveInputVector.x != 0 || _moveInputVector.y != 0) {
                CalculateMovePosition();
            }
        
            // Zoom only works on up / down scroll wheel direction
            if (_cameraZoomInputVector.y != 0) {
                ZoomCamera();
            }
            Accelerate();
        }

        private void FixedUpdate() {
            // CalculateCameraHeight();
        }

        private void LateUpdate() {
            //TODO: Check if _targetPosition is close instead of equal
            transform.position = Vector3.SmoothDamp(transform.position, _targetPosition, ref _velocity, cameraSmoothing);
        }

        private void OnDisable() {
            _playerControls.Gameplay.Disable();
        }

        private void GetInput() {
            _moveInputVector = _playerControls.Gameplay.Move.ReadValue<Vector2>();
            _isAccelerating = _playerControls.Gameplay.FastMove.ReadValue<float>() > 0;
            // Normalize camera input vector to only capture direction and not scroll speed
            _cameraZoomInputVector= _playerControls.Gameplay.Zoom.ReadValue<Vector2>().normalized;
        }

        private void CalculateMovePosition() {
            // Calculate the target position based on the input
            _targetPosition += new Vector3(_moveInputVector.x, 0, _moveInputVector.y).normalized * _currentSpeed * Time.deltaTime;

            // Get the bounds of the terrain
            Bounds terrainBounds = _terrain.terrainData.bounds;

            // Transform the bounds to world space
            Vector3 terrainCenter = _terrain.transform.position + terrainBounds.center;
            Vector3 terrainSize = terrainBounds.size;

            // Clamp Player movement to be within the terrain bounds
            ClampMovement(terrainSize, terrainCenter);
        }

        private void ClampMovement(Vector3 terrainSize, Vector3 terrainCenter) {
            // Calculate the bounding box from terrain center and dimensions
            Vector3 start = terrainCenter - (terrainSize / 2);
            Vector3 end = terrainCenter + (terrainSize / 2);
            
            // Clamp target position to the start and end of the bounding box
            _targetPosition.x = Mathf.Clamp(_targetPosition.x, start.x, end.x);
            _targetPosition.z = Mathf.Clamp(_targetPosition.z, start.z, end.z);
        }
        
        
        private void Accelerate() {
            float targetSpeed = _isAccelerating ? fastMoveSpeed : moveSpeed;

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

        private void ZoomCamera() {
            // Zoom in
            // Calculate the target position based on the input
            _targetHeight += _cameraZoomInputVector.y * zoomSpeed * Time.deltaTime;
            
            // Clamp Height 
            _targetHeight = Mathf.Clamp(_targetHeight, macroViewHeight, mapViewHeight);
            
            // Update the camera target position
            _targetPosition.y = _targetHeight;
        }
        
    }