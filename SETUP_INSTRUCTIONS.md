# Company System with Authentication Microservice - Setup Instructions

## Overview
This project consists of two main components:
1. **AuthService** - A microservice for user authentication (runs on port 7002)
2. **CompanySystem.Web** - The main company system (runs on port 7001)

## Prerequisites
- .NET 8.0 SDK
- XAMPP with MySQL running on localhost
- Visual Studio or VS Code

## Database Setup

### 1. Start XAMPP
- Start Apache and MySQL services in XAMPP

### 2. Create Database
- Open phpMyAdmin (http://localhost/phpmyadmin)
- Create a new database named `company_user`

## Running the Applications

### Step 1: Start the Authentication Microservice
1. Open a terminal/command prompt
2. Navigate to the AuthService directory:
   ```bash
   cd AuthService
   ```
3. Restore packages and run:
   ```bash
   dotnet restore
   dotnet run
   ```
4. The Auth Service will start on https://localhost:7002
5. You can view the API documentation at https://localhost:7002/swagger

### Step 2: Start the Main Company System
1. Open another terminal/command prompt
2. Navigate to the CompanySystem.Web directory:
   ```bash
   cd CompanySystem.Web
   ```
3. Restore packages and run:
   ```bash
   dotnet restore
   dotnet run
   ```
4. The main system will start on https://localhost:7001

## Accessing the System

### Login Page
- Navigate to https://localhost:7001
- You will be automatically redirected to the login page

### Demo Credentials
The system comes with pre-seeded users:

**Administrator:**
- Email: admin@company.com
- Password: password123

**Regular User:**
- Email: user@company.com
- Password: password123

## Architecture

### Authentication Flow
1. User enters credentials on the login page
2. Main system sends credentials to Auth Microservice via HTTP API
3. Auth Microservice validates credentials against `company_user` database
4. If valid, Auth Microservice returns user information
5. Main system creates authentication cookies and redirects to dashboard

### Database Structure
- **company_user** database contains the Users table with:
  - UserId (Primary Key)
  - Email (Unique)
  - Password (BCrypt hashed)
  - FirstName, LastName
  - Role (Administrator, Employee, etc.)
  - IsActive, CreatedDate, LastLoginDate

### API Endpoints (Auth Microservice)
- `POST /api/auth/login` - Authenticate user
- `POST /api/auth/validate` - Validate user credentials
- `GET /api/auth/health` - Health check

## Features

### Login Page
- Clean, modern design with separate CSS file
- Responsive layout
- Form validation
- Loading animations
- Demo credentials display
- No signup functionality (as requested)

### Security Features
- BCrypt password hashing
- Cookie-based authentication
- HTTPS enforcement
- CORS configuration
- Input validation
- SQL injection protection via Entity Framework

## Troubleshooting

### Common Issues

1. **Database Connection Error**
   - Ensure XAMPP MySQL is running
   - Check database name is `company_user`
   - Verify connection string in appsettings.json

2. **Auth Service Connection Error**
   - Ensure Auth Service is running on port 7002
   - Check firewall settings
   - Verify AuthService:BaseUrl in main system's appsettings.json

3. **Port Conflicts**
   - If ports 7001 or 7002 are in use, update launchSettings.json
   - Update AuthService:BaseUrl accordingly

### Logs
- Check console output for detailed error messages
- Both services provide comprehensive logging

## Development Notes

### Adding New Users
Users can be added directly to the database or by extending the Auth Service with a registration endpoint.

### Extending Authentication
The microservice architecture makes it easy to:
- Add JWT token support
- Implement OAuth providers
- Add multi-factor authentication
- Scale authentication independently

### Database Migrations
The Auth Service automatically creates the database schema on first run using Entity Framework's EnsureCreated() method.

## File Structure

```
/
├── AuthService/                 # Authentication Microservice
│   ├── Controllers/
│   ├── Data/
│   ├── DTOs/
│   ├── Models/
│   ├── Services/
│   └── Program.cs
├── CompanySystem.Web/           # Main System
│   ├── Controllers/
│   ├── Services/
│   ├── ViewModels/
│   ├── Views/
│   ├── wwwroot/css/login.css    # Separate login CSS
│   └── Program.cs
└── SETUP_INSTRUCTIONS.md
```

## Support
For issues or questions, check the console logs and ensure all prerequisites are met.
