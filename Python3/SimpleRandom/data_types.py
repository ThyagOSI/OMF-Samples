import json

# Define our types using JSONSchema notation
simple_type = {
    "id": "simple_type",
    "description": "A simple number + timestamp object",
    "type": "object",
    "properties": {
        "IndexedDateTimeProperty": { "type": "string", "format": "date-time", "index": True },
        "NumberProperty": { "type": "number" }
    }
}

complex_type = {
    "id": "complex_type",
    "description": "A complex type including strings, ints, doubles, arrays, and nested objects",
    "type": "object",
    "properties": {
        "IndexedDateTimeProperty": { "type": "string", "format": "date-time", "index": True},
        "StringProperty": { "type": "string" },
        "DoubleProperty": { "type": "number" },
        "IntegerProperty": { "type": "integer" },
        "ObjectProperty": {
            "type": "object",
            "properties": {
                "NumberProperty": { "type": "number" },
                "IntegerArrayProperty": { "type": "array", "items": { "type": "integer" }, "maxItems": 3 },
                "NestedObjectProperty": {
                    "type": "object",
                    "properties": {
                        "NumberProperty": { "type": "number" },
                        "StringProperty": { "type": "string" }
                    }
                }
            }
        }
    }
}



# Add all the types into single json array so we can send them in a single "bulk" message
json_types = json.dumps([simple_type, complex_type])
