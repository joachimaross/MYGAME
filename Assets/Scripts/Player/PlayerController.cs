using System;
using UnityEngine;

namespace MarketHustle.Player
{
    /// <summary>
    /// Mobile-friendly first-person player controller.
    /// Hook this into a CharacterController or Rigidbody as needed.
    /// Movement input is expected to come from a floating joystick UI (set joystickInput from UI script).
    /// Look input for mobile can be set via swipe delta values or an on-screen look control.
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed = 4f;
        public float runMultiplier = 1.8f;

        [Header("Look")]
        public float lookSpeed = 0.2f;
        public Transform cameraHolder;

        [Header("References")]
        public CharacterController characterController;

        // Inputs (set from UI/InputHandler)
        [HideInInspector] public Vector2 joystickInput = Vector2.zero; // x = horizontal, y = forward
        [HideInInspector] public Vector2 lookDelta = Vector2.zero; // from swipe

        float pitch = 0f;

        void Reset()
        {
            characterController = GetComponent<CharacterController>();
            cameraHolder = Camera.main ? Camera.main.transform : null;
        }

        void Update()
        {
            HandleLook();
            HandleMove();
        }

        void HandleLook()
        {
            if (cameraHolder == null) return;

            // Apply look via lookDelta (set by touch/swipe)
            Vector2 d = lookDelta * lookSpeed;
            pitch -= d.y;
            pitch = Mathf.Clamp(pitch, -80f, 80f);
            cameraHolder.localEulerAngles = new Vector3(pitch, 0f, 0f);
            transform.Rotate(Vector3.up, d.x);

            // decay lookDelta after applying so UI sets deltas each frame
            lookDelta = Vector2.zero;
        }

        void HandleMove()
        {
            if (characterController == null) return;

            Vector3 forward = transform.forward * joystickInput.y;
            Vector3 right = transform.right * joystickInput.x;
            Vector3 move = (forward + right).normalized;

            bool running = Input.GetKey(KeyCode.LeftShift); // placeholder for mobile run toggle
            float speed = moveSpeed * (running ? runMultiplier : 1f);

            Vector3 velocity = move * speed;

            // gravity fallback
            velocity.y += Physics.gravity.y * Time.deltaTime;

            characterController.Move(velocity * Time.deltaTime);
        }
    }
}
