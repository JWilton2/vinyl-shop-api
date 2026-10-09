using System;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace apiEcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
	{
        private readonly IConfiguration myconfiguration;
        private readonly string ConnectionStr;

        public OrdersController(IConfiguration configuration)
		{
            myconfiguration = configuration;


            //search for connection string that is saved in appsettings
            ConnectionStr = myconfiguration.GetConnectionString("DefaultConnection");
        }

        [HttpPost]
        public IActionResult CreateNewOrder([FromBody] CreateOrder newOrder)
        {
            using (SqliteConnection connection = new SqliteConnection(ConnectionStr))
            {
                int stockAmount = 0;
                double purchasedPrice = 0;
                
                connection.Open();

                //check sufficient quantity for any new orders

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                    SELECT Quantity, Price
                    FROM Vinyls
                    WHERE VinylID = $vinylid";

                    command.Parameters.AddWithValue("$vinylid", newOrder.VinylID);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        if (!reader.Read())
                        {
                            return Problem("Error identifying the product");

                        }
                        stockAmount = reader.GetInt32(0);
                        purchasedPrice = reader.GetDouble(1);

                    }
                    
                    

                    if (stockAmount < newOrder.Quantity)
                    {
                        return Problem("Insufficient quantity");
                    }

                }

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
					INSERT INTO
					Orders (VinylID, Quantity, CustomerEmail, PurchasePrice, OrderStatus)
					VALUES ($vinylid,$quantity, $customeremail, $purchprice, $ordstatus)";

                    command.Parameters.AddWithValue("$vinylid", newOrder.VinylID);
                    command.Parameters.AddWithValue("$quantity", newOrder.Quantity);
                    command.Parameters.AddWithValue("$customeremail", newOrder.CustomerEmail);
                    command.Parameters.AddWithValue("$purchprice", purchasedPrice);
                    command.Parameters.AddWithValue("$ordstatus", "Processing");
                    command.ExecuteNonQuery();
                }

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                    UPDATE Vinyls
                    SET Quantity = Quantity - $quantity
                    WHERE VinylID = $vinylid";

                    command.Parameters.AddWithValue("$vinylid", newOrder.VinylID);
                    command.Parameters.AddWithValue("$quantity", newOrder.Quantity);
                    command.ExecuteNonQuery();
                }

            }
            return Ok("sucessful order placed");


        }

        [HttpGet]
        public IActionResult GetOrders()
        {
            List<Order> orderList = new List<Order>();
            Order current;

            using (SqliteConnection connection = new SqliteConnection(ConnectionStr))
            {
                connection.Open();

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"SELECT * FROM Orders";

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            current = new Order { OrderID = reader.GetInt32(0),
                                                  VinylID = reader.GetInt32(1),
                                                  Quantity = reader.GetInt32(2),
                                                  CustomerEmail = reader.GetString(3),
                                                  PurchasePrice = reader.GetDouble(4),
                                                  OrderStatus = reader.GetString(5) };
                            orderList.Add(current);
                        }
                    }
                }

            }
            return Ok(orderList);
        }

        [HttpPut("id")]
        public IActionResult UpdateOrderStatus(int id, [FromBody] Order updatedOrder)
        {
            using (SqliteConnection connection = new SqliteConnection(ConnectionStr))
            {
                connection.Open();

                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                    UPDATE Orders
                    SET OrderStatus = $orderstatus
                    WHERE OrderID = $orderid";

                    command.Parameters.AddWithValue("$orderstatus", updatedOrder.OrderStatus);
                    command.Parameters.AddWithValue("$orderid", id);

                    command.ExecuteNonQuery();
                      
                }
            }
            return Ok();
        }

    }
}

