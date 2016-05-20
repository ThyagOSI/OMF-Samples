using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace IngressServiceAPI.API
{
    /// <summary>
    /// Holds the data for an OMF message.  Contains a set of properties that map to the message
    /// header values and a byte array to hold the message body.  Also supports compressing
    /// and decompressing of the body.
    /// </summary>
    public class Message
    {
        public const string HeaderKey_ProducerToken = "producertoken";
        public const string HeaderKey_MessageType = "messagetype";
        public const string HeaderKey_MessageFormat = "messageformat";
        public const string HeaderKey_MessageCompression = "messagecompression";
        public const string HeaderKey_Action = "action";

        public IDictionary<string, string> Headers { get; private set; }
        public byte[] Body { get; set; }

        public string ProducerToken
        {
            get
            {
                string token = string.Empty;
                Headers.TryGetValue(HeaderKey_ProducerToken, out token);
                return token;
            }
            set
            {
                Headers[HeaderKey_ProducerToken] = value;
            }
        }

        public MessageType MessageType
        {
            get
            {
                MessageType type = MessageType.Values;

                string headerVal = "";
                if (Headers.TryGetValue(HeaderKey_MessageType, out headerVal))
                {
                    Enum.TryParse(headerVal, true, out type);
                }

                return type;
            }
            set
            {
                Headers[HeaderKey_MessageType] = value.ToString();
            }
        }

        public MessageFormat MessageFormat
        {
            get
            {
                MessageFormat format = MessageFormat.JSON;

                string headerVal = "";
                if (Headers.TryGetValue(HeaderKey_MessageFormat, out headerVal))
                {
                    Enum.TryParse(headerVal, true, out format);
                }

                return format;
            }
            set
            {
                Headers[HeaderKey_MessageFormat] = value.ToString();
            }
        }

        public MessageCompression MessageCompression
        {
            get
            {
                MessageCompression compression = MessageCompression.None;

                string headerVal = "";
                if (Headers.TryGetValue(HeaderKey_MessageCompression, out headerVal))
                {
                    Enum.TryParse(headerVal, true, out compression);
                }

                return compression;
            }
            set
            {
                Headers[HeaderKey_MessageCompression] = value.ToString();
            }
        }

        public MessageAction Action
        {
            get
            {
                MessageAction action = MessageAction.Create;
                string headerVal = "";
                if (Headers.TryGetValue(HeaderKey_Action, out headerVal))
                {
                    Enum.TryParse(headerVal, true, out action);
                }

                return action;
            }

            set
            {
                Headers[HeaderKey_Action] = value.ToString();
            }
        }

        public Message()
        {
            Headers = new Dictionary<string, string>();
        }

        public void Compress(MessageCompression compressionType)
        {
            if (compressionType != MessageCompression.GZip)
                throw new NotImplementedException("Only GZip compression is currently supported.");

            Body = GZipCompress(Body);
            MessageCompression = compressionType;
        }

        public void Decompress()
        {

            switch (MessageCompression)
            {
                case MessageCompression.None:
                    break;
                case MessageCompression.GZip:
                    Body = GZipDecompress(Body);
                    break;
                default:
                    throw new NotImplementedException(string.Format("{0} compression is not implemented.", MessageCompression));
            }

            MessageCompression = MessageCompression.None;
        }

        private static byte[] GZipCompress(byte[] uncompressed)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                using (GZipStream gzip = new GZipStream(memory, CompressionMode.Compress, true))
                {
                    gzip.Write(uncompressed, 0, uncompressed.Length);
                }
                return memory.ToArray();
            }
        }

        private static byte[] GZipDecompress(byte[] compressed)
        {
            using (MemoryStream memoryStream = new MemoryStream(compressed))
            using (GZipStream stream = new GZipStream(memoryStream, CompressionMode.Decompress))
            {
                const int size = 4096;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = stream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    return memory.ToArray();
                }
            }
        }
    }

    public enum MessageType
    {
        Values = 0,
        Streams,
        Types
    }

    public enum MessageFormat
    {
        JSON = 0
    }

    public enum MessageCompression
    {
        None = 0,
        GZip
    }

    public enum MessageAction
    {
        Create = 0,
        Update,
        Delete
    }


}
