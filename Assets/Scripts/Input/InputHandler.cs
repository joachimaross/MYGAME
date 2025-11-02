using UnityEngine;

/// <summary>
/// Hooks UI joysticks/touch input to the PlayerController.
/// Place this on a GameObject in scene (for example an InputManager) and assign the joysticks.
/// </summary>
public class InputHandler : MonoBehaviour
{
    public RuntimeJoystick moveJoystick;
    public float lookSensitivity = 0.2f;

    Camera mainCamera;
    MarketHustle.Player.PlayerController playerController;

    void Start()
    {
        mainCamera = Camera.main;
        var playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null) playerController = playerObj.GetComponent<MarketHustle.Player.PlayerController>();
    }

    void Update()
    {
        if (playerController == null)
        {
            var playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) playerController = playerObj.GetComponent<MarketHustle.Player.PlayerController>();
        }

        if (playerController == null) return;

        // joystick movement
        if (moveJoystick != null)
        {
            Vector2 v = moveJoystick.Value;
            playerController.joystickInput = v;
        }

        // simple swipe look: use mouse/touch delta (works for desktop testing)
        if (Input.GetMouseButton(0))
        {
            float dx = Input.GetAxis("Mouse X");
            float dy = Input.GetAxis("Mouse Y");
            playerController.lookDelta = new Vector2(dx, dy) * lookSensitivity;
        }
    }
}
