from pathlib import Path
from producer import produce_message, flush_messages
import csv
import json


def main():
    base_dir = Path(__file__).resolve().parent
    csv_path = base_dir / "data" / "activity_readings.csv"

    with open(csv_path, "r", encoding="utf-8") as file:

        reader = csv.DictReader(file)

        for row in reader:
            message = json.dumps(row)

            produce_message(message)

    flush_messages()

    print("All messages were sent to Kafka.")


if __name__ == "__main__":
    main()