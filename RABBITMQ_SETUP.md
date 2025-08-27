# RabbitMQ Implementation Guide

## Overview
This implementation replaces the HTTP-based communication between `CompanySystem.Web` and `AuthService` with RabbitMQ message queuing for authentication requests. This provides better performance, reliability, and decoupling between services.

## Architecture

### Request/Response Pattern
- **CompanySystem.Web** sends authentication requests via RabbitMQ
- **AuthService** consumes requests, processes authentication, and sends responses back
- Uses correlation IDs to match requests with responses
- Implements timeouts and error handling

### Queue Structure
- `auth.login.request` - Main queue for authentication requests
- `auth.login.response` - Temporary per-client queues for responses
- `auth.login.dlq` - Dead letter queue for failed messages

## Prerequisites

1. **RabbitMQ Server** - Already installed at `C:\Program Files\RabbitMQ Server`
2. **MySQL Server** - For both services' databases
3. **.NET 8/9** - For running the services

## Setup Instructions

### 1. Start RabbitMQ Server
```powershell
# Navigate to RabbitMQ sbin directory
cd "C:\Program Files\RabbitMQ Server\rabbitmq_server-*\sbin"

# Start RabbitMQ service
.\rabbitmq-server.bat

# Or start as Windows service
net start RabbitMQ
```

### 2. RabbitMQ Management Interface (Optional)
```powershell
# Enable management plugin
.\rabbitmq-plugins.bat enable rabbitmq_management

# Access web interface at: http://localhost:15672
# Default credentials: guest/guest
```

### 3. Build Services
```powershell
# Build AuthService
cd AuthService
dotnet build

# Build CompanySystem.Web
cd ../CompanySystem.Web
dotnet build
```

### 4. Run Services

#### Terminal 1 - AuthService
```powershell
cd AuthService
dotnet run
```

#### Terminal 2 - CompanySystem.Web
```powershell
cd CompanySystem.Web
dotnet run
```

## Configuration

### RabbitMQ Settings
Both services use identical RabbitMQ configuration in their `appsettings.json`:

```json
{
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "guest",
    "Password": "guest",
    "VirtualHost": "/",
    "RequestTimeout": "00:00:30",
    "AuthQueues": {
      "RequestQueue": "auth.login.request",
      "ResponseQueue": "auth.login.response", 
      "DeadLetterQueue": "auth.login.dlq",
      "Durable": true,
      "AutoDelete": false,
      "Exclusive": false
    }
  }
}
```

## Testing

### 1. Health Check Endpoints
```bash
# Test CompanySystem.Web health
curl http://localhost:5000/api/health

# Test RabbitMQ authentication flow
curl http://localhost:5000/api/health/auth
```

### 2. Login Flow
1. Navigate to `http://localhost:5000`
2. Use demo credentials:
   - Admin: `admin@company.com` / `password123`
   - User: `user@company.com` / `password123`

### 3. RabbitMQ Management
- Access: `http://localhost:15672`
- Monitor queues, messages, and connections
- View message rates and queue depths

## Monitoring

### RabbitMQ Queues
- **auth.login.request**: Should show consumers (AuthService)
- **Temporary response queues**: Created per Web client
- **auth.login.dlq**: Should remain empty (failed messages)

### Logs
Both services log RabbitMQ operations:
- Connection establishment
- Message publishing/consuming
- Correlation ID tracking
- Error handling

## Troubleshooting

### Common Issues

1. **RabbitMQ Connection Failed**
   - Ensure RabbitMQ server is running
   - Check firewall settings
   - Verify credentials in appsettings.json

2. **Authentication Timeouts**
   - Check if AuthService is running
   - Verify queue configuration matches
   - Review AuthService logs for errors

3. **Queue Not Found**
   - AuthService creates queues on startup
   - Ensure AuthService starts before Web service
   - Check RabbitMQ management interface

### Debugging Steps

1. **Check RabbitMQ Status**
   ```powershell
   # Check if RabbitMQ is running
   Get-Service RabbitMQ
   
   # Check processes
   Get-Process | Where-Object {$_.ProcessName -like "*rabbit*"}
   ```

2. **View Queue Status**
   - Access RabbitMQ Management: http://localhost:15672
   - Navigate to Queues tab
   - Check message counts and consumer status

3. **Service Logs**
   - AuthService logs show message processing
   - Web service logs show request/response flow
   - Look for correlation IDs to trace requests

## Best Practices Implemented

### Reliability
- **Message Durability**: Queues survive RabbitMQ restarts
- **Dead Letter Queues**: Failed messages for investigation
- **Correlation IDs**: Request/response matching
- **Timeouts**: Prevent hanging requests

### Performance
- **Connection Pooling**: Reuse RabbitMQ connections
- **Async Processing**: Non-blocking message handling
- **Prefetch Limits**: Control message consumption rate

### Error Handling
- **Graceful Degradation**: Error responses for failures
- **Retry Logic**: Built into RabbitMQ client
- **Logging**: Comprehensive error tracking

### Security
- **Message Validation**: Input validation on both sides
- **Secure Configuration**: Configurable credentials
- **Isolation**: Separate queues per function

## Migration Notes

The implementation maintains the same `IAuthApiService` interface, so the Web application requires minimal changes. The HTTP-based service is kept as a fallback option.

To switch back to HTTP:
1. Comment out RabbitMQ service registration
2. Uncomment HTTP client registration
3. Restart CompanySystem.Web

## Performance Benefits

- **Reduced Latency**: Direct message passing vs HTTP overhead
- **Better Throughput**: Asynchronous processing
- **Improved Resilience**: Message queuing handles service outages
- **Scalability**: Easy to add multiple AuthService instances
