using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.GameLogic.Hubs.Telnet
{
	/// <summary>
	/// Various configuration options for Telnet
	/// </summary>
	public struct TelnetConfig
	{
		/// <summary>
		/// Maximum number of network reads before overflow
		/// </summary>
		public const int MAX_NET_READS = 8;

		/// <summary>
		/// Telnet server listening port
		/// </summary>
		public const int SERVER_LISTEN_PORT = 4000;

		/// <summary>
		/// Telnet server IP
		/// </summary>
		public const string SERVER_LISTEN_IP = "127.0.0.1";

		/// <summary>
		/// Telnet IO stream reader states
		/// </summary>
		public enum IO_READ
		{
			SENDEXCEED = -1,
			PENDINGREAD,
			SUCCESSREAD
		}
	}
}