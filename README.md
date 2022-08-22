# tweeter-messages-api

An article about installing and using Kafka
https://thecloudblog.net/post/event-driven-architecture-with-apache-kafka-for-net-developers-part-1-event-producer/

1. Apache Zookeeper: The Zookeeper dependency will be removed from Kafka in the future by some vendors such as Confluent. Read the latest documentation from the vendor that you intend to use for Kafka.
2. Kafka
3. Kafdrop: Kafdrop is a popular web-based user interface for viewing Kafka topics and browsing consumer groups. It makes your Kafka cluster observable, which helps you diagnose issues and helps you with development.
4. Schema Registry: Schema Registry is a service that lives outside of your cluster and allows the developers to manage the message schemas. Kafka supports messages in Avro, JSON, and Protobuf formats, and the Schema Registry supports the storage and retrieval of versioned schemas in all those formats.