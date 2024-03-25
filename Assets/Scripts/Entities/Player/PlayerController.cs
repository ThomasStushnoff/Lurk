using System;
using System.Collections.Generic;
using System.Linq;
using Audio;
using Interfaces;
using Managers;
using Objects;
using StateMachines;
using UI;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;
using World;
using Random = UnityEngine.Random;

// TODO: REFACTOR AND USE FSM
namespace Entities.Player
{
    public class PlayerController : BaseEntity
    {
        [TitleHeader("Player Settings")]
        [SerializeField]
        public PlayerSettings settings;
        [SerializeField] private Transform groundCheck;
        public Transform cameraTransform;
        [SerializeField] private CharacterController character;
        [SerializeField] private HUDController hudController;
        [SerializeField] private Volume postProcessVolume;
        [SerializeField] private List<AudioDataEnumSoundFx> footstepSounds;
        [SerializeField] private AudioDataEnumSoundFx bloodyFloorSound;
        public Transform itemHoldTransform;
        public Transform cameraHoldTransform;

        public Action OnSilhouetteAppear;
        public Action OnRoomLightChange;
        
        private float CurrentStamina { get; set; }
        public float CurrentSanity { get; set; }
        
        private bool _onGround;
        private float _xRotation;
        private bool _isCrouching;
        private bool _isSneaking;
        private bool _isVaulting;
        private Transform _puzzleFocusTarget;
        private bool _isFocusingOnPuzzle;
        private bool _isInCameraMode;
        private bool _isInspecting;
        private float _rotationDirection;
        private GameObject _inspectingObject;
        private Vector3 _objOriginalPosition;
        private Quaternion _objOriginalRotation;
        private DepthOfField _depthOfField;
        private BaseState<IBaseEntity> _currentState;
        private Vector3 _defaultCameraLocalPosition;
        private Vector3 _lastCameraPosition;
        private Quaternion _lastCameraRotation;
        private IInteractable _interactable;

        protected override void Awake()
        {
            base.Awake();
            
            if (GameManager.Instance.localPlayer == null)
                GameManager.Instance.localPlayer = this;
        }
        
        private void Start()
        {
            InputManager.FreeCursor.performed += _ => EnableCursor();
            InputManager.FreeCursor.canceled += _ => DisableCursor();
            
            InputManager.Interact.started += _ => BeginInteractions();
            InputManager.Interact.canceled += _ => EndInteractions();
            
            InputManager.Inspect.started += _ => ToggleInspect();
            InputManager.RotateLeft.performed += _ => StartRotatingLeft();
            InputManager.RotateLeft.canceled += _ => StopRotating();
            InputManager.RotateRight.performed += _ => StartRotatingRight();
            InputManager.RotateRight.canceled += _ => StopRotating();
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            CurrentStamina = settings.maxStamina;
            CurrentSanity = settings.maxSanity;

            _defaultCameraLocalPosition = cameraTransform.localPosition;
        }
        
        private void Update()
        {
            AudioManager.Instance.UpdateAudioSource(transform.position);
            
            HandleCameraMovement();
            
            HandleMovement();
            HandleVault();
            HandleStamina();
            HandleSanity();
            if (_isInspecting) RotateInspectingObject();
            HandlePuzzleInteractions();
            
            CheckGrounded();
            
            postProcessVolume.profile.TryGet<DepthOfField>(out _depthOfField);
        }

        private void LateUpdate()
        {
            if (_isFocusingOnPuzzle) FollowPuzzle();
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (hasFocus)
                DisableCursor();
            else
                EnableCursor();
        }

        public override void ChangeState(BaseState<IBaseEntity> newState)
        {
            _currentState?.ExitState();
            _currentState = newState;
            _currentState.EnterState();
        }

        private void CheckGrounded()
        {
            if (!settings) return;

            _onGround = Physics.CheckSphere(groundCheck.position, 0.1f, settings.ground);
        }
        
        private bool IsFloorBloody()
        {
            // if (Physics.Raycast(groundCheck.position, Vector3.down, out var hit, 0.1f))
            //     return hit.collider.CompareTag("BloodyFloor");
            var colliders = new Collider[12];
            var size = Physics.OverlapSphereNonAlloc(groundCheck.position, 0.1f, colliders, settings.bloodyFloor);
            for (var i = 0; i < size; i++)
            {
                if (colliders[i].CompareTag("BloodyFloor") || colliders[i].gameObject.CompareTag("BloodyFloor"))
                    return true;
            }
            
            return false;
        }

        private void HandleCameraMovement()
        {
            if (!Cursor.visible)
            {
                var look = InputManager.Look.ReadValue<Vector2>();
                var lookX = look.x * settings.mouseSensitivity * Time.deltaTime;
                var lookY = look.y * settings.mouseSensitivity * Time.deltaTime;
                var localPos = cameraTransform.localPosition;
                
                _xRotation -= lookY;
                _xRotation = Mathf.Clamp(_xRotation, -90.0f, 90.0f);
                
                cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
                transform.Rotate(Vector3.up * lookX);
                
                // Head Bobbing.
                if (IsMoving() && !_isInCameraMode && !_isInspecting && !_isFocusingOnPuzzle)
                {
                    var speed = Mathf.PI * 2 * settings.bobFrequency * Time.time;
                    var waveSliceX = Mathf.Cos(speed);
                    var waveSliceY = Mathf.Sin(speed);
                    
                    var bobAmountX = waveSliceX * settings.bobAmountX;
                    var bobAmountY = waveSliceY * settings.bobAmountY;
                    
                    cameraTransform.localPosition = new Vector3(localPos.x + bobAmountX, 
                        localPos.y + bobAmountY, localPos.z);
                }
                else
                {
                    cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, 
                        _defaultCameraLocalPosition, Time.deltaTime * settings.bobSpeed);
                }
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

            var input = InputManager.Move.ReadValue<Vector2>();
            var move = cameraTransform.TransformDirection(new Vector3(input.x, 0, input.y));
            move.y = 0;
            move.Normalize();

            // Crouching.
            if (InputManager.Crouch.WasPressedThisFrame())
            {
                _isCrouching = !_isCrouching;
                // character.height = _isCrouching ? 1 : 2; // To be removed.
                currentSpeed *= _isCrouching ? settings.crouchSpeedMultiplier : 1;
            }

            // Sneaking.
            _isSneaking = InputManager.Sneak.IsPressed();
            if (_isSneaking) currentSpeed *= settings.sneakSpeedMultiplier;

            // Apply speed reduction based on stamina.
            currentSpeed *= Mathf.Lerp(0.5f, 1.0f, (100.0f - CurrentStamina) / 100.0f);
            // Apply speed reduction based on sanity.
            currentSpeed *= Mathf.Lerp(0.5f, 1.0f, (100.0f - CurrentSanity) / 100.0f);

            character.Move(move * (currentSpeed * Time.deltaTime));

            if (IsMoving()) GenerateNoise(_isSneaking);
        }

        private void HandleVault()
        {
            // TODO:
            // 1. Add animation.
            // 2. Optimize.
            if (!InputManager.Vault.WasPressedThisFrame() || !CanVault(out var obstacleHeight) || CurrentStamina <= 0) return;
            
            if (obstacleHeight > 0)
            {
                var vaultHeight = obstacleHeight + 0.5f;

                var newPos = new Vector3(transform.position.x, vaultHeight, transform.position.z).
                    Add(transform.forward * settings.vaultDistance);
                transform.position = newPos;
                CurrentStamina -= settings.vaultStaminaCost;
                // transform.position = Vector3.Lerp(transform.position, newPosition, 0.5f);
            }
        }

        private void HandleStamina()
        {
            if (IsMoving() || _isVaulting)
            {
                // Decrease stamina when moving or vaulting.
                var drainRate = _isSneaking ? settings.sneakStaminaDrainRate : settings.staminaDrainRate;
                CurrentStamina -= Time.deltaTime * drainRate;
            }
            else if (_isCrouching || !IsMoving())
            {
                // Regenerate stamina when crouching or standing still.
                CurrentStamina += Time.deltaTime * settings.crouchStaminaRegenRate;
            }
            else
            {
                // Regenerate stamina when not moving.
                CurrentStamina += Time.deltaTime * settings.staminaRegenRate;
            }

            CurrentStamina = Mathf.Clamp(CurrentStamina, 0, settings.maxStamina);
            hudController.UpdateStamina(CurrentStamina / settings.maxStamina);
        }

        private void HandleSanity()
        {
            // Player is low on stamina.
            if (CurrentStamina <= settings.staminaThreshold)
                CurrentSanity -= Time.deltaTime * settings.sanityDrainRate;
            // Player is near an enemy.
            if (IsEnemyNearby())
                CurrentSanity -= Time.deltaTime * settings.sanityDrainRate;
            
            CurrentSanity = Mathf.Clamp(CurrentSanity, 0, settings.maxSanity);
            hudController.UpdateSanity(CurrentSanity / settings.maxSanity);

            // Trigger Insanity.
            if (CurrentSanity <= 0)
            {
                // Player loses control over character.
                InputManager.DisableMovementInput();
                // Player dies with a jump scare.
                // Trigger Jump Scare.
                // AudioManager.Instance.PlayOneShotAudio();
                // Restart the level.
            }
        }

        public void UpdateSanity(float value) => CurrentSanity += value;

        public static void EnableCursor()
        {
            // var cursorFree = InputManager.FreeCursor.IsPressed();
            // Cursor.lockState = cursorFree ? CursorLockMode.None : CursorLockMode.Locked;
            // Cursor.visible = cursorFree;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        
        public static void DisableCursor()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        
        private void GenerateNoise(bool isSneaking)
        {
            // TODO:
            // 1. Adjust the values later.
            // 2. Add SFX.
            var volume = isSneaking ? settings.sneakVolume : 1.0f;
            var pitch = isSneaking ? settings.sneakPitch : 1.0f;
            
            // Randomize the footstep sound.
            var randomIndex = Random.Range(0, footstepSounds.Count);
            var footstepSound = IsFloorBloody() ? bloodyFloorSound : footstepSounds.ElementAt(randomIndex);
            if (!audioSource.isPlaying)
            {
                audioSource.PlaySoundFx(footstepSound);
                audioSource.volume = volume;
                audioSource.pitch = pitch;
            } 
            
            // Play footstep SFX and generate noise.
            
            // Ideas:
            // Since the enemy will be patrolling, the noise will attract the enemy.
            // If certain noise level is reached, the enemy will be notified.
            // So, we create a noise level and notify the enemy.
            // Logic:
            // 1. Enemy will have an overlap circle.
            // 2. Expose a noise level variable for the player.
            // 3. If the player is within the circle, and the noise level is above a certain threshold, the enemy will be notified.
            // 4. The enemy will then move towards the player and do the deed.
        }
        
        

        private void BeginInteractions()
        {
            // TODO:
            // 1. Add cooldown maybe.
            // 2. UI feedback.
            // 3. SFX.
            if (!Physics.Raycast(cameraTransform.position, cameraTransform.forward, out var hit, 
                    settings.interactDistance, settings.interactable)) return;
            Debug.Log($"hit.collider.name: {hit.collider.name}!");
            
            // Check if the object is interactable.
            if (hit.collider.TryGetComponent<IInteractable>(out var interactable))
            {
                _interactable = interactable;
                interactable.BeginInteract(this);
                Debug.Log($"Interacting with {hit.collider.name}!");
            }
            
            // Check if the object is collectible.
            if (hit.collider.TryGetComponent<ICollectible>(out var collectible))
            {
                collectible.Collect();
                Debug.Log($"Collected {hit.collider.name}!");
            }
        }

        private void EndInteractions()
        {
            if (_interactable == null) return;
            
            _interactable.EndInteract();
            _interactable = null;
        }

        private void HandlePuzzleInteractions()
        {
            // TODO:
            // Use Hold.
            // Add a delay when interacting (Holding the button).
            if (!InputManager.InteractPuzzle.WasPerformedThisFrame()) return;
            
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out var hit, 
                settings.interactDistance, settings.puzzle))
            {
                var puzzle = hit.collider.GetComponent<BasePuzzle>();
                if (puzzle != null)
                {
                    puzzle.BeginInteract(this);
                }
            }
        }
        
        public void FocusOnPuzzle(Transform puzzleTransform)
        {
            _lastCameraPosition = cameraTransform.localPosition;
            _lastCameraRotation = cameraTransform.localRotation;
            // Debug.Log($"lastCameraPosition: {_lastCameraPosition}");
            // Debug.Log($"lastCameraRotation: {_lastCameraRotation}");
            _puzzleFocusTarget = puzzleTransform;
            _isFocusingOnPuzzle = true;
        }
        
        public void StopFocusingOnPuzzle()
        {
            _puzzleFocusTarget = null;
            _isFocusingOnPuzzle = false; 
            cameraTransform.SetLocalPositionAndRotation(_lastCameraPosition, _lastCameraRotation);
        }

        private void FollowPuzzle()
        {
            if (_puzzleFocusTarget == null) return;
            
            // TODO
            // Serialize the values maybe?
            var offset = new Vector3(0, 2, -2);
            var targetPosition = _puzzleFocusTarget.position + _puzzleFocusTarget.TransformDirection(offset);
            var smoothTime = 0.5f;
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, targetPosition, smoothTime);
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
            if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out var hit, 
                    settings.inspectDistance, settings.inspectable))
            {
                Debug.Log($"Inspecting {hit.collider.name}!");
                if (hit.collider.GetComponent<IInspectable>() != null)
                    StartInspecting(hit.collider.gameObject);
            }
            else
            {
                Debug.Log("Stopped inspecting!");
                StopInspecting();
            }
        }
        
        private void StartInspecting(GameObject obj) 
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

        private void StopInspecting() 
        {
            if (_inspectingObject == null) return;
            _inspectingObject.transform.position = _objOriginalPosition;
            _inspectingObject.transform.rotation = _objOriginalRotation;
            
            if (_depthOfField) _depthOfField.active = false;
            
            _isInspecting = false;
            _inspectingObject = null;
        }
        
        private bool IsMoving() => InputManager.Move.ReadValue<Vector2>().magnitude > 0;
        
        private bool IsInPanicState() => CurrentSanity <= settings.panicThreshold || CurrentStamina <= settings.staminaThreshold;
        
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

        private bool IsEnemyNearby()
        {
            var col = Physics.OverlapSphere(transform.position, settings.detectionRadius, settings.enemy);
            if (col.Length > 0)
            {
                // Play Heart Beat SFX.
                return true;
            }
            
            return false;
        }
        
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(cameraTransform.position, cameraTransform.position + cameraTransform.forward * settings.interactDistance);
        }
    }
}