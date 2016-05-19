# Some test values we can use to write to our streams.

# In these examples, we set the time properties to the current UTC time
# minus some millisecond offset which creates unique indexes into our time
# indexed streams.  The time format needs to be ISO 8601.  The 'Z' appended at the
# end of the timestamp tells the service that it's UTC rather than the local timezone.

import json
from datetime import *

now = datetime.utcnow()
t1 = (now - timedelta(milliseconds=1)).isoformat() + 'Z'
t2 = (now - timedelta(milliseconds=2)).isoformat() + 'Z'
t3 = (now - timedelta(milliseconds=3)).isoformat() + 'Z'
t4 = (now - timedelta(milliseconds=4)).isoformat() + 'Z'
t5 = (now - timedelta(milliseconds=5)).isoformat() + 'Z'
t6 = (now - timedelta(milliseconds=6)).isoformat() + 'Z'

simple_stream1_values = {
    "streamId": "simple_stream1",
    "values": [
        { "IndexedDateTimeProperty": t6, "NumberProperty": 99.4 },
        { "IndexedDateTimeProperty": t5, "NumberProperty": 99 },
        { "IndexedDateTimeProperty": t4, "NumberProperty": 100 },
        { "IndexedDateTimeProperty": t3, "NumberProperty": 99.4 },
        { "IndexedDateTimeProperty": t2, "NumberProperty": 99 },
        { "IndexedDateTimeProperty": t1, "NumberProperty": 100 }
    ]
}

simple_stream2_values = {
    "streamId": "simple_stream2",
    "values": [
        { "IndexedDateTimeProperty": t6, "NumberProperty": 99.4 },
        { "IndexedDateTimeProperty": t5, "NumberProperty": 99 },
        { "IndexedDateTimeProperty": t4, "NumberProperty": 100 },
        { "IndexedDateTimeProperty": t3, "NumberProperty": 99.4 },
        { "IndexedDateTimeProperty": t2, "NumberProperty": 99 },
        { "IndexedDateTimeProperty": t1, "NumberProperty": 100 }
    ]
}

complex_stream_values = {
    "streamId": "complex_stream",
    "values": [
        {
            "IndexedDateTimeProperty": t1,
            "StringProperty": "ExampleAsset",
            "DoubleProperty": 123.4,
            "IntegerProperty": -123,
            "ObjectProperty": {
                "NumberProperty": 345,
                "IntegerArrayProperty": [ 1, 2, 3 ],
                "NestedObjectProperty": {
                    "NumberProperty": -111,
                    "StringProperty": "TestString"
                }
            }
        }
    ]
}


# Add all the values into single json array so we can send them in a single "bulk" message
json_values = json.dumps([simple_stream1_values, simple_stream2_values, complex_stream_values])
