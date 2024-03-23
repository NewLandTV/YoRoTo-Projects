namespace YoRoToServer
{
    class GameLogic
    {
        public static void Update()
        {
            for (int i = 1; i <= Server.clients.Values.Count; i++)
            {
                if (Server.clients[i].player != null)
                {
                    Server.clients[i].player.Update();
                }
            }

            ThreadManager.UpdateMain();
        }
    }
}
