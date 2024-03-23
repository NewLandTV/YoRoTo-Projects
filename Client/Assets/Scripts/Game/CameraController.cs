using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Player player;

    [SerializeField]
    private float sensitivity = 100f;
    [SerializeField]
    private float clampAngle = 85f;

    private float verticalRotation;
    private float horizontalRotation;

    private void Awake()
    {
        verticalRotation = transform.localEulerAngles.x;
        horizontalRotation = transform.localEulerAngles.y;

        Cursor.visible = false;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private IEnumerator Start()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ToggleCursorMode();
            }

            if (Cursor.lockState == CursorLockMode.Locked)
            {
                Look();
            }

            yield return null;
        }
    }

    private void Look()
    {
        float mouseVertical = Input.GetAxis("Mouse Y");
        float mouseHorizontal = Input.GetAxis("Mouse X");

        verticalRotation -= mouseVertical * sensitivity * Time.deltaTime;
        horizontalRotation += mouseHorizontal * sensitivity * Time.deltaTime;

        verticalRotation = Mathf.Clamp(verticalRotation, -clampAngle, clampAngle);

        transform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
        player.transform.rotation = Quaternion.Euler(0f, horizontalRotation, 0f);
    }

    private void ToggleCursorMode()
    {
        Cursor.visible = !Cursor.visible;

        Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
