README - PROG7311 POE Part 2, ST10091943 Daniel Pienaar
---

To build and run this application, do the following:

> Ensure you have Visual Studio (used: version 2022), SQL Server (used: Developer edition) and SQL Server Management Studio (used: version 19) installed, as well as their respective requirements. The application was made with .NET 6.0.

> Run SSMS and connect to the installed server using windows authentication. Right-click the server name (topmost value in the object explorer on the left) and select properties. Then copy the server name, which will be used in the connection string within the application.

> Open the solution "FarmCentral.sln" in visual studio. Go to the file named "appsettings.json" in the solution explorer on the right. Next to the value named "connString", replace the value next to "server=" with the name of the server you copied earlier, and save the file.

> Go to "Tools", "NuGet Package Manager", then "Package Manager Console" at the top of the window. Enter "update-database" in the console that appears and press enter. Ignore any yellow warnings concerning ProductPrice, they won't cause any issues. This process creates the database on your system.

> Click "Start Without Debugging" or CTRL + F5 to run the application.

> You will see the login page open on your web browser. Here, you can click "Populate Database" to generate some premade data to test the system. You can also register and login as a new employee. To view the details of these objects, go to "HomeController.cs" in the solution explorer and look at the method named "PopulateDB".