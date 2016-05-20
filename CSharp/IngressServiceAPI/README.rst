
Ingress Service API
===================

The Ingress Service API is a set of helper classes and methods used to send stream based data to a service endpoint that supports the OSIsoft Messaging Format (OMF).  

Currently, the API supports three operations:

- Creating types
- Creating streams
- Sending values

These operations are exposed via the ``IngressClient`` which is responsible for formatting the stream data into OMF messages, authenticating with the Ingress Service, and sending the data to the service endpoint.

Creating Types
--------------
Types define the structure of the data that will be sent to the service.  We use `JSONSchema <http://json-schema.org/examples.html>`_ to describe types.  Types must be created and sent to the service before any streams associated with the type can be sent.  Here's an example of a type that contains an indexed date-time and a numeric value.  We give it an id of "SimpleType" which we then reference later on when creating streams.
::

    string typeSchema =
    @"{
          ""id"": ""SimpleType"",
          ""type"": ""object"",
          ""properties"": {
              ""Time"": { ""type"": ""string"", ""format"": ""date-time"", ""index"": true },
              ""Value"": { ""type"": ""number"" }
          }
      }";

    ingressClient.CreateTypes(new string[] { typeSchema });

Creating Streams
----------------
Streams define a group of typed and ordered data.  Conceptually,  you could think of a stream as relating to a sensor or a device that emits a continuous stream of data.   Since all data stored in the Ingress Service's historian is associated with a stream, this needs to be created before data can be successfully sent and stored in the historian.

Streams are created by building a ``StreamInfo`` object and passing it to the ``IngressClient``.  The ``StreamInfo`` is a tuple that consist of a streamId and it's associated typeId.  The streamId can be any unique string that you want to use to identify your stream.
::

    StreamInfo stream1 = new StreamInfo() { StreamId = "TestStream1", TypeId = "SimpleType" };

    ingressClient.CreateStreams(new StreamInfo[] { stream1 });


Sending Values
---------------
Once your type and stream is created, you can now send values associated with the stream.  Values are sent by creating a collection of ``StreamValues`` objects and then passing that to the ``IngressClient``.  ``StreamValues`` is an object that contains your streamId associated with your values, and a collection of objects that contain your values.
::

    // Create a collection of values and add your data
    var values = new List<SimpleType>();
    values.Add(new SimpleType() { Time = t1, Value = 3.14 });
    values.Add(new SimpleType() { Time = t2, Value = 6.67384 });
    values.Add(new SimpleType() { Time = t3, Value = 299792458 });

    // Package them into a StreamValues object
    var vals1 = new StreamValues() { StreamId = stream1.StreamId, Values = values };

    // Send them to the service
    ingressClient.SendValuesAsync(new StreamValues[] { vals1 });

Insuring High Throughput
------------------------
There are some practices to consider when using the ``IngressClient`` to attain the highest possible throughput.  The first is batch sending data.  All ``IngressClient`` methods accept collections.  By adding multiple objects into those collections, you allow the client to send them all at once which reduces the number of round trips to the service.  This is especially important for sending values, were you may want to send hundreds or even thousands of messages per second.  

One caveat to sending batches is that we're limited to 256kb per batch sent.  Since the ``IngressClient`` serializes data into JSON before sending to the service, the size of your batched data may be larger than anticipated and sends may fail.  If this happens, the size of the batches sent must be reduced.

Another way to increase throughput is by sending data asynchronously.  Because types must be sent before streams, and streams must be sent before values, the ``CreateTypes`` and ``CreateStreams`` methods are synchronous.  However, the ``SendValuesAsync`` method is asynchronous, allowing multiple values collections to be sent simultaneously. This removes the need to wait for the service to return before sending the next batch of values.  This is especially important to prevent throughput problems with high latency connections to the service.

Finally, the ``IngressClient`` can compress the data sent to the service if you set ``IngressClient.UseCompression = true``.  For small amounts of data this probably won't help, but for larger batches of data sent over a low bandwidth connection, this can significantly improve throughput.  Using compression can also allow batches of data to be sent that would otherwise exceed the 256kb size limit.