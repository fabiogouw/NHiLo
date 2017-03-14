# NHilo

Documentation on using NHilo is available at https://nhilo.codeplex.com/.

## What is NHilo?

NHilo is an implementation of the Hilo algorithm. It aims to be a tool for generating database keys without having to rely on DBMS features like SQL Server's identity column. The repository shouldn't be responsible for generating any information, it must be only a storage. Primary keys, even surrogate keys, must be defined in the application tier.

