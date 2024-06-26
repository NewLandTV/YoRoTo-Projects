﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace YoRoToServer
{
    /// <summary>Sent from server to client.</summary>
    public enum ServerPackets
    {
        welcome = 1,
        spawnPlayer,
        playerPosition,
        playerRotation,
        chatReceived
    }

    /// <summary>Sent from client to server.</summary>
    public enum ClientPackets
    {
        welcomeReceived = 1,
        playerMovement,
        chatSend
    }

    public class Packet : IDisposable
    {
        private List<byte> buffer;

        private byte[] readableBuffer;

        private int readPos;

        #region Constructor

        /// <summary>Creates a new empty packet (without an ID).</summary>
        public Packet()
        {
            buffer = new List<byte>();

            readPos = 0;
        }

        /// <summary>Creates a new packet with a given ID. Used for sending.</summary>
        /// <param name="id">The packet ID.</param>
        public Packet(int id)
        {
            buffer = new List<byte>();

            readPos = 0;

            Write(id);
        }

        /// <summary>Creates a packet from which data can be read. Used for receiving.</summary>
        /// <param name="data">The bytes to add to the packet.</param>
        public Packet(byte[] data)
        {
            buffer = new List<byte>();

            readPos = 0;

            SetBytes(data);
        }

        #endregion

        #region Functions

        /// <summary>Sets the packet's content and prepares it to be read.</summary>
        /// <param name="data">The bytes to add to the packet.</param>
        public void SetBytes(byte[] data)
        {
            Write(data);

            readableBuffer = buffer.ToArray();
        }

        /// <summary>Inserts the length of the packet's content at the start of the buffer.</summary>
        public void WriteLength()
        {
            buffer.InsertRange(0, BitConverter.GetBytes(buffer.Count));
        }

        /// <summary>Inserts the given int at the start of the buffer.</summary>
        /// <param name="value">The int to insert.</param>
        public void InsertInt(int value)
        {
            buffer.InsertRange(0, BitConverter.GetBytes(value));
        }

        /// <summary>Gets the packet's content in array form.</summary>
        public byte[] ToArray()
        {
            readableBuffer = buffer.ToArray();

            return readableBuffer;
        }

        /// <summary>Gets the length of the packet's content.</summary>
        public int Length()
        {
            return buffer.Count;
        }

        /// <summary>Gets the length of the unread data contained in the packet.</summary>
        public int UnreadLength()
        {
            return Length() - readPos;
        }

        /// <summary>Resets the packet instance to allow it to be reused.</summary>
        /// <param name="shouldReset">Whether or not to reset the packet.</param>
        public void Reset(bool shouldReset = true)
        {
            if (shouldReset)
            {
                buffer.Clear();

                readableBuffer = null;
                readPos = 0;
            }
            else
            {
                readPos -= 4;
            }
        }

        #endregion

        #region Write

        /// <summary>Adds a byte to the packet.</summary>
        /// <param name="value">The byte to add.</param>
        public void Write(byte value)
        {
            buffer.Add(value);
        }

        /// <summary>Adds an array of bytes to the packet.</summary>
        /// <param name="value">The byte array to add.</param>
        public void Write(byte[] value)
        {
            buffer.AddRange(value);
        }

        /// <summary>Adds a short to the packet.</summary>
        /// <param name="value">The short to add.</param>
        public void Write(short value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }

        /// <summary>Adds an int to the packet.</summary>
        /// <param name="value">The int to add.</param>
        public void Write(int value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }

        /// <summary>Adds a long to the packet.</summary>
        /// <param name="value">The long to add.</param>
        public void Write(long value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }

        /// <summary>Adds a float to the packet.</summary>
        /// <param name="value">The float to add.</param>
        public void Write(float value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }

        /// <summary>Adds a bool to the packet.</summary>
        /// <param name="value">The bool to add.</param>
        public void Write(bool value)
        {
            buffer.AddRange(BitConverter.GetBytes(value));
        }

        /// <summary>Adds a string to the packet.</summary>
        /// <param name="value">The string to add.</param>
        public void Write(string value)
        {
            Write(value.Length);

            buffer.AddRange(Encoding.ASCII.GetBytes(value));
        }

        /// <summary>Adds a Vector3 to the packet.</summary>
        /// <param name="value">The Vector3 to add.</param>
        public void Write(Vector3 value)
        {
            Write(value.X);
            Write(value.Y);
            Write(value.Z);
        }

        /// <summary>Adds a Quaternion to the packet.</summary>
        /// <param name="value">The Quaternion to add.</param>
        public void Write(Quaternion value)
        {
            Write(value.X);
            Write(value.Y);
            Write(value.Z);
            Write(value.W);
        }

        #endregion

        #region Read Data

        /// <summary>Reads a byte from the packet.</summary>
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        public byte ReadByte(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                byte value = readableBuffer[readPos];

                if (moveReadPos)
                {
                    readPos += 1;
                }

                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'byte'!");
            }
        }

        /// <summary>Reads an array of bytes from the packet.</summary>
        /// <param name="length">The length of the byte array.</param>
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        public byte[] ReadBytes(int length, bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                byte[] value = buffer.GetRange(readPos, length).ToArray();

                if (moveReadPos)
                {
                    readPos += length;
                }

                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'byte[]'!");
            }
        }

        /// <summary>Reads a short from the packet.</summary>
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        public short ReadShort(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                short value = BitConverter.ToInt16(readableBuffer, readPos);

                if (moveReadPos)
                {
                    readPos += 2;
                }

                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'short'!");
            }
        }

        /// <summary>Reads an int from the packet.</summary>
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        public int ReadInt(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                int value = BitConverter.ToInt32(readableBuffer, readPos);

                if (moveReadPos)
                {
                    readPos += 4;
                }

                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'int'!");
            }
        }

        /// <summary>Reads a long from the packet.</summary>
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        public long ReadLong(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                long value = BitConverter.ToInt64(readableBuffer, readPos);

                if (moveReadPos)
                {
                    readPos += 8;
                }

                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'long'!");
            }
        }

        /// <summary>Reads a float from the packet.</summary>
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        public float ReadFloat(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                float value = BitConverter.ToSingle(readableBuffer, readPos);

                if (moveReadPos)
                {
                    readPos += 4;
                }

                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'float'!");
            }
        }

        /// <summary>Reads a bool from the packet.</summary>
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        public bool ReadBool(bool moveReadPos = true)
        {
            if (buffer.Count > readPos)
            {
                bool value = BitConverter.ToBoolean(readableBuffer, readPos);

                if (moveReadPos)
                {
                    readPos += 1;
                }

                return value;
            }
            else
            {
                throw new Exception("Could not read value of type 'bool'!");
            }
        }

        /// <summary>Reads a string from the packet.</summary>
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        public string ReadString(bool moveReadPos = true)
        {
            try
            {
                int length = ReadInt();
                string value = Encoding.ASCII.GetString(readableBuffer, readPos, length);

                if (moveReadPos && value.Length > 0)
                {
                    readPos += length;
                }

                return value;
            }
            catch
            {
                throw new Exception("Could not read value of type 'string'!");
            }
        }

        /// <summary>Reads a Vector3 from the packet.</summary>
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        public Vector3 ReadVector3(bool moveReadPos = true)
        {
            return new Vector3(ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos));
        }

        /// <summary>Reads a Quaternion from the packet.</summary>
        /// <param name="moveReadPos">Whether or not to move the buffer's read position.</param>
        public Quaternion ReadQuaternion(bool moveReadPos = true)
        {
            return new Quaternion(ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos), ReadFloat(moveReadPos));
        }

        #endregion

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    buffer = null;
                    readableBuffer = null;
                    readPos = 0;
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }
    }
}
