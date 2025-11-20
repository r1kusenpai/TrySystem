# Installing SQLite Package

The project requires System.Data.SQLite package to be installed. Follow these steps:

## Option 1: Using Visual Studio Package Manager Console

1. Open Visual Studio
2. Go to **Tools** → **NuGet Package Manager** → **Package Manager Console**
3. Run this command:
   ```
   Install-Package System.Data.SQLite.Core -Version 1.0.118.0
   ```

## Option 2: Using Visual Studio NuGet Package Manager UI

1. Right-click on the **TrySystem** project in Solution Explorer
2. Select **Manage NuGet Packages...**
3. Click on **Browse** tab
4. Search for **System.Data.SQLite.Core**
5. Select version **1.0.118.0**
6. Click **Install**

## Option 3: Using Command Line (if NuGet CLI is installed)

Navigate to the solution directory and run:
```
nuget install System.Data.SQLite.Core -Version 1.0.118.0 -OutputDirectory packages
```

After installing the package, rebuild the solution and the errors should be resolved.

