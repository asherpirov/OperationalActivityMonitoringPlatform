def create_stations_table(connection):
    cursor = connection.cursor()

    query = """
        CREATE TABLE IF NOT EXISTS Stations (
            Id VARCHAR(50) PRIMARY KEY,
            Name VARCHAR(100),
            Sector VARCHAR(100),
            Status VARCHAR(20),
            CreatedAt DATETIME
        )
    """

    cursor.execute(query)

    connection.commit()
    cursor.close()

def insert_stations(connection, df):
    cursor = connection.cursor()

    query = """
        INSERT IGNORE INTO Stations
        (Id, Name, Sector, Status, CreatedAt)
        VALUES (%s, %s, %s, %s, NOW())
    """

    for _, row in df.iterrows():
        values = (
            row["station_id"],
            row["name"],
            row["sector"],
            row["status"]
        )

        cursor.execute(query, values)

    connection.commit()
    cursor.close()