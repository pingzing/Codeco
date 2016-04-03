#Codeco
Codeco is Windows 8.1/Windows Phone 8.1/Windows 10 Universal app that securely stores and encrypts arbitrary dictionaries of values, formatted as two-column `.csv` or comma-delimited `.txt` files. I wrote it to store one-time codes for my DanskeBank account, but it can be used to store any arbitrary set of key-value pairs.

##Windows 10
Lives on its own separate `windows10` branch, which no longer contains the Windows 8, Windows Phone, or Shared projects. This will be the development branch for the forseeable future, with the older version (on `master`) only receiving updates if it's _really_ necessary.

##Building
Simply open the .sln file in Visual Studio and hit build. All dependencies should be acquired via NuGet.

Codeco requires Visual Studio 2015.
