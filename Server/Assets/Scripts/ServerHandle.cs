using UnityEngine;

public class ServerHandle
{
    public static void WelcomeReceived(int fromClient, Packet packet)
    {
        int clientIdCheck = packet.ReadInt();
        string username = packet.ReadString();

        Debug.Log($"[Log] {Server.clients[fromClient].tcp.socket.Client.RemoteEndPoint} connected successfully and is now player {fromClient}.");

        if (fromClient != clientIdCheck)
        {
            Debug.Log($"[Log] Player \"{username}\" (ID : {fromClient}) has assumed the wrong client ID : ({fromClient})!");
        }

        Server.clients[fromClient].SendIntoGame(username);
    }

    public static void PlayerMovement(int fromClient, Packet packet)
    {
        bool[] inputs = new bool[packet.ReadInt()];

        for (int i = 0; i < inputs.Length; i++)
        {
            inputs[i] = packet.ReadBool();
        }

        Quaternion rotation = packet.ReadQuaternion();

        Server.clients[fromClient].player.SetInput(inputs, rotation);
    }

    public static void ChatSend(int fromClient, Packet packet)
    {
        int clientId = packet.ReadInt();
        string message = packet.ReadString();

        if (clientId == fromClient)
        {
            Debug.Log($"[Chat] Chat - {Server.clients[clientId].player.username} : {message}");

            ServerSend.ChatReceived($"{Server.clients[clientId].player.username} : {message}");
        }
    }

    public static void PlayerShoot(int fromClient, Packet packet)
    {
        Vector3 shootDirection = packet.ReadVector3();

        Server.clients[fromClient].player.Shoot(shootDirection);
    }
}
