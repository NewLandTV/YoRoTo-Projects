using UnityEngine;

public class ClientSend : MonoBehaviour
{
    private static void SendTCPData(Packet packet)
    {
        packet.WriteLength();

        Client.instance.tcp.SendData(packet);
    }

    private static void SendUDPData(Packet packet)
    {
        packet.WriteLength();

        Client.instance.udp.SendData(packet);
    }

    #region Packets

    public static void WelcomeReceived()
    {
        using (Packet packet = new Packet((int)ClientPackets.welcomeReceived))
        {
            packet.Write(Client.instance.myId);
            packet.Write($"Player {Client.instance.myId}");

            SendTCPData(packet);
        }
    }

    public static void PlayerMovement(bool[] inputs)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerMovement))
        {
            packet.Write(inputs.Length);

            for (int i = 0; i < inputs.Length; i++)
            {
                packet.Write(inputs[i]);
            }

            packet.Write(GameManager.players[Client.instance.myId].transform.rotation);

            SendUDPData(packet);
        }
    }

    public static void ChatSend(string message)
    {
        using (Packet packet = new Packet((int)ClientPackets.chatSend))
        {
            packet.Write(Client.instance.myId);
            packet.Write(message);

            SendTCPData(packet);
        }
    }

    public static void PlayerShoot(Vector3 facing)
    {
        using (Packet packet = new Packet((int)ClientPackets.playerShoot))
        {
            packet.Write(facing);

            SendTCPData(packet);
        }
    }

    #endregion
}
