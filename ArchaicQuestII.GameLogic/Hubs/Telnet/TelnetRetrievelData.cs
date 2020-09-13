using System;
using System.Collections.Generic;
using System.Text;

namespace ArchaicQuestII.GameLogic.Hubs.Telnet
{
	/// <summary>
	/// This class is for Telnet to return data retrieval results.
	/// </summary>
	public class TelnetRetrievelData
	{
		/// <summary>
		/// The data retrieved from the network connection
		/// </summary>
		public string Data { get; private set; }

		/// <summary>
		/// The status code of the input buffer read operation
		/// </summary>
		public TelnetConfig.IO_READ StatusCode { get; private set; }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="data">The data retrieved from Telnet Handler</param>
		/// <param name="statusCode">The status code of the data retrieval</param>
		public TelnetRetrievelData(string data, TelnetConfig.IO_READ statusCode)
		{
			Data = data;
			StatusCode = statusCode;
		}
	}
}
