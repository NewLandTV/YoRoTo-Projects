using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public static Dictionary<int, Player> players = new Dictionary<int, Player>();

    [SerializeField]
    private GameObject localPlayerPrefab;
    [SerializeField]
    private GameObject playerPrefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        Client.instance.ConnectToServer();
    }

    public void SpawnPlayer(int id, string username, Vector3 spawnPosition, Quaternion spawnRotation)
    {
        GameObject player;

        if (id == Client.instance.myId)
        {
            player = Instantiate(localPlayerPrefab, spawnPosition, spawnRotation);
        }
        else
        {
            player = Instantiate(playerPrefab, spawnPosition, spawnRotation);
        }

        Player playerComponent = player.GetComponent<Player>();

        playerComponent.Initialize(id, username);

        players.Add(id, playerComponent);
    }
}
