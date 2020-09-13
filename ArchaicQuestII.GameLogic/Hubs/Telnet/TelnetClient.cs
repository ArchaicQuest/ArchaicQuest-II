using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArchaicQuestII.GameLogic.Hubs.Telnet
{
    public class TelnetClient
    {
        private readonly NetworkStream _stream;
        private string _output = "";
        private TcpClient _tcpClient;
        private string _connectionID;

        // Buffer for reading from stream
        private byte[] streamreadbuffer;
        private readonly int BUF_SIZE = 256;

        // ASCII string terminators
        private readonly char NUL = (char)0x00;
        private readonly char LF = (char)0x0A;
        private readonly char CR = (char)0x0D;
        private readonly byte[] writelineterminate = { 0 };

        private TelnetClient()
        {
            
        }

        public TelnetClient(TcpClient tcpClient, string connectionID)
        {
            _tcpClient = tcpClient;
            _connectionID = connectionID;
            _stream = _tcpClient.GetStream();
        }

        public TelnetRetrievelData RetrieveInput()
        {
            // Read from the stream until done
            while (_stream.DataAvailable)
            {
                byte[] streamread = new byte[BUF_SIZE];
                int bytesread = _stream.Read(streamread, 0, BUF_SIZE);

                // Modify existing buffer to fit read data exactly, fill buffer with read data, don't copy NULs
                if (streamreadbuffer == null)
                {
                    // Use buffer position to properly insert characters in case a NUL is skipped
                    int bufpos = 0;
                    streamreadbuffer = new byte[bytesread];
                    for (int i = 0; i < bytesread; i++)
                    {
                        if ((char)streamread[i] != NUL)
                        {
                            streamreadbuffer[bufpos] = streamread[i];
                            bufpos += 1;
                        }
                    }

                }
                else
                {
                    // Set buffer to (buffer + bytes read)
                    byte[] newbuffer = new byte[streamreadbuffer.Length + bytesread];
                    Buffer.BlockCopy(streamreadbuffer, 0, newbuffer, 0, streamreadbuffer.Length);
                    Buffer.BlockCopy(streamread, 0, newbuffer, streamreadbuffer.Length + 1, streamread.Length);
                    streamreadbuffer = newbuffer;
                }
            }

            // Process buffer
            if (streamreadbuffer != null)
            {
                // Handle exceeding the max number of passes before finding a termination character
                if (streamreadbuffer.Length > TelnetConfig.MAX_NET_READS * BUF_SIZE)
                {
                    streamreadbuffer = null;
                    QueueOutput("SEND LENGTH EXCEEDED.");
                    return new TelnetRetrievelData(null, TelnetConfig.IO_READ.SENDEXCEED);
                }

                // Find first newline character
                int newlineindex = -1;
                for (int i = 0; i < streamreadbuffer.Length; i++)
                {
                    if ((char)streamreadbuffer[i] == LF)
                    {
                        newlineindex = i;
                    }
                }

                // If no newline is found, return pending a finished read
                if (newlineindex == -1)
                {
                    return new TelnetRetrievelData(null, TelnetConfig.IO_READ.PENDINGREAD);
                }
                else
                {
					if (newlineindex == 0)
                    {
                        // Only a LF has been sent -- reset the streamreadbuffer and send a newline to be processed
                        streamreadbuffer = null;
                        return new TelnetRetrievelData("\n", TelnetConfig.IO_READ.SUCCESSREAD);
                    }
                    else if (newlineindex == 1 && (char)streamreadbuffer[0] == CR && (char)streamreadbuffer[1] == LF)
                    {
                        // Only a CR LF has been sent -- reset the streamreadbuffer and send a newline to be processed
                        streamreadbuffer = null;
                        return new TelnetRetrievelData("\n", TelnetConfig.IO_READ.SUCCESSREAD);
                    }
                    else
                    {
                        // Data + LF have been sent, check to see if a CR has also been sent and decrement newline index if so
                        if (streamreadbuffer[newlineindex - 1] == CR)
                        {
                            newlineindex -= 1;
                        }

						// Process a line of input and remove processed data from buffer
						// The newline index now represents the desired length of input.
						// Process the input and set streamreadbuffer to null if that's it, otherwise remove the processed line
						// sb.ToString().TrimEnd( Environment.NewLine.ToCharArray());
						byte[] inputlinebuffer = new byte[newlineindex];

						for (int i = 0; i < newlineindex; i++)
                        {
                            inputlinebuffer[i] = streamreadbuffer[i];
                        }

                        // Check to see if that was all from the streamreadbuffer, and if so set the index
                        int firstcharindex = -1;
                        for (int i = newlineindex; i < streamreadbuffer.Length; i++)
                        {
                            if ((char)streamreadbuffer[i] != NUL && (char)streamreadbuffer[i] != CR && (char)streamreadbuffer[i] != LF)
                            {
                                firstcharindex = i;
                            }
                        }

                        if (firstcharindex == -1)
                        {
                            // End of stream, set to null
                            streamreadbuffer = null;
                        }
                        else
                        {
                            // Remove read data
                            byte[] newstreamreadbuffer = new byte[streamreadbuffer.Length - firstcharindex];
                            for (int i = firstcharindex; i < streamreadbuffer.Length; i++)
                            {
                                newstreamreadbuffer[i] = streamreadbuffer[i];
                            }
                            streamreadbuffer = newstreamreadbuffer;
                        }

                        // Process input
                        return new TelnetRetrievelData(Encoding.ASCII.GetString(inputlinebuffer), TelnetConfig.IO_READ.SUCCESSREAD);
                    }
                }
            }

            // If we made it here, there's either no data to process or the stream didn't include a newline
            return new TelnetRetrievelData(null, TelnetConfig.IO_READ.PENDINGREAD);
        }

        public void QueueOutput(string output)
        {
            if (output == null || output == "")
                return;

            this._output += "\n" + ((output == "\n") ? "" : output);
        }

        public void SendOutput()
        {
            if (_output != "")
            {

                // TODO: Strip HTML, process color


                byte[] send = Encoding.ASCII.GetBytes(_output.TrimEnd('\n').ToCharArray());
                _stream.Write(send, 0, send.Length);
                _stream.Write(writelineterminate, 0, 1);
                _output = "";
            }
        }

        public void CloseConnection()
        {
            SendOutput();
            _stream.Close();
        }
    }
}
