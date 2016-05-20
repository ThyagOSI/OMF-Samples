namespace IngressServiceAPI.API
{
    /// <summary>
    /// This defines the relationship between a stream and a type.  StreamId and TypeId
    /// property names are defined in the OMF spec.  This class is serialized into 
    /// an OMF message.
    /// </summary>
    public class StreamInfo
    {
        public string StreamId { get; set; }
        public string TypeId { get; set; }
    }
}