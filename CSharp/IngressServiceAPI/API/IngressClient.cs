using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IngressServiceAPI.API
{
    /// <summary>
    /// Client used to send OMF message to the ingress service.
    /// </summary>
    public class IngressClient : IDisposable
    {
        private readonly HttpClient _client;
        private string _producerToken;

        public bool UseCompression { get; set; }

        /// <summary>
        /// Create an IngressClient by passing it the required connection information.
        /// </summary>
        /// <param name="serviceUrl">The HTTP endpoint for the ingress service.</param>
        /// <param name="producerToken">Security token used to authenticate with the service.</param>     
        public IngressClient(string serviceUrl, string producerToken)
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(serviceUrl);
            _producerToken = producerToken;
        }

        /// <summary>
        /// Sends a collection of JSONSchema strings to ingress service.  The JSONSchema describes
        /// the types used for the data that will be sent.
        /// </summary>
        /// <param name="types">A collection of JSONSchema string.</param>
        public void CreateTypes(IEnumerable<string> types)
        {
            string json = string.Format("[{0}]", string.Join(",", types));
            var bytes = Encoding.UTF8.GetBytes(json);
            SendMessageAsync(bytes, MessageType.Types, MessageAction.Create).Wait();
        }

        /// <summary>
        /// Sends a collection of StreamId, TypeId pairs to the endpoint.
        /// </summary>
        /// <param name="streams"></param>
        public void CreateStreams(IEnumerable<StreamInfo> streams)
        {
            string json = JsonConvert.SerializeObject(streams);
            var bytes = Encoding.UTF8.GetBytes(json);
            SendMessageAsync(bytes, MessageType.Streams, MessageAction.Create).Wait();
        }

        /// <summary>
        /// Sends the actual values to the ingress service.  This is async to allow for higher
        /// throughput to the event hub.
        /// </summary>
        /// <param name="values">A collection of values and their associated streams.</param>
        /// <returns></returns>
        public Task SendValuesAsync(IEnumerable<StreamValues> values)
        {
            string json = JsonConvert.SerializeObject(values);
            var bytes = Encoding.UTF8.GetBytes(json);
            return SendMessageAsync(bytes, MessageType.Values, MessageAction.Create);
        }

        private async Task SendMessageAsync(byte[] body, MessageType msgType, MessageAction action)
        {
            Message msg = new Message();
            msg.ProducerToken = _producerToken;
            msg.MessageType = msgType;
            msg.Action = action;
            msg.MessageFormat = MessageFormat.JSON;
            msg.Body = body;

            if (UseCompression)
                msg.Compress(MessageCompression.GZip);

            HttpContent content = HttpContentFromMessage(msg);
            HttpResponseMessage response = await _client.PostAsync("" /* use the base URI */, content);
            response.EnsureSuccessStatusCode();
        }

        private HttpContent HttpContentFromMessage(Message msg)
        {
            ByteArrayContent content = new ByteArrayContent(msg.Body);
            foreach(var header in msg.Headers)
            {
                content.Headers.Add(header.Key, header.Value);
            }
            return content;
        }

        #region IDisposable
        private bool _disposed = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _client.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
