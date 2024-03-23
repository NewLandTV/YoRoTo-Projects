using System;
using System.Threading;

namespace YoRoToServer
{
    class Program
    {
        private static bool isRunning;

        static void Main(string[] args)
        {
            Console.Title = "YoRoTo Server";

            isRunning = true;

            Thread mainThread = new Thread(new ThreadStart(MainThread));

            mainThread.Start();

            #region YoRoTo Web Server
            new Thread(new ThreadStart(delegate
            {
                Console.WriteLine("[Log] Started YoRoTo web server.");

                System.Globalization.CultureInfo cultureInfo = new System.Globalization.CultureInfo("en-US");
                System.Net.IPEndPoint ipep = new System.Net.IPEndPoint(System.Net.IPAddress.Any, 9267);
                System.Net.Sockets.Socket server = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork, System.Net.Sockets.SocketType.Stream, System.Net.Sockets.ProtocolType.Tcp);

                string sendData = "<!DOCTYPE html>\n<html>\n<head>\n    <meta charset=\"UTF-8\">\n    <title>Welcome to YoRoTo Web Server</title>\n</head>\n<body>\n    <h1>YoRoTo Web Channel 1</h1>\n</body>\n</html>";

                server.Bind(ipep);
                server.Listen(100);

                while (true)
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder(100);
                    System.Net.Sockets.Socket client = server.Accept();

                    Console.WriteLine($"[Log] {client.RemoteEndPoint} Join to YoRoTo Web Server.");

                    try
                    {
                        sb.AppendLine("HTTP/1.1 200 ok");
                        sb.AppendLine($"date: {DateTime.UtcNow.ToString("ddd, dd MMM yyy HH':'mm':'ss 'GMT'", cultureInfo)}");
                        sb.AppendLine("server: yoroto web server");
                        sb.AppendLine($"Content-Length: {sendData.Length}");
                        sb.AppendLine("content-type:text/html; charset=UTF-8");
                        sb.AppendLine();
                        sb.AppendLine(sendData);

                        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());

                        client.Send(bytes);

                        Thread.Sleep(10);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Error] On YoRoTo web server error : {ex}");
                    }
                    finally
                    {
                        client.Close();
                    }
                }
            })).Start();
            #endregion

            Server.Start(50, 9268);
        }

        private static void MainThread()
        {
            Console.WriteLine($"[Log] Main thread started. Running at {Constants.TICKS_PER_SECOND} ticks per second.");

            DateTime nextLoop = DateTime.Now;

            while (isRunning)
            {
                while (nextLoop < DateTime.Now)
                {
                    GameLogic.Update();

                    nextLoop = nextLoop.AddMilliseconds(Constants.MS_PER_TICK);

                    if (nextLoop > DateTime.Now)
                    {
                        Thread.Sleep(nextLoop - DateTime.Now);
                    }
                }
            }
        }
    }
}
