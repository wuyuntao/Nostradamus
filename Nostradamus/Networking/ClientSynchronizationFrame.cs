namespace Nostradamus.Networking
{
    class ClientSynchronizationFrame
    {
        public readonly int ClientId;
        public readonly int Time;
        public readonly Command[] Commands;

        public ClientSynchronizationFrame(int clientId, int time, Command[] commands)
        {
            ClientId = clientId;
            Time = time;
            Commands = commands;
        }
    }
}
