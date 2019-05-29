# CSV Match Parser for TunaWithBacon

## Description

This application reads Skullgirls matchs from a CSV file and add them into the [TunaWithBacon](tunawithbeacon.com) database.

## Requirements and usage:

1. Make sure to have the [config.xml](config.xml) in the same folder as the program.
2. Make sure you filled the CSV and the config.xml with valid data.
3. Make sure that the computer on which you run the program has acces to the database.
4. If you are using a SQL Server database, put "SQLServer" in the config.xml on the line "DatabaseType" and make sure that your database allows SQL Authentication mode, because it does not work with Windows authentication.
5. If you are using a MySQL databse, put "MySQL" in the config.xml on the line "DatabaseType". To be able to compile the application, you need to add the [.NET MySQL Connector Library](https://dev.mysql.com/downloads/connector/net/) to your Visual Studio.
6. Start TWB_Parser.exe

## Files format

### CSV Format:

`Event,Date,Region,Netplay,Version,P1Name,P1Char1,P1Char2,P1Char3,P2Name,P2Char1,P2Char2,P2Char3,URL`

* The `Date` format is YYYY-MM-DD.
* The `Netplay` is 0 or 1, matching Offline and Online.
* For more information about `PXCharX`, check [this file](TWB_Parser/CharEnum.cs).

### config.xml Information:

* CSVMatchsFilename : The path or filename of the CSV file.
* DatabaseType      : "SQLServer" or "MySQL" depending on the database you are using.
* ServerAddress     : The name/address of the server where the database is.
* PortMySQL         : Port of the MySQL database address. Useless if you are using SQL Server.
* DatabaseName      : The name of the database.
* DatabaseUsername  : The username of the account connecting to the database through SQL Authentication.
* DatabasePassword  : The password of the account connecting to the database through SQL Authentication.

## Behavior

The application will insert matches into the `matches` table and the groups into the `groups` table.

A groupe is the unique combination of `date/region/netplay/event/version`. For each group an unique ID is generated with `Guid.NewGuid()`. This ID will be inserted in both tables as a match must have a group ID.

The program does not keep track of the groups it encoutered while reading the CSV. It will create a new one everytime it detects a new `date/region/netplay/event/version` combination.

You must put the matchs in the right order in the CSV as they will be inserted in this order. 

## License and Maintainers

This software is a free software that is under the [MIT license](LICENSE) and is maintained by Servan CHARLOT ([@Servan42](https://twitter.com/servan42)).

[TunaWithBacon](tunawithbeacon.com) is a website created and maintained by [Gelato](https://twitter.com/gelatosgc).