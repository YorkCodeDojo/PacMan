﻿using System;
using System.IO;
using System.Text;

namespace NPacMan.Bot
{
    public class StreamString
    {
        private Stream ioStream;
        private UnicodeEncoding streamEncoding;

        public StreamString(Stream ioStream)
        {
            this.ioStream = ioStream;
            streamEncoding = new UnicodeEncoding();
        }

        public string ReadString()
        {
            try
            {
                int len;
                len = ioStream.ReadByte() * 256;
                len += ioStream.ReadByte();
                var inBuffer = new byte[len];
                ioStream.Read(inBuffer, 0, len);

                return streamEncoding.GetString(inBuffer);
            }
            catch (Exception ex)
            {
                throw new DeadPipeException("Error during ReadString", ex);
            }
        }

        public int WriteString(string outString)
        {
            try
            {
                byte[] outBuffer = streamEncoding.GetBytes(outString);
                int len = outBuffer.Length;
                if (len > UInt16.MaxValue)
                {
                    len = (int)UInt16.MaxValue;
                }
                ioStream.WriteByte((byte)(len / 256));
                ioStream.WriteByte((byte)(len & 255));
                ioStream.Write(outBuffer, 0, len);
                ioStream.Flush();

                return outBuffer.Length + 2;
            }
            catch (Exception ex)
            {
                throw new DeadPipeException("Error during WriteString", ex);
            }
        }
    }
}