using System.Collections.Generic;

namespace IngressServiceAPI.API
{
    /// <summary>
    /// Holds a set of values for a stream.  The property names StreamId and 
    /// Values are defined in the OMF spec.  This class is serialized into an 
    /// OMF message.
    /// </summary>
    public class StreamValues
    {
        public string StreamId { get; set; }
        public IEnumerable<object> Values { get; set; }
    }
}