using UnityEngine;

namespace MarketHustle.Player
{
    /// <summary>
    /// Third-person player controller with mobile support.
    /// Handles movement, camera following, and basic interactions.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 5f;
        public float rotationSpeed = 10f;
        public Transform cameraHolder;

        [Header("Mobile Controls")]
        public RuntimeJoystick joystick;
        public float joystickDeadzone = 0.1f;

        private CharacterController characterController;
        private Vector3 moveDirection;
        private bool isMoving;

        void Start()
        {
            characterController = GetComponent<CharacterController>();
            if (characterController == null)
            {
                characterController = gameObject.AddComponent<CharacterController>();
                characterController.height = 2f;
                characterController.center = new Vector3(0f, 1f, 0f);
            }

            // Find joystick if not assigned
            if (joystick == null)
            {
                joystick = FindObjectOfType<RuntimeJoystick>();
            }

            // Set up camera if not assigned
            if (cameraHolder == null)
            {
                var cam = Camera.main;
                if (cam != null)
                {
                    cameraHolder = cam.transform;
                }
            }
        }

        void Update()
        {
            HandleMovement();
            HandleRotation();
        }

        void HandleMovement()
        {
            Vector3 inputDirection = Vector3.zero;

            // Keyboard input (fallback for testing)
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if (Mathf.Abs(horizontal) > joystickDeadzone || Mathf.Abs(vertical) > joystickDeadzone)
            {
                inputDirection = new Vector3(horizontal, 0f, vertical);
            }

            // Mobile joystick input
            if (joystick != null)
            {
                Vector2 joystickInput = joystick.Value;
                if (joystickInput.magnitude > joystickDeadzone)
                {
                    inputDirection = new Vector3(joystickInput.x, 0f, joystickInput.y);
                }
            }

            // Normalize and apply movement
            if (inputDirection.magnitude > 1f)
            {
                inputDirection.Normalize();
            }

            moveDirection = inputDirection * moveSpeed;
            characterController.Move(moveDirection * Time.deltaTime);

            isMoving = inputDirection.magnitude > joystickDeadzone;
        }

        void HandleRotation()
        {
            if (!isMoving) return;

            // Rotate towards movement direction
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        // Public method for external systems to move player
        public void MoveTo(Vector3 position)
        {
            characterController.enabled = false;
            transform.position = position;
            characterController.enabled = true;
        }

        // Interaction trigger
        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("StoreEntrance"))
            {
                var entrance = other.GetComponent<MarketHustle.Game.StoreEntrance>();
                if (entrance != null)
                {
                    entrance.EnterStore();
                }
            }
        }
    }
}
