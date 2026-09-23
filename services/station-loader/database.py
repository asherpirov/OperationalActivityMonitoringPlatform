import os
import mysql.connector
from dotenv import load_dotenv


load_dotenv("../../.env")


def get_connection():
    connection = mysql.connector.connect(
        host = os.getenv("MYSQL_HOST", "localhost"),
        port = 3306,
        user = "root",
        password = os.getenv("MYSQL_ROOT_PASSWORD"),
        database = os.getenv("MYSQL_DATABASE")
    )
    return connection

connection = get_connection()

if connection.is_connected():
    print("Connected to MySQL successfully")

connection.close()