# NHilo

## What is NHilo?

NHilo is an implementation of the Hilo algorithm (from Hibernate). It aims to be a tool for generating database keys without having to rely on DBMS features like SQL Server's identity column or Oracle's sequence. The repository shouldn't be responsible for generating any information, it must be only a storage. Primary keys, even surrogate keys, must be defined in the application tier.

Here's a snippet showing how to use NHiLo. First, you need to create a factory that will create a sequence generator object for each entity you have. Then you simply call the GetKey method and it give you a brand new and unique value for use as a primary key.

```csharp
using NHilo;
// ...
var builder = new ConfigurationBuilder();
var config = builder.AddJsonFile("appsettings.json").Build();
// ...
var factory = new HiLoGeneratorFactory(config);
var generator = factory.GetKeyGenerator("myEntity");
long key = generator.GetKey();
```

In your application configuration file (appsettings.json, for example), you need specified at least one connection string. NHiLo will use the last connection string for storing its control value table. Currently we have tested NHiLo with SQL Server, SQL Server CE, MySQL, SQLite and Oracle.

For a full documentation, please go to [NHilo's wiki page](https://github.com/fabiogouw/NHilo/wiki).
