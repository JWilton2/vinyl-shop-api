# Vinyl Shop Web API 

A RESTful web API made in C# to manage the shop's inventory and facilitate the placement of orders for an e-commerce record shop

## Project Overview
* Inventory Browsing and Filtering: Users can fetch all records, filter results specifically by genre or select the "Bargain Bin" to automatically retrieve a random selection of vinyls that cost under £10.
* Order Processing: Customers can securely create new orders. The API automatically verifies sufficient stock exists before confirming the purchase and deducting the quantity from the database.
* Admin Inventory Management: Authenticated endpoints allow store owners to add new vinyls to the database, update existing stock levels and pricing or modify the shipping status of existing orders.
* "Now Playing" Feature: Simulates a physical record player by fetching a random vinyl from the database to display on the storefront, caching the result to optimise time.

## Key Features

* Database Integration: Connects to a local SqLite database using Microsoft.Data.Sqlite. The tables are queried and updated using parameterised SQL strings to perform CRUD operations and protect against SQL injections
* Data Transfer Objects (DTOs) : Uses custom DTOs to separate incoming data from the internal database so that users can only submit into certain data fields when creating an order
* Memory Caching: Uses IMemoryCache to temporarily store a selected record in cache for the "Now Playing" feature, this reduces unnecessary database queries on a common route
* Global Exception Handling: Uses IExceptionHandler to catch general application errors, removing repetitive try-catch blocks and returning standard HTTP error responses instead of exposing the internal stack traces 

## Technical Stack

* Language: C#
* Framework: ASP.NET Web API
* Database: SQLite
