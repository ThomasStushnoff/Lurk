using System;
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
            // TODO: Figure out all controls.
        }
        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            HandleMovement();
            HandleVelocity();
            HandleCursor();
            CheckGrounded();
        }
        
        private void FixedUpdate()
        {
            
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
            var speed = settings.movementSpeed;

            character.Move(move * (speed * Time.deltaTime));

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
    }
}