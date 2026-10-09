using System;
using Microsoft.Data.Sqlite;
namespace apiEcom
{
    public class DatabaseConnection
    {
        private static string connectionStr;


        public DatabaseConnection()
        {
            
        }

        public static void InitialiseDatabase(string connectionString)
        {
            connectionStr = connectionString;
            try
            {
                using (SqliteConnection connection = new SqliteConnection(connectionStr))
                {
                    connection.Open();

                    string createVinylsTable = @"CREATE TABLE IF NOT EXISTS Vinyls(
                                    VinylID INTEGER PRIMARY KEY AUTOINCREMENT, 
                                    Title TEXT NOT NULL, 
                                    Artist TEXT NOT NULL, 
                                    Genre TEXT NOT NULL, 
                                    Condition TEXT NOT NULL, 
                                    Price REAL NOT NULL, 
                                    Quantity INTEGER NOT NULL);";

                    string createOrdersTable = @"CREATE TABLE IF NOT EXISTS Orders(
                                    OrderID INTEGER PRIMARY KEY AUTOINCREMENT, 
                                    VinylID INTEGER NOT NULL, 
                                    Quantity INTEGER NOT NULL, 
                                    CustomerEmail TEXT NOT NULL,
                                    PurchasePrice REAL NOT NULL,
                                    OrderStatus TEXT NOT NULL,
                                    FOREIGN KEY(VinylID) REFERENCES Vinyls(VinylID));";


                    using (SqliteCommand command = new SqliteCommand(createVinylsTable, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                    using (SqliteCommand command = new SqliteCommand(createOrdersTable, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    Console.Write("Sucessfully created");
                }
            }

            catch (Exception e)
            {
                Console.Write($"Error: {e.Message}");
            }

            
        }


    }
}

