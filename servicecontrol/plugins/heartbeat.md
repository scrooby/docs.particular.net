---
title: Heartbeat Plugin
summary: Use the Heartbeat plugin to monitor the health of the endpoints
reviewed: 2016-11-15
component: Heartbeats
tags:
 - ServiceControl
 - Heartbeat
related:
 - servicepulse/intro-endpoints-heartbeats
redirects:
 - servicecontrol/heartbeat-configuration
---

The Heartbeat plugin enables endpoint health monitoring in ServicePulse. It sends heartbeat messages from the endpoint to the ServiceControl queue. These messages are sent every 10 seconds (by default).

An endpoint that is marked for monitoring (by ServicePulse) will be expected to send a heartbeat message within the specified time interval. As long as a monitored endpoint sends heartbeat messages, it is marked as "active". Marking an endpoint as active means it is able to properly and periodically send messages using the endpoint-defined transport.

Note that even if an endpoint is able to send heartbeat messages and it is marked as "active", other failures may occur within the endpoint and its host that may prevent it from performing as expected. For example, the endpoint may not be able to process incoming messages, or it may be able to send messages to the ServiceControl queue but not to another queue. To monitor and get alerts for such cases, develop a custom check using the CustomChecks plugin.

If a heartbeat message is not received by ServiceControl from an endpoint, that endpoint is marked as "inactive".

An inactive endpoint indicates that there is a failure in the communication path between ServiceControl and the monitored endpoint. For example, such failures may be caused by a failure of the endpoint itself, a communication failure in the transport, or when ServiceControl is unable to receive and process the heartbeat messages sent by the endpoint.

NOTE: It is essential to deploy this plugin to the endpoint in production for ServicePulse to be able to monitor the endpoint.

WARNING: The Heartbeat plugin does not support [Send-Only endpoints](/nservicebus/hosting/#self-hosting-send-only-hosting).


### Deprecated NuGet

If are using the older version of the plugin, namely **ServiceControl.Plugin.Heartbeat** remove the package and replace it with the appropriate plugin based on the NServiceBus version. This package has been deprecated and unlisted.


## Configuration


partial: queue


### Heartbeat Interval

ServiceControl heartbeats are sent, by the plugin, at a predefined interval of 10 seconds. The interval value can be overridden on a per endpoint basis adding the following application setting to the endpoint configuration file:

snippet: heartbeatsIntervalConfig

Where the value is convertible to a `TimeSpan` value. The above sample is setting the endpoint heartbeat interval to 30 seconds.


partial: intervalCode


When configuring heartbeat interval, ensure ServiceControl setting [`HeartbeatGracePeriod`](/servicecontrol/creating-config-file.md#plugin-specific-servicecontrolheartbeatgraceperiod) is greater than the heartbeat interval.


### Time-To-Live (TTL)

When the plugin sends heartbeat messages, the default TTL is fixed to four times the configured value of the Heartbeat interval.

TTL is now configurable, as of Version 1.1.0

Add the app setting in app.config as shown to configure the TTL to a custom value instead of the default value based on heartbeat interval. Provide the timespan string for the value as shown. In this example, a heartbeat message will be sent every 30 seconds and the TTL for the heartbeat message is 3 minutes.

snippet: heartbeatsTtlConfig

Note: To enable the change the endpoint needs to be restarted.

partial: ttlCode


partial: disable


## Expired heartbeat messages forwarded to Dead letter queue

Heartbeat messages have a time to be received (TTBR) set based on the TTL value. If ServiceControl does not consume the heartbeat messages before the TTBR expires then transports that support a dead letter queue (DLQ) will move these messages to the DLQ. This can happen when ServiceControl is stopped or very busy. The dead letter queue needs to be monitored and cleaned up.


### MSMQ

If this dead letter queue behavior is not required then it can disabled. This frequently happens with MSMQ as NServiceBus configures the usage of the DLQ by default. Read [MSMQ connection strings](/nservicebus/msmq/connection-strings.md) on how to disabled dead letter queue usage.
