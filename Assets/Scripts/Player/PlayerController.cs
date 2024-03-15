using Interfaces;
using Managers;
using Objects;
using UI;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [TitleHeader("Player Settings")]
        [SerializeField] private PlayerSettings settings;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private CharacterController character;
        [SerializeField] private UIController uiController;
        
        // [TitleHeader("Network Settings")]
        // public bool isHost;

        private bool _onGround;
        private float _xRotation;
        private Actions _actions;
        private Vector3 _velocity;
        private float _currentStamina;
        private float _currentSanity;
        private void Awake()
        {
            if (GameManager.Instance.localPlayer == null)
                GameManager.Instance.localPlayer = this;

            _actions = new Actions();
            _actions.Player.FreeCursor.performed += _ => HandleCursor();
            _actions.Player.Interact.started += _ => HandleInteractions();
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
            // TODO:
            // 1. Use FSM later.
            // 2. Network later.
            HandleMovement();
            HandleStamina();
            HandleSanity();
            
            // Removed since no jumping.
            // HandleVelocity();
            HandleCursor();
            CheckGrounded();
        }

        private void OnEnable() => _actions?.Enable();

        private void OnDisable() => _actions?.Disable();

        private void CheckGrounded()
        {
            if (!settings) return;

            _onGround = Physics.CheckSphere(groundCheck.position, 0.1f, settings.ground);
        }

        private void HandleMovement()
        {
            #region Moving

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

            // Modify movement speed.
            var isSlowWalking = _actions.Player.SlowWalk.IsPressed();
            if (isSlowWalking) currentSpeed /= settings.slowWalkMultiplier;
            
            currentSpeed *= Mathf.Lerp(0.5f, 1.0f, (100.0f - _currentStamina) / 100.0f);

            character.Move(move * (currentSpeed * Time.deltaTime));

            if (IsMoving()) GenerateNoise(isSlowWalking);

            #endregion

            #region Looking

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

            #endregion
        }

        private void HandleStamina()
        {
            switch (IsMoving())
            {
                case true:
                    // Reduce stamina when walking.
                    _currentSanity -= Time.deltaTime * settings.staminaDrainRate;
                    break;
                case false when IsInPanicState():
                    // Regenerate stamina when not moving and in panic state.
                    _currentSanity += Time.deltaTime * settings.staminaRegenRate;
                    break;
                case false when !IsInPanicState():
                    // Regenerate stamina when not moving and not in panic state.
                    _currentSanity += Time.deltaTime * settings.staminaRegenRate * 2;
                    break;
            }
            
            _currentSanity = Mathf.Clamp(_currentSanity, 0, settings.maxStamina);
            uiController.UpdateStamina(_currentSanity / settings.maxStamina);
        }

        private void HandleSanity()
        {
            _currentStamina = Mathf.Clamp(_currentStamina, 0, settings.maxSanity);
            uiController.UpdateSanity(_currentStamina / settings.maxSanity);
        }

        public void UpdateSanity(float value) => _currentStamina += value;
        
        private void HandleVelocity()
        {
            if (_onGround && _velocity.y < 0)
                _velocity.y = -2.0f;

            // Handle jumping.
            if (_onGround && _actions.Player.Jump.WasPressedThisFrame())
                _velocity.y = Mathf.Sqrt(settings.jumpHeight * -2.0f * settings.gravity);

            _velocity.y += settings.gravity * Time.deltaTime;
            character.Move(_velocity * Time.deltaTime);
        }
        
        private void HandleCursor()
        {
            var cursorFree = _actions.Player.FreeCursor.IsPressed();
            Cursor.lockState = cursorFree ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = cursorFree;
        }

        private void GenerateNoise(bool isSlowWalking)
        {
            // TODO:
            // 1. Adjust values later.
            // 2. AudioMixers.
            
            var noiseLevel = Mathf.Lerp(0.1f, 1.0f, (100.0f - _currentStamina) / 100.0f);
            if (isSlowWalking) noiseLevel /= 2;
        }

        private void HandleInteractions()
        {
            // TODO:
            // 1. Add cooldown maybe.
            // 2. UI feedback.
            // 3. Animation.
            var ray = new Ray(cameraTransform.position, cameraTransform.forward);
            if (!Physics.Raycast(ray, out var hit, settings.interactDistance)) return;
            
            var interactable = hit.collider.GetComponent<IInteractable>();
            interactable?.Interact();
            Debug.Log($"Interacted with {hit.collider.name}!");
        }
        
        private bool IsMoving() => _actions.Player.Move.ReadValue<Vector2>().magnitude > 0;
        
        private bool IsInPanicState() => _currentStamina <= settings.panicThreshold || _currentSanity <= settings.staminaThreshold;
    }
}