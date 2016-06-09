# sysinfo.js

This sample demonstrates how to construct and send OMF messages from a Node.js application. It uses the [os-monitor](https://www.npmjs.com/package/os-monitor) package to capture basic memory and CPU statistics from the host system and generates corresponding OMF type, stream, and value messages for ingestion into an OMF-compliant endpoint. 

## Setup

1. Create a local copy of the Git repo.
2. Install the `os-monitor`, `request`, `seq`, and `nconf` packages:

   ```javascript
   npm install os-monitor request seq nconf
   ```
3. Modify the config.json file to set the appropriate target endpoint and producertoken. If the endpoint is a PI Connector Relay, set `verifyCert` to `false`.
4. Run the application:

   ```javascript
   node sysinfo.js
   ```
