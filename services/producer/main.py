from pathlib import Path

from producer import produce_message, flush_messages


def main():
    base_dir = Path(__file__).resolve().parents[2]
    csv_path = base_dir / "data" / "activity_readings.csv"

    with open(csv_path, "r", encoding="utf-8") as file:

        next(file)

        for line in file:
            line = line.rstrip("\r\n")

            produce_message(line)

    flush_messages()

    print("All messages were sent to Kafka.")


if __name__ == "__main__":
    main()