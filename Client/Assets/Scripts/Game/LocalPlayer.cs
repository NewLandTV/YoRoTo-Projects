using System.Collections;
using UnityEngine;

public class LocalPlayer : MonoBehaviour
{
    [SerializeField]
    private Transform cameraTransform;

    private IEnumerator Start()
    {
        StartCoroutine(Tick());

        while (true)
        {
            SendInputToServer();

            yield return new WaitForFixedUpdate();
        }
    }

    private IEnumerator Tick()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                ClientSend.PlayerShoot(cameraTransform.forward);
            }

            yield return null;
        }
    }

    private void SendInputToServer()
    {
        bool[] inputs = new bool[5]
        {
            Input.GetKey(KeyCode.W),
            Input.GetKey(KeyCode.S),
            Input.GetKey(KeyCode.A),
            Input.GetKey(KeyCode.D),
            Input.GetKey(KeyCode.Space)
        };

        ClientSend.PlayerMovement(inputs);
    }
}
