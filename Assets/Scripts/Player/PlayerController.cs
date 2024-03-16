using Interfaces;
using Managers;
using Objects;
using UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerController : MonoBehaviour, IEntity
    {
        [TitleHeader("Player Settings")]
        [SerializeField] private PlayerSettings settings;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private CharacterController character;
        [FormerlySerializedAs("uiController")]
        [SerializeField] private HUDController hudController;
        [SerializeField] Volume postProcessVolume;
        
        // [TitleHeader("Network Settings")]
        // public bool isHost;

        private bool _onGround;
        private float _xRotation;
        private Actions _actions;
        private Vector3 _velocity;
        private float _currentStamina;
        private float _currentSanity;
        private bool _isCrouching;
        private bool _isSneaking;
        private bool _isVaulting;
        private bool _isInspecting;
        private float _rotationDirection;
        private GameObject _inspectingObject = null;
        private Vector3 _objOriginalPosition;
        private Quaternion _objOriginalRotation;
        private DepthOfField _depthOfField;

        private void Awake()
        {
            if (GameManager.Instance.localPlayer == null)
                GameManager.Instance.localPlayer = this;

            _actions = new Actions();
        }

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            _currentStamina = settings.maxStamina;
            _currentSanity = settings.maxSanity;
        }

        private void Update()
        {
            AudioManager.Instance.UpdateAudioSource(transform.position);
            
            HandleCameraMovement();
            // TODO:
            // 1. Use FSM later.
            // 2. Network later.
            HandleMovement();
            HandleVault();
            HandleStamina();
            HandleSanity();
            if (_isInspecting) RotateInspectingObject();

            // Removed since no jumping.
            // HandleVelocity();
            HandleCursor();
            CheckGrounded();
            
            postProcessVolume.profile.TryGet<DepthOfField>(out _depthOfField);
        }

        private void OnEnable()
        {
            _actions.Player.FreeCursor.performed += _ => HandleCursor();
            _actions.Player.Interact.started += _ => HandleInteractions();
            
            _actions.Player.Inspect.performed += _ => ToggleInspect();
            _actions.Player.RotateLeft.performed += _ => StartRotatingLeft();
            _actions.Player.RotateLeft.canceled += _ => StopRotating();
            _actions.Player.RotateRight.performed += _ => StartRotatingRight();
            _actions.Player.RotateRight.canceled += _ => StopRotating();
            
            _actions?.Enable();
        }

        private void OnDisable()
        {
            _actions.Player.FreeCursor.performed -= _ => HandleCursor();
            _actions.Player.Interact.started -= _ => HandleInteractions();
            
            _actions.Player.Inspect.performed -= _ => ToggleInspect();
            _actions.Player.RotateLeft.performed -= _ => StartRotatingLeft();
            _actions.Player.RotateLeft.canceled -= _ => StopRotating();
            _actions.Player.RotateRight.performed -= _ => StartRotatingRight();
            _actions.Player.RotateRight.canceled -= _ => StopRotating();
            
            _actions?.Disable();
        }

        private void CheckGrounded()
        {
            if (!settings) return;

            _onGround = Physics.CheckSphere(groundCheck.position, 0.1f, settings.ground);
        }

        private void HandleCameraMovement()
        {
            if (!Cursor.visible)
            {
                var look = _actions.Player.Look.ReadValue<Vector2>();
                var lookX = look.x * settings.mouseSensitivity * Time.deltaTime;
                var lookY = look.y * settings.mouseSensitivity * Time.deltaTime;

                _xRotation -= lookY;
                _xRotation = Mathf.Clamp(_xRotation, -90.0f, 90.0f);

                cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
                transform.Rotate(Vector3.up * lookX);
            }
        }

        private void HandleMovement()
        {
            var currentSpeed = settings.movementSpeed;

            // Movement is slowed or disabled when in panic state.
            if (IsInPanicState())
            {
                // TODO:
                // 1. Figure out whether to disable movement or slow it down.
                currentSpeed /= 2f;
            }

            var input = _actions.Player.Move.ReadValue<Vector2>();
            var move = cameraTransform.TransformDirection(new Vector3(input.x, 0, input.y));
            move.y = 0;
            move.Normalize();

            // Crouching.
            if (_actions.Player.Crouch.WasPressedThisFrame())
            {
                _isCrouching = !_isCrouching;
                character.height = _isCrouching ? 1 : 2;// To be removed.
                currentSpeed *= _isCrouching ? settings.crouchSpeedMultiplier : 1;
            }

            // Sneaking.
            _isSneaking = _actions.Player.Sneak.IsPressed();
            if (_isSneaking) currentSpeed *= settings.sneakSpeedMultiplier;

            currentSpeed *= Mathf.Lerp(0.5f, 1.0f, (100.0f - _currentStamina) / 100.0f);

            character.Move(move * (currentSpeed * Time.deltaTime));

            if (IsMoving()) GenerateNoise(_isSneaking);
        }

        private void HandleVault()
        {
            // TODO:
            // 1. Add animation.
            // 2. Optimize.
            if (!_actions.Player.Vault.WasPressedThisFrame() || !CanVault(out var obstacleHeight)) return;
            
            if (obstacleHeight > 0)
            {
                var vaultHeight = obstacleHeight + 0.5f;

                var newPosition = new Vector3(transform.position.x, vaultHeight, transform.position.z) +
                                  transform.forward * settings.vaultDistance;
                transform.position = newPosition;
                // transform.position = Vector3.Lerp(transform.position, newPosition, 0.5f);
            }
        }

        private void HandleStamina()
        {
            switch (IsMoving())
            {
                case true:
                    // Reduce stamina when walking.
                    _currentStamina -= Time.deltaTime * settings.staminaDrainRate;
                    break;
                case false when IsInPanicState():
                    // Regenerate stamina when not moving and in panic state.
                    _currentStamina += Time.deltaTime * settings.staminaRegenRate;
                    break;
                case false when !IsInPanicState():
                    // Regenerate stamina when not moving and not in panic state.
                    _currentStamina += Time.deltaTime * settings.staminaRegenRate * 2;
                    break;
            }

            _currentStamina = Mathf.Clamp(_currentStamina, 0, settings.maxStamina);
            hudController.UpdateStamina(_currentStamina / settings.maxStamina);
        }

        private void HandleSanity()
        {
            _currentSanity = Mathf.Clamp(_currentSanity, 0, settings.maxSanity);
            hudController.UpdateSanity(_currentSanity / settings.maxSanity);
        }

        public void UpdateSanity(float value) => _currentSanity += value;

        private void HandleCursor()
        {
            var cursorFree = _actions.Player.FreeCursor.IsPressed();
            Cursor.lockState = cursorFree ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = cursorFree;
        }

        private void GenerateNoise(bool isSneaking)
        {
            // TODO:
            // 1. Adjust values later.
            // 2. AudioMixers.
            
            var noiseLevel = Mathf.Lerp(0.1f, 1.0f, (100.0f - _currentStamina) / 100.0f);
            if (isSneaking) noiseLevel /= 2;
        }

        private void HandleInteractions()
        {
            // TODO:
            // 1. Add cooldown maybe.
            // 2. UI feedback.
            // 3. SFX.
            var ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (!Physics.Raycast(ray, out var hit, settings.interactDistance)) return;
            
            var interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable == null) return;
            
            interactable.Interact();
            Debug.Log($"Interacted with {hit.collider.name}!");

            // Check if the object is collectible.
            var collectible = hit.collider.GetComponent<ICollectible>();
            collectible?.Collect();
            Debug.Log($"Collected {hit.collider.name}!");
        }

        private void StartRotatingLeft() => _rotationDirection = -1.0f;

        private void StartRotatingRight() => _rotationDirection = 1.0f;

        private void StopRotating() => _rotationDirection = 0.0f;

        private void RotateInspectingObject() 
        {
            if (_rotationDirection != 0 && _isInspecting)
            {
                var rotationSpeed = settings.inspectRotationSpeed;
                _inspectingObject.transform.Rotate(Vector3.up, rotationSpeed * _rotationDirection * Time.deltaTime, 
                    Space.World);
            }
        }

        private void ToggleInspect()
        {
            if (_isInspecting)
                StartInspecting(_inspectingObject);
            else
                StopInspecting();
        }
        
        void StartInspecting(GameObject obj) 
        {
            _inspectingObject = obj;
            _objOriginalPosition = obj.transform.position;
            _objOriginalRotation = obj.transform.rotation;
            
            _inspectingObject.transform.position = cameraTransform.position + cameraTransform.forward * settings.inspectDistance;
            _inspectingObject.transform.rotation = Quaternion.identity;

            if (_depthOfField)
            {
                _depthOfField.active = true;
                // _depthOfField.focusDistance.SetValue();
            }
            
            _isInspecting = true;
        }

        void StopInspecting() 
        {
            _inspectingObject.transform.position = _objOriginalPosition;
            _inspectingObject.transform.rotation = _objOriginalRotation;
            
            if (_depthOfField) _depthOfField.active = false;
            
            _isInspecting = false;
            _inspectingObject = null;
        }
        
        private bool IsMoving() => _actions.Player.Move.ReadValue<Vector2>().magnitude > 0;
        
        private bool IsInPanicState() => _currentSanity <= settings.panicThreshold || _currentStamina <= settings.staminaThreshold;
        
        private bool CanVault(out float obstacleHeight)
        {
            obstacleHeight = 0;

            // Define the start position of the ray.
            var rayStart = groundCheck.position;
            // Define the direction of the ray.
            var rayDirection = transform.forward;
            
            if (Physics.Raycast(rayStart, rayDirection, out var hit, settings.vaultDistance, settings.obstacle))
            {
                // Calculate the height of the obstacle.
                obstacleHeight = hit.collider.bounds.max.y;
                var heightDifference = obstacleHeight - transform.position.y;

                // Check if the obstacle is shorter than the character.
                if (heightDifference < character.height / 2.0f)
                {
                    Debug.DrawRay(rayStart, rayDirection * hit.distance, Color.green, 2f);
                    return true;
                }
                else
                {
                    Debug.DrawRay(rayStart, rayDirection * settings.vaultDistance, Color.red, 2f);
                    return false;
                }
            }
            
            Debug.DrawRay(rayStart, rayDirection * settings.vaultDistance, Color.blue, 2f);
            return true;
        }
    }
}