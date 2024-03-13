using System;
using Interfaces;
using Objects;
using UnityEngine;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Player Settings")]
        [SerializeField] private PlayerSettings settings;
        [SerializeField] private Transform groundCheck;
        [SerializeField] private Transform cameraTransform;
        [SerializeField] private CharacterController character;
        
        // [Header("Network Settings")]
        // public bool isHost;
        
        private bool _onGround;
        private float _xRotation;
        private Actions _actions;
        private Vector3 _velocity;

        private void Awake()
        {
            _actions = new Actions();
            _actions.Player.FreeCursor.performed += _ => HandleCursor();
            _actions.Player.Interact.started += _ => HandleInteractions();
        }
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            // TODO:
            // 1. Use FSM later.
            // 2. Network later.
            HandleMovement();
            HandleVelocity();
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

            var input = _actions.Player.Move.ReadValue<Vector2>();
            var move = cameraTransform.TransformDirection(new Vector3(input.x, 0, input.y));
            move.y = 0;
            move.Normalize();

            // Modify movement speed.
            var isSlowWalking = _actions.Player.SlowWalk.IsPressed();
            var modifiedSpeed = isSlowWalking ? settings.movementSpeed / settings.slowWalkMultiplier : settings.movementSpeed;
            modifiedSpeed *= Mathf.Lerp(1.0f, 1.5f, (100f - settings.sanity) / 100f);

            character.Move(move * (modifiedSpeed * Time.deltaTime));

            cameraTransform.localPosition = new Vector3(0, 0.3f, 0);

            #endregion

            #region Looking

            if (!Cursor.visible)
            {
                var look = _actions.Player.Look.ReadValue<Vector2>();
                var lookX = look.x * settings.mouseSensitivity * Time.deltaTime;
                var lookY = look.y * settings.mouseSensitivity * Time.deltaTime;

                _xRotation -= lookY;
                _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

                cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
                transform.Rotate(Vector3.up * lookX);
            }

            #endregion
        }
        
        private void HandleVelocity()
        {
            if (_onGround && _velocity.y < 0)
            {
                _velocity.y = -2f;
            }

            // Handle jumping.
            if (_onGround && _actions.Player.Jump.WasPressedThisFrame())
            {
                _velocity.y = Mathf.Sqrt(settings.jumpHeight * -2f * settings.gravity);
            }

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
            
            var noiseLevel = Mathf.Lerp(0.1f, 1.0f, (100f - settings.sanity) / 100f);
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
    }
}