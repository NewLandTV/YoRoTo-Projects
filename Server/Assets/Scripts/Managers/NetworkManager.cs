using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    [SerializeField]
    private GameObject playerPrefab;

    private void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 30;

        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Server.Start(50, 9268);
    }

    private void OnApplicationQuit()
    {
        Server.Stop();
    }

    public Player InstantiatePlayer()
    {
        return Instantiate(playerPrefab, Vector3.down * 0.5f, Quaternion.identity).GetComponent<Player>();
    }
}
