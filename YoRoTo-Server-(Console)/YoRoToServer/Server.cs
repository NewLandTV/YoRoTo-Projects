﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace YoRoToServer
{
    class Server
    {
        public static int MaxPlayers
        {
            get;
            private set;
        }

        public static int Port
        {
            get;
            private set;
        }

        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();

        public delegate void PacketHandler(int fromClient, Packet packet);

        public static Dictionary<int, PacketHandler> packetHandlers;

        private static TcpListener tcpListener;
        private static UdpClient udpListener;

        public static void Start(int maxPlayers, int port)
        {
            MaxPlayers = maxPlayers;
            Port = port;

            Console.WriteLine("[Log] Staring server...");

            InitializeServerData();

            tcpListener = new TcpListener(IPAddress.Any, Port);

            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            udpListener = new UdpClient(Port);

            udpListener.BeginReceive(UDPReceiveCallback, null);

            Console.WriteLine($"[Log] Server started on {Port}");
        }

        private static void TCPConnectCallback(IAsyncResult asyncResult)
        {
            TcpClient client = tcpListener.EndAcceptTcpClient(asyncResult);

            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            Console.WriteLine($"[Log] Incoming connection from {client.Client.RemoteEndPoint}...");

            for (int i = 1; i <= MaxPlayers; i++)
            {
                if (clients[i].tcp.socket == null)
                {
                    clients[i].tcp.Connect(client);

                    return;
                }
            }
            Console.WriteLine($"[Log] {client.Client.RemoteEndPoint} failed to connect : Server full!");
        }

        private static void UDPReceiveCallback(IAsyncResult asyncResult)
        {
            try
            {
                IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

                byte[] data = udpListener.EndReceive(asyncResult, ref clientEndPoint);

                udpListener.BeginReceive(UDPReceiveCallback, null);

                if (data.Length < 4)
                {
                    return;
                }

                using (Packet packet = new Packet(data))
                {
                    int clientId = packet.ReadInt();
                    
                    if (clientId == 0)
                    {
                        return;
                    }

                    if (clients[clientId].udp.endPoint == null)
                    {
                        clients[clientId].udp.Connect(clientEndPoint);

                        return;
                    }

                    if (clients[clientId].udp.endPoint.ToString().Equals(clientEndPoint.ToString()))
                    {
                        clients[clientId].udp.HandleData(packet);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] receiving UDP data : {ex}");
            }
        }

        public static void SendUDPData(IPEndPoint clientEndPoint, Packet packet)
        {
            try
            {
                if (clientEndPoint != null)
                {
                    udpListener.BeginSend(packet.ToArray(), packet.Length(), clientEndPoint, null, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Error] sending data to {clientEndPoint} via UDP : {ex}");
            }
        }

        private static void InitializeServerData()
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                clients.Add(i, new Client(i));
            }

            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                { (int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
                { (int)ClientPackets.playerMovement, ServerHandle.PlayerMovement },
                { (int)ClientPackets.chatSend, ServerHandle.ChatSend }
            };

            Console.WriteLine("[Log] Initialized packets.");
        }
    }
}
