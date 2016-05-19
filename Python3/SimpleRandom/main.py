# Import packages
import sys, argparse, json, random, time, platform, requests, gzip
from datetime import datetime

# Import test data modules
import data_types, data_streams, data_values

parser = argparse.ArgumentParser()
parser.add_argument("endpoint")
parser.add_argument("producertoken")
parser.add_argument("-n", "--no_verify", help="Do not verify SSL certificates. May be required to send messages to the Connector Relay.",
                    action="store_true")

args = parser.parse_args()

# Address of the destination endpoint
# Should be of the form http://<host/ip>:<port>/ingress/messages for Connector Relay endpoints
relay_url = args.endpoint

# If sending messages to a Connector Relay, do not verify HTTPS SSL certificates
verifySSL = not args.no_verify

# Send the types message
msg_header = {'producertoken': args.producertoken, 'messagetype': 'Types', 'messageformat': 'JSON'}
response = requests.post(relay_url, headers=msg_header, data=data_types.json_types, verify=verifySSL)
print('Response from relay from types message: {0} {1}'.format(response.status_code, response.text))

# Send the streams message
msg_header = {'producertoken': args.producertoken, 'messagetype': 'Streams', 'messageformat': 'JSON'}
response = requests.post(relay_url, headers=msg_header, data=data_streams.json_streams, verify=verifySSL)
print('Response from relay from streams message: {0} {1}'.format(response.status_code, response.text))

# Send the values message
msg_header = {'producertoken': args.producertoken, 'messagetype': 'Values', 'messageformat': 'JSON'}
response = requests.post(relay_url, headers=msg_header, data=data_values.json_values, verify=verifySSL)
print('Response from relay from values message: {0} {1}'.format(response.status_code, response.text))

# We're going to send the rest our data compressed, so modify the headers to let the
# service know about the compression.  For such small value messages, compression doesn't 
# save us much space, but we're doing it here for demonstration sake.
msg_header['messagecompression'] = 'gzip'

# Loop indefinitely, sending random events conforming to the value schema.
while True:
    # The event representing the data
    event = {'IndexedDateTimeProperty': datetime.utcnow().isoformat() + 'Z',
             'NumberProperty': random.random() * 1000}

    # Package the event into the "values" message format, which is an array of tuples
    # where each tuple contains the stream ID and a collection of events for that stream
    stream_value = [{'streamId': 'simple_stream1', 'values': [event]}]
    json_stream_value = json.dumps(stream_value)

    # compress the data before sending
    gzipped = gzip.compress(bytes(json_stream_value, 'utf-8'))

    # Send to the relay
    response = requests.post(relay_url, headers=msg_header, data=gzipped, verify=verifySSL)
    print('Response from relay from values message: {0} {1}'.format(response.status_code, response.text))

    time.sleep(1)
