# DiningVsCodeNew 🍽️

An ASP.NET Core Web API backend for managing cafeteria operations at scale — including user management, meal ordering, voucher processing, and payment integrations.

---

## 🚀 Features

- ✅ JWT-based authentication and authorization
- 🍽️ RESTful APIs for users, meals, payments, vouchers
- 🔍 Built-in support for filtering/sorting with [Sieve](https://github.com/Biarity/Sieve)
- 📧 Email sending capabilities (SMTP config)
- 🔐 Role-based access control
- 🧾 Swagger UI for API documentation

---

## 📁 Project Structure

DiningVsCodeNew/
│
├── Controllers/ # API controllers
├── Models/ # Data models
├── Repositories/ # Data access logic
├── Services/ # Business logic
├── Program.cs # App entry point
├── appsettings.json # App config
└── web.config # IIS deployment config


---

## 🛠️ Setup & Run Locally

### ✅ Prerequisites

- [.NET 6 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
- SQL Server or local DB connection (if configured)
- Visual Studio / VS Code

### 🔧 Running the App

```bash
# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the app
dotnet run

##By default, the app runs at:

http://localhost:5050

## 🔒 Authentication

This API uses JWT Bearer Tokens for securing endpoints.

    Token issuer: https://localhost:7146

    Secret key: stored in appsettings.json

    To authorize API requests, include:


🌐 API Documentation

Visit Swagger UI after running the app:

http://localhost:5050/swagger

Build for Release

dotnet publish -c Release -o ./publish



Hosting on IIS

    Ensure ASP.NET Core Hosting Bundle is installed

    Configure IIS to point to your publish folder

    Use web.config for IIS integration (AspNetCoreModuleV2)



Testing

Unit testing planned with xUnit or NUnit. Add test projects using:

dotnet new xunit -n DiningVsCodeNew.Tests



For support or contributions:

    GitHub: @sanari2019

    Email: samuel.anari@yourdomain.com

📄 License

This project is licensed under the MIT License — see the LICENSE file for details.


---

