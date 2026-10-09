using System;
using Microsoft.Data.Sqlite;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Caching.Memory;

namespace apiEcom.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VinylsController : ControllerBase
    {
        private readonly IConfiguration myconfiguration;
        private readonly string ConnectionStr;
        private readonly IMemoryCache myCache;

        public VinylsController(IConfiguration configuration, IMemoryCache cache)
        {
            myconfiguration = configuration;
            myCache = cache;

            //search for connection string that is saved in appsettings
            ConnectionStr = myconfiguration.GetConnectionString("DefaultConnection");

        }

        [HttpPost]
        public IActionResult CreateNewVinyl([FromBody] Vinyl newVinyl)
        {
            using (SqliteConnection connection = new SqliteConnection(ConnectionStr))
            {
                connection.Open();


                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                    INSERT INTO
                    Vinyls (Title, Artist, Genre, Condition, Price, Quantity)
                    VALUES ($title, $artist, $genre, $condition, $price, $quantity)";

                    command.Parameters.AddWithValue("$title", newVinyl.Title);
                    command.Parameters.AddWithValue("$artist", newVinyl.Artist);
                    command.Parameters.AddWithValue("$genre", newVinyl.Genre);
                    command.Parameters.AddWithValue("$condition", newVinyl.Condition);
                    command.Parameters.AddWithValue("$price", newVinyl.Price);
                    command.Parameters.AddWithValue("$quantity", newVinyl.Quantity);


                    command.ExecuteNonQuery();
                }
            }
            return Ok();
        }

        [HttpGet("BargainBin")]
        public IActionResult GetBargainBin()
        {
            List<Vinyl> vinylList = new List<Vinyl>();
            Random rng = new Random();
            int num = rng.Next(1, 5);

            using (SqliteConnection connection = new SqliteConnection(ConnectionStr))
            {
                connection.Open();
                using (SqliteCommand command = connection.CreateCommand())
                {
                    command.CommandText = @"
                    SELECT *
                    FROM Vinyls
                    WHERE PRICE < 10
                    ORDER BY RANDOM() LIMIT $limit";

                    command.Parameters.AddWithValue("$limit", num);

                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            vinylList.Add(new Vinyl
                            {
                                VinylID = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Artist = reader.GetString(2),
                                Genre = reader.GetString(3),
                                Condition = reader.GetString(4),
                                Price = reader.GetDouble(5),
                                Quantity = reader.GetInt32(6)
                            });
                        }
                    }
                }
            }
            return Ok(vinylList);

        }

        [HttpGet("now-playing")]
        public IActionResult GetNowPlaying()
        {
            Vinyl? currentlyPlaying = null;
            const string key = "NowPlayingSong";

            if (!myCache.TryGetValue(key, out currentlyPlaying))
            {
                using (SqliteConnection connection = new SqliteConnection(ConnectionStr))
                {
                    connection.Open();
                    using (SqliteCommand command = connection.CreateCommand())
                    {

                        command.CommandText = "SELECT * FROM Vinyls ORDER BY RANDOM() LIMIT 1";

                        using (SqliteDataReader reader = command.ExecuteReader())
                        {

                            if (reader.Read())
                            {
                                currentlyPlaying = new Vinyl
                                {
                                    VinylID = reader.GetInt32(0),
                                    Title = reader.GetString(1),
                                    Artist = reader.GetString(2),
                                    Genre = reader.GetString(3),
                                    Condition = reader.GetString(4),
                                    Price = reader.GetDouble(5),
                                    Quantity = reader.GetInt32(6)
                                };
                            }
                        }
                    }
                }



                if (currentlyPlaying != null)
                {
                    var cacheOptions = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(45));
                    myCache.Set(key, currentlyPlaying, cacheOptions);
                }
            }
            if (currentlyPlaying ==null)
            {
                return NotFound("We have no records to play");
            }

            return Ok(currentlyPlaying);
        }


        [HttpGet]
        public IActionResult GetVinyls([FromQuery] string? genre)
        {
            List<Vinyl> vinylList = new List<Vinyl>();
            Vinyl current;
            string sqlQuery = "";

            using (SqliteConnection connection = new SqliteConnection(ConnectionStr))
            {
                connection.Open();

                using (SqliteCommand command = connection.CreateCommand())
                {
                    //throw new Exception("Big error!");

                    sqlQuery = @"SELECT * FROM Vinyls";

                    if (genre != null)
                    {
                        sqlQuery += " WHERE Genre = $genre";
                        command.Parameters.AddWithValue("$genre", genre);
                    }


                    command.CommandText = sqlQuery;
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            current = new Vinyl
                            {
                                VinylID = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Artist = reader.GetString(2),
                                Genre = reader.GetString(3),
                                Condition = reader.GetString(4),
                                Price = reader.GetDouble(5),
                                Quantity = reader.GetInt32(6)
                            };


                            vinylList.Add(current);
                        }
                    }
                }
            }
            return Ok(vinylList);
        }



        [HttpPut("{id}")]
        public IActionResult UpdateVinyl(int id, [FromBody] Vinyl updatedVinyl)
        {
            using (SqliteConnection connection = new SqliteConnection(ConnectionStr))
            {
                connection.Open();

                using (SqliteCommand command = connection.CreateCommand())
                {

                    command.CommandText = @"
                    UPDATE Vinyls
                    SET Title = $title, Artist = $artist, Genre = $genre, Condition = $condition, Price = $price, Quantity = $quantity
                    WHERE VinylID = $ID";

                    command.Parameters.AddWithValue("$title", updatedVinyl.Title);
                    command.Parameters.AddWithValue("$artist", updatedVinyl.Artist);
                    command.Parameters.AddWithValue("$genre", updatedVinyl.Genre);
                    command.Parameters.AddWithValue("$condition", updatedVinyl.Condition);
                    command.Parameters.AddWithValue("$price", updatedVinyl.Price);
                    command.Parameters.AddWithValue("$quantity", updatedVinyl.Quantity);
                    command.Parameters.AddWithValue("$ID", id);
                    command.ExecuteNonQuery();
                }

            }
            return Ok();
        }
    }
}