from database import get_connection
from station_repository import create_stations_table, insert_stations
import pandas as pd
from pathlib import Path



def main():
    connection = None

    try:
        connection = get_connection()

        create_stations_table(connection)

        print("Stations table is ready.")

        base_dir = Path(__file__).resolve().parents[2]
        csv_path = base_dir / "data" / "stations.csv"

        df = pd.read_csv(csv_path)

        insert_stations(connection, df)

        print(df)

    except Exception as e:
        print(f"Error: {e}")

    finally:
        if connection and connection.is_connected():
            connection.close()
            print("Database connection closed.")


if __name__ == "__main__":
    main()