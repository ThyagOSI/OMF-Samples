var monitor = require('os-monitor'),
    request = require('request'),
    nconf = require('nconf'),
    seq = require('seq'),
    zlib = require('zlib');

nconf.file({ file: './config.json' });

// setup common headers
var msgHeader = {
    producertoken: nconf.get('producertoken'),
    messageformat: 'JSON'
};

// setup common request options
var options = {
    uri: nconf.get('endpoint'),
    rejectUnauthorized: nconf.get('verifyCert'),
    method: 'POST',
    headers: msgHeader
};

var streamName = 'os-monitor.' + monitor.os.hostname();

function sendType(cb){
    //register the type
    msgHeader.messagetype = 'types';
    options.body = JSON.stringify([{
	id: 'os-monitor.event',
	type: 'object',
	properties: {
	    loadavg: {
		type: 'object',
		properties: {
		    oneMinute: { type: 'number' },
		    fiveMinute: { type: 'number' },
		    fifteenMinute: { type: 'number' }
		}
	    },
	    uptime: { type: 'integer' },
	    freemem: { type: 'integer' },
	    totalmem: { type: 'integer' },
	    timestamp: {
		type: 'string',
		format: 'date-time',
		index: 'true'
	    }
	}    
    }]);
    
    request(options, function(err, res, body){
	if(err){
	    console.log('Error sending type message.');
	    throw Error(err);
	} else {
	    console.log(res.statusCode, 'Type message sent.');
	    cb();
	}
    });
}

function sendStream(cb){
    //register the stream
    msgHeader.messagetype = 'streams';
    options.body = JSON.stringify([{
	streamId: streamName,
	typeId: 'os-monitor.event'
    }]);
    
    request(options, function(err, res, body){
	if(err){
	    console.log('Error sending stream message.');
	    throw Error(err);
	} else {
	    console.log(res.statusCode, 'Stream message sent.');
	    cb();
	}
    });
}

//use seq to ensure that the monitor is not started until
//the type and stream messages are successfully sent in order
seq()
    .seq(function(){
	sendType(this);
    })
    .seq(function(){
	sendStream(this);
    })
    .seq(function(){
	monitor.start({ delay: nconf.get('interval') } );
    })
;

monitor.on('monitor', function(monitorEvent) {
    var timestring = new Date(monitorEvent.timestamp * 1000).toISOString();
    //transform into object compliant with our type
    var event = {
	streamId: streamName,
	values: [{
	    loadavg: {
		oneMinute: monitorEvent.loadavg[0],
		fiveMinute: monitorEvent.loadavg[1],
		fifteenMinute: monitorEvent.loadavg[2]
	    },
	    uptime: Math.round(monitorEvent.uptime),
	    freemem: monitorEvent.freemem,
	    totalmem: monitorEvent.totalmem,
	    timestamp: timestring
	}]
    }
    msgHeader.messagetype = 'values';
    var jsonValue = JSON.stringify([event]);
    // For such a small message, compression doesn't help much, but we're doing it here
    // for demonstration.
    msgHeader.messagecompression = 'gzip';
    zlib.gzip(jsonValue, function(err, compressed){
	options.body = compressed;
	request(options, function(err, res, body){
	    if(err){
		console.log(timestring, err, 'Error sending value message.');
	    } else {
		console.log(timestring, res.statusCode, 'Value message sent.');
	    }
	});
    });
});

//shutdown gracefully on SIGTERM
process.on('SIGTERM', function () {
    monitor.stop();
});
