# Arbox Employee Management System – .NET 8 MVC
Run with Docker or Visual Studio (sln included).

## Run in Visual Studio (recommended for Windows)
1. Open `ArboxEmployeeMS.sln` in VS 2022 (17.10+)
2. Set configuration to **Debug**.
3. Press **F5** (IIS Express or Kestrel). It will launch to the dashboard.
   - Profiles available: **IIS Express**, **http**, **https**.

## Run via CLI
```bash
dotnet run
```

## Docker
```bash
docker build -t arbox-ems .
docker run --rm -p 8080:8080 -v %cd%\App_Data:/app/App_Data -v %cd%\Logs:/app/Logs arbox-ems
```
Open http://localhost:8080
