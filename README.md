# Company System - Training Project

## Project Overview
A comprehensive company management system built with ASP.NET Core, featuring role-based access control, employee management, department management, and content management capabilities.

## Technology Stack
- **Backend**: ASP.NET Core 9.0, C#, SQL Server, Entity Framework Core
- **Frontend**: HTML, CSS, JavaScript, MVC Pattern
- **Architecture**: 3-Layer Architecture (Web, Business, Data)

## Project Structure

### Solution Architecture
```
CompanySystem/
├── CompanySystem.Web/          # Presentation Layer (Controllers, Views, APIs)
├── CompanySystem.Business/     # Business Logic Layer (Services, Interfaces)
└── CompanySystem.Data/         # Data Access Layer (Models, DbContext, Repositories)
```

### Detailed Folder Structure

#### 1. CompanySystem.Web (Presentation Layer)
```
CompanySystem.Web/
├── Areas/                      # Role-based areas
│   ├── Admin/                  # Administrator area
│   │   ├── Controllers/
│   │   └── Views/
│   ├── HR/                     # HR area
│   │   ├── Controllers/
│   │   └── Views/
│   ├── Lead/                   # Lead area
│   │   ├── Controllers/
│   │   └── Views/
│   └── Employee/               # Employee area
│       ├── Controllers/
│       └── Views/
├── Controllers/                # Main controllers
├── Views/                      # Main views
│   ├── Shared/                 # Shared layouts and partials
│   ├── Home/                   # Home page views
│   └── Account/                # Authentication views
├── ViewModels/                 # View models for data transfer
├── Helpers/                    # Helper classes and utilities
├── Middleware/                 # Custom middleware
├── Properties/                 # Project properties
├── wwwroot/                    # Static files
│   ├── css/                    # Stylesheets
│   ├── js/                     # JavaScript files
│   ├── images/                 # Image assets
│   └── lib/                    # Library files (Bootstrap, jQuery, etc.)
├── appsettings.json            # Application configuration
├── appsettings.Development.json # Development configuration
├── Program.cs                  # Application entry point
└── CompanySystem.Web.csproj    # Project file
```

#### 2. CompanySystem.Business (Business Logic Layer)
```
CompanySystem.Business/
├── Services/                   # Business services
│   ├── Auth/                   # Authentication services
│   ├── Department/             # Department management services
│   ├── User/                   # User management services
│   ├── Notes/                  # Notes management services
│   └── CMS/                    # Content management services
├── Interfaces/                 # Service interfaces
│   ├── Auth/                   # Authentication interfaces
│   ├── Department/             # Department interfaces
│   ├── User/                   # User interfaces
│   ├── Notes/                  # Notes interfaces
│   └── CMS/                    # CMS interfaces
├── DTOs/                       # Data Transfer Objects
├── Validators/                 # Validation classes
├── Exceptions/                 # Custom exceptions
├── Extensions/                 # Extension methods
├── Utilities/                  # Utility classes
├── Class1.cs                   # Default class file
└── CompanySystem.Business.csproj # Project file
```

#### 3. CompanySystem.Data (Data Access Layer)
```
CompanySystem.Data/
├── Models/                     # Entity models
├── Data/                       # DbContext and configurations
├── Repositories/               # Repository pattern implementation
│   ├── Generic/                # Generic repository
│   └── Specific/               # Specific repositories
├── Configurations/             # Entity configurations (Fluent API)
├── Migrations/                 # Entity Framework migrations
├── SeedData/                   # Database seed data
├── Enums/                      # Enumerations
├── Class1.cs                   # Default class file
└── CompanySystem.Data.csproj   # Project file
```

### Root Solution Files
```
CompanySystem/
├── CompanySystem.sln           # Solution file
├── README.md                   # Project documentation
├── .gitignore                  # Git ignore rules
└── .history/                   # IDE history files
```

## Core Entities (Code First)
- **User**: Employee information, authentication, roles
- **Role**: User roles (Administrator, HR, Lead, Employee)
- **Department**: Department information and management
- **Notes**: Technical and behavioral notes
- **MainPageContent**: CMS content for main page

## Role-Based Access Control
1. **Administrator**: Full system access, user management, content management
2. **HR**: Employee profiles, salary management, behavioral notes
3. **Lead**: Team management, technical notes for subordinates
4. **Employee**: Self-service profile viewing

## Getting Started
1. Clone the repository
2. Open the solution in Visual Studio
3. Configure the connection string in `appsettings.json`
4. Run Entity Framework migrations
5. Seed the database with initial data
6. Build and run the application

## Project Dependencies
- Microsoft.EntityFrameworkCore.SqlServer
- Microsoft.EntityFrameworkCore.Tools
- Microsoft.Extensions.DependencyInjection
