# Qupid

I was tired of writing a new REST API for every new app, every new database, every new table, every new query. 
I want a RESTful web API server that automatically makes all the data in my database available for query - out-of-the-box. 
I also want that API to be configurable, top to bottom and left to right; maybe even programmable. 
I don't want a code template generator. 
It should just work.

So I started building Qupid.  


Qupid is a self-configuring web API server. 
Given a database connection, Qupid analyzes the database and automatically creates and hosts a RESTful web API that makes all database table data available, immediately. The API is defined as individual json files, each representing a single endpoint. A default API configuration is built-in, providing GET, POST, PUT, and DELETE functionality for the entire database. By modifying the json configuration files, individual APIs can be removed, restricted, or modified. The same goes for the default configuration. 



## Getting Started

Qupid is implemented in .NET using ASP.NET Core. So you'll need VisualStudio or something similar to build the bits. 

#### Configuration/api.json
At the top of the file, set the connectionString property.

#### Properties/launchsettings.json
The file  includes configurations to run the server as a standalone console app or using IIS/IISExpress.

Run the thing.
