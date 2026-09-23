from confluent_kafka import Producer
import os

conf = conf = {
    "bootstrap.servers": os.getenv(
        "KAFKA_BOOTSTRAP_SERVERS",
        "localhost:9092"
    )
}
producer = Producer(conf)
topic = 'activity-readings'

def delivery_callback(err, msg):
    if err:
        print(f'Message failed: {err}')
    else:
        print(f'Delivered to {msg.topic()} [{msg.partition()}]')

def produce_message(data):
    producer.produce(topic, value= data.encode("utf-8"), callback=delivery_callback)
    producer.poll(0)

def flush_messages():
    producer.flush()