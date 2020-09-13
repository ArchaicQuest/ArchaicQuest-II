using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArchaicQuestII.GameLogic.Hubs.Telnet
{
    /// <summary>
    /// Singleton Telnet Hub
    /// </summary>
    public sealed class TelnetHub
    {
        private static TelnetHub _instance;

        private Thread _connectionLoop;

        private Dictionary<string, TelnetClient> _telnetClients;
        TcpListener _telnetServer;

        private bool _running;

        /// <summary>
        /// Private constructor for singleton pattern
        /// </summary>
        private TelnetHub()
        {

        }

        /// <summary>
        /// Initializes and starts the telnet service
        /// </summary>
        public void Start()
        {
            if (_running is false)
            {
                _connectionLoop = new Thread(new ThreadStart(ProcessConnections));
            }

            _running = true;
        }

        /// <summary>
        /// Processes accepting/closing telnet connections and telnet input/outpit
        /// </summary>
        public void ProcessConnections()
		{
            do
            {
                // Accept connections
                if (_telnetServer.Pending())
                {
                    var connectionID = Guid.NewGuid().ToString();
                    TelnetClient client = new TelnetClient(_telnetServer.AcceptTcpClient(), connectionID);
                    _telnetClients.Add(connectionID, client);

                    client.QueueOutput("Welcome!");
                }

                // Process input
                foreach (var client in _telnetClients)
				{
                    var read = client.Value.RetrieveInput();

                    // TODO: Process command

                    // ... for now, echo input
                    if (read.StatusCode == TelnetConfig.IO_READ.SUCCESSREAD)
                        client.Value.QueueOutput(read.Data);
                }

                // Process output
                foreach (var client in _telnetClients)
				{
                    client.Value.SendOutput();
				}

                Thread.Sleep(10);
            } while (_running);
		}

        /// <summary>
        /// Sends message to all Telnet clients
        /// </summary>
        /// <param name="message">The message to send to the client</param>
        public void Send(string message)
		{
            foreach (var client in _telnetClients)
			{
                client.Value.QueueOutput(message);
			}
        }

        /// <summary>
        /// Sends message to a specific Telnet client
        /// </summary>
        /// <param name="message">The message to send to the client</param>
        /// <param name="hubId">The Telnet connection ID</param>
        public void SendToClient(string message, string hubId)
        {
			TelnetClient client;
            _telnetClients.TryGetValue(hubId, out client);

            if (client != null)
			{
                client.QueueOutput(message);
			}
        }

        /// <summary>
        /// Retrieves TelnetHub singleton instance
        /// </summary>
        public static TelnetHub Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new TelnetHub();

                return _instance;
            }
        }
    }
}
