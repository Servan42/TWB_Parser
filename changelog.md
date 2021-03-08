# TWB_Parser Changelog

## Version 1.2.0 | 2021-03-08

+ Adding an enum entry for Annie: "AN".
+ Adding a version check: "Annie Patch" & "Annie Patch Beta".
+ Changing the config management. Switched from "config.txt" to "TWB_Parser.exe.config" to respect C# standards. 
+ Adding a warpup of every enumtypes to make the CSV redaction easier: [List_of_CharactersCode_Regions_Versions](List_of_CharactersCode_Regions_Versions.md).

## Version 1.1.1 | 2019-08-18

+ Adding CSV data checking on: Empty or white space member, Region, Version, Characters acronyms. (Were already there member numbers and header checking). 
+ Correcting the fact that when the CSV file is incorrect at some point, the n first correct lines were still inserted into the database.
- Removing the stacktrace from the error log, keeping only the message.

## Version 1.1.0 | 2019-05-28

+ Adding compatibility with MySQL. SQL Server is still usable.
+ Updated the format of config.xml to choose between MySQL and SQL Server.
+ Updated the README.md
+ Added a changelog.md (meta).

## Version 1.0.0 | 2019-05-26

+ Creation