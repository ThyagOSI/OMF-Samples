using System;

namespace IngressServiceAPI
{
    class ComplexType
    {
        public DateTime DateTimeProperty { get; set; }
        public string StringProperty { get; set; }
        public double DoubleProperty { get; set; }
        public int IntegerProperty { get; set; }
        public ComplexType_Nested1 ObjectProperty { get; set; }

        public static ComplexType CreateSampleValue()
        {
            ComplexType sample = new ComplexType();
            sample.DateTimeProperty = DateTime.UtcNow;
            sample.StringProperty = "SampleValue";
            sample.DoubleProperty = 1.1;
            sample.IntegerProperty = 2;
            sample.ObjectProperty = new ComplexType_Nested1()
            {
                NumberProperty = 3.3,
                IntegerArrayProperty = new int[] { 1, 2, 3 },
                NestedObjectProperty = new ComplexType_Nested2()
                {
                    NumberProperty = 4.4,
                    StringProperty = "Nested string value"
                }
            };

            return sample;
        }

        public static string JsonSchema =
        @"{
            ""id"": ""ComplexType"",
            ""description"": ""A complex type including strings, ints, doubles, arrays, and nested objects"",
            ""type"": ""object"",
            ""properties"": {
                ""DateTimeProperty"": { ""type"": ""string"", ""format"": ""date-time"", ""index"": true },
                ""StringProperty"": { ""type"": ""string""},
                ""DoubleProperty"": { ""type"": ""number"" },
                ""IntegerProperty"": { ""type"": ""integer"" },
                ""ObjectProperty"": {
                    ""type"": ""object"",
                    ""properties"": {
                        ""NumberProperty"": { ""type"": ""number"" },
                        ""IntegerArrayProperty"": { ""type"": ""array"", ""items"": { ""type"": ""integer"" }, ""maxItems"": 3 },
                        ""NestedObjectProperty"": {
                            ""type"": ""object"",
                            ""properties"": {
                                ""NumberProperty"": { ""type"": ""number"" },
                                ""StringProperty"": { ""type"": ""string"" }
                            }
                        }
                    }
                }
            }
        }";  
    }

    class ComplexType_Nested1
    {
        public double NumberProperty { get; set; }
        public int[] IntegerArrayProperty { get; set; }
        public ComplexType_Nested2 NestedObjectProperty { get; set; }
    }

    class ComplexType_Nested2
    {
        public double NumberProperty { get; set; }
        public string StringProperty { get; set; }
    }
}
