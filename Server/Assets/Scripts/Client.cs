using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class Client
{
    public static int dataBufferSize = 4096;

    public int id;

    public Player player;
    public TCP tcp;
    public UDP udp;

    public Client(int clientId)
    {
        id = clientId;

        tcp = new TCP(id);
        udp = new UDP(id);
    }

    public class TCP
    {
        public TcpClient socket;

        private readonly int id;

        private NetworkStream stream;
        private Packet receivedData;

        private byte[] receiveBuffer;

        public TCP(int id)
        {
            this.id = id;
        }

        public void Connect(TcpClient socket)
        {
            this.socket = socket;
            this.socket.ReceiveBufferSize = dataBufferSize;
            this.socket.SendBufferSize = dataBufferSize;

            stream = this.socket.GetStream();

            receivedData = new Packet();

            receiveBuffer = new byte[dataBufferSize];

            stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

            ServerSend.Welcome(id, "YoRoTo ������ ���� ���� ȯ���մϴ�!");
        }

        public void SendData(Packet packet)
        {
            try
            {
                if (socket != null)
                {
                    stream.BeginWrite(packet.ToArray(), 0, packet.Length(), null, null);
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"[Error] sending data to player {id} via TCP : {ex}");
            }
        }

        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            try
            {
                int byteLength = stream.EndRead(asyncResult);

                if (byteLength <= 0)
                {
                    Server.clients[id].Disconnect();

                    return;
                }

                byte[] data = new byte[byteLength];

                Array.Copy(receiveBuffer, data, byteLength);

                receivedData.Reset(HandleData(data));
                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }
            catch (Exception ex)
            {
                Debug.Log($"[Error] receiving TCP data : {ex}");

                Server.clients[id].Disconnect();
            }
        }

        private bool HandleData(byte[] data)
        {
            int packetLength = 0;

            receivedData.SetBytes(data);

            if (receivedData.UnreadLength() >= 4)
            {
                packetLength = receivedData.ReadInt();

                if (packetLength <= 0)
                {
                    return true;
                }
            }

            while (packetLength > 0 && packetLength <= receivedData.UnreadLength())
            {
                byte[] packetBytes = receivedData.ReadBytes(packetLength);

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using (Packet packet = new Packet(packetBytes))
                    {
                        int packetId = packet.ReadInt();

                        Server.packetHandlers[packetId](id, packet);
                    }
                });

                packetLength = 0;

                if (receivedData.UnreadLength() >= 4)
                {
                    packetLength = receivedData.ReadInt();

                    if (packetLength <= 0)
                    {
                        return true;
                    }
                }
            }

            if (packetLength <= 1)
            {
                return true;
            }

            return false;
        }

        public void Disconnect()
        {
            socket.Close();

            stream = null;
            receivedData = null;
            receiveBuffer = null;
            socket = null;
        }
    }

    public class UDP
    {
        public IPEndPoint endPoint;

        private int id;

        public UDP(int id)
        {
            this.id = id;
        }

        public void Connect(IPEndPoint endPoint)
        {
            this.endPoint = endPoint;
        }

        public void SendData(Packet packet)
        {
            Server.SendUDPData(endPoint, packet);
        }

        public void HandleData(Packet packetData)
        {
            int packetLength = packetData.ReadInt();

            byte[] packetBytes = packetData.ReadBytes(packetLength);

            ThreadManager.ExecuteOnMainThread(() =>
            {
                using (Packet packet = new Packet(packetBytes))
                {
                    int packetId = packet.ReadInt();

                    Server.packetHandlers[packetId](id, packet);
                }
            });
        }

        public void Disconnect()
        {
            endPoint = null;
        }
    }

    public void SendIntoGame(string playerName)
    {
        player = NetworkManager.instance.InstantiatePlayer();

        player.Initialize(id, playerName);

        for (int i = 1; i <= Server.clients.Values.Count; i++)
        {
            if (Server.clients[i].player != null)
            {
                if (Server.clients[i].id != id)
                {
                    ServerSend.SpawnPlayer(id, Server.clients[i].player);
                }
            }
        }

        for (int i = 1; i <= Server.clients.Values.Count; i++)
        {
            if (Server.clients[i].player != null)
            {
                ServerSend.SpawnPlayer(Server.clients[i].id, Server.clients[i].player);
            }
        }
    }

    private void Disconnect()
    {
        Debug.Log($"[Log] {tcp.socket.Client.RemoteEndPoint} has disconnected.");

        ThreadManager.ExecuteOnMainThread(() =>
        {
            UnityEngine.Object.Destroy(player.gameObject);

            player = null;
        });

        tcp.Disconnect();
        udp.Disconnect();

        ServerSend.PlayerDisconnected(id);
    }
}
