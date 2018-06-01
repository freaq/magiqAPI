# magiqAPI

> Automatic REST API for any SQLServer Database

- self-configuring, RESTful API server
- requires only a database connection string to work
- fully configurable and programmable
- no code generation



## Getting Started

- Download the zip folder.
- At the top of `Configuration/api.json`, configure the database connection string.
```shell
"connectionString": "Server=MY_SQLSERVER_PATH;Database=MY_DATABASE_NAME;Integrated Security=true;",
```
- Launch the executable.
- Enjoy full, RESTful access to your database.
	- Including partial OData support


## Features





## Why



magiqAPI is a self-configuring, RESTful API server. Give it a database connection string and launch the executable: done. magiqAPI gives you full, configurable, programmable, RESTful access to your database. 


I was tired of writing a new REST API for every new app, every new database, every new table, every new query. 
I want a RESTful web API server that automatically makes all the data in my database available for query - out-of-the-box. 
I also want that API to be configurable, top to bottom and left to right; maybe even programmable. 
I don't want a code template generator. 
It should just work.

So I started building Qupid.  


Qupid is a self-configuring web API server. 
Given a database connection, Qupid analyzes the database and automatically creates and hosts a RESTful web API that makes all database table data available, immediately. The API is defined as individual json files, each representing a single endpoint. A default API configuration is built-in, providing GET, POST, PUT, and DELETE functionality for the entire database. By modifying the json configuration files, individual APIs can be removed, restricted, or modified. The same goes for the default configuration. 


