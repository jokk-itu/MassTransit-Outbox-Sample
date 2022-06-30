# MassTransit-Outbox-Sample

Testing the new Transactional Outbox from the MassTransit framework.

## Introduction

The project is created to test out the new Outbox implementation in MassTransit.
It uses EF Core as abstraction on MSSQL to handle the outbox behaviour.
It has been setup with both outbox to eliminate dual write behaviour, and inbox to enable idempotent consumers.

To test the behaviour, four endpoints are exposed in an API.

```
/publish/outbox/ef
/send/outbox/ef
/publish
/send
```

The outbox endpoints use the outbox behaviour to send/publish messages to RabbitMQ, and then inbox behaviour for the consumer.
The others simply send and publish events directly to RabbitMQ without outbox/inbox behaviour.

## How to run

The project serves the system using docker-compose, and uses an API, MSSQL and RabbitMQ.

It can be run using the commandline tool -> docker-compose up

Or through Visual Studio, since there is an accompanying docker-compose.dcproj file.
Just set the docker-compose project as starting project in Visual Studio, and you are good to go.

Navigate to the API on http://localhost:5000/swagger/index.html
