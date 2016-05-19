# A collection of stream IDs and their associated types

import json

streams = [
	{"streamId": "complex_stream", "typeId": "complex_type"},
	{"streamId": "simple_stream1", "typeId": "simple_type"},
	{"streamId": "simple_stream2", "typeId": "simple_type"},
]

json_streams = json.dumps(streams)