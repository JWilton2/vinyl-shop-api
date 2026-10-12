# Vinyl Shop Web API 

A RESTful web API made in C# to manage the shop's inventory and facilitate the placement of orders for an e-commerce record shop

## Key Features

* Database Integration: Connects to a local SqLite database using Microsoft.Data.Sqlite. The tables are queried and updated using parameterised SQL strings to perform CRUD operations and protect against SQL injections
* Data Transfer Objects (DTOs) : Uses custom DTOs to separate incoming data from the internal database so that users can only submit into certain data fields when creating an order
* Memory Caching: Uses IMemoryCache to temporarily store a selected record in cache for the "Now Playing" feature, this reduces unnecessary database queries on a common route
* Global Exception Handling: Uses IExceptionHandler to catch general application errors, removing repetitive try-catch blocks and returning standard HTTP error responses instead of exposing the internal stack traces 

## Technical Stack

* Language: C#
* Framework: ASP.NET Web API
* Database: SQLite
