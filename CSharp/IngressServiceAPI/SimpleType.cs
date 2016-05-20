using System;

namespace IngressServiceAPI
{
    /// <summary>
    /// Sample data structure to represent data objects in the target system.
    /// </summary>
    class SimpleType
    {
        public DateTime Time { get; set; }
        public double Value { get; set; }

        public const string JsonSchema =
            @"{
                ""id"": ""SimpleType"",
                ""type"": ""object"",
                ""properties"": {
                    ""Time"": { ""type"": ""string"", ""format"": ""date-time"", ""index"": true },
                    ""Value"": { ""type"": ""number"" }
                }
            }";
    }
    
}
