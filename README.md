# User Notifications Manager Service

## Overview
This service allows managing user notification preferences and sending notifications based on those preferences. It supports sending notifications via email and SMS, with rate limiting, authentication, and structured error handling built-in.

## Features
- Manage user notification preferences.
- Send notifications based on user preferences.
- Rate limiting for API requests.
- Retry mechanism for failed notifications.
- Health checks and structured logging.
- **JWT-based authentication and authorization.**
- **Global exception handling middleware for error management.**
- **GitHub Actions CI/CD pipeline integration.**
- Containerized deployment with Docker and Docker Compose.
- Preloaded user data for testing.

---

## **Project Structure**
```
/UserNotificationsManager
├── UserNotificationsManager.sln       # Solution file
├── src/
│   ├── NotificationManager.API/        # Main API project
│   │   ├── Controllers/
│   │   ├── Models/
│   │   ├── Services/
│   │   ├── Repositories/
│   │   ├── Middleware/                 # Exception handling middleware
│   │   ├── Configurations/
│   │   ├── Program.cs
│   │   ├── appsettings.json
│   ├── NotificationManager.Tests/       # Unit tests

├── Dockerfile                           # Docker setup
├── docker-compose.yml                    # Multi-container setup
├── .github/workflows/ci.yml              # GitHub Actions pipeline
├── .gitignore                            # Git ignore settings
├── README.md                             # Documentation
```

---

## **Installation & Setup**
### **Prerequisites**
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Docker](https://www.docker.com/)
- [Postman](https://www.postman.com/) for API testing

### **Running Locally**
1. Clone the repository:
   ```sh
   git clone <repo_url>
   cd UserNotificationsManager
   ```
2. Build and run the application:
   ```sh
   dotnet build
   dotnet run --project src/NotificationManager.API
   ```
3. The API will be accessible at `http://localhost:8080`

### **Running with Docker**
1. Build and run the containers:
   ```sh
   docker-compose up --build
   ```
2. Access the API at `http://localhost:8080`

---

## **Preloaded User Data**
The application starts with preloaded user data:
```json
[
  {
    "userId": 1,
    "email": "ironman@avengers.com",
    "telephone": "+123456789",
    "preferences": { "email": true, "sms": true }
  },
  {
    "userId": 2,
    "email": "loki@avengers.com",
    "telephone": "+123456788",
    "preferences": { "email": true, "sms": false }
  }
]
```

---

## **API Endpoints**
### **1. Authentication**
**POST** `/api/auth/login`
#### Request Body:
```json
{
  "username": "admin",
  "password": "password123"
}
```
#### Response:
```json
{
  "Token": "<JWT_TOKEN>"
}
```
**Note:** Use the returned token to access protected endpoints.

---

### **2. Send Notification**
**POST** `/api/notifications/send`
#### Request Headers:
```sh
Authorization: Bearer <JWT_TOKEN>
```
#### Request Body:
```json
{
  "userId": 1,
  "message": "Hello, this is your notification!"
}
```
#### Response:
```json
{
  "status": "sent",
  "message": "Notification sent successfully."
}
```

---

### **3. Edit User Preferences**
**PUT** `/api/users/preferences`
#### Request Headers:
```sh
Authorization: Bearer <JWT_TOKEN>
```
#### Request Body:
```json
{
  "email": "user@example.com",
  "preferences": { "email": true, "sms": false }
}
```
#### Response:
```json
{
  "status": "updated",
  "message": "User preferences updated."
}
```

---

## **Sending Emails and SMS**
- Emails are sent via a mock email service at `http://notification-service:5001/send-email`.
- SMS messages are sent via a mock SMS service at `http://notification-service:5001/send-sms`.

### **Example cURL Requests**
#### **Send Email**
```sh
curl -X POST http://localhost:5001/send-email -H "Content-Type: application/json" -d '{"email": "ironman@avengers.com", "message": "Hello Ironman!"}'
```

#### **Send SMS**
```sh
curl -X POST http://localhost:5001/send-sms -H "Content-Type: application/json" -d '{"telephone": "+123456789", "message": "Hello Ironman!"}'
```

---

## **Testing**
### **Unit Tests**
Run the tests using:
```sh
cd src/NotificationManager.Tests
dotnet test
```
### **API Testing**
Use **Postman** or **cURL**:
```sh
curl -X POST http://localhost:8080/api/notifications/send -H "Authorization: Bearer <token>" -H "Content-Type: application/json" -d '{ "userId": 1, "message": "Hello!" }'
```

---

## **Security**
- **JWT Authentication** is required for API access.
- **Global Exception Handling** middleware ensures structured error responses.
- **API rate limiting** is enabled to prevent abuse.

---

## **GitHub Actions - CI/CD**
### **Adding GitHub Actions Pipeline**
The project uses **GitHub Actions** for **automated testing & build verification**.

📌 **Workflow file location:** `.github/workflows/ci.yml`

### **GitHub Actions Configuration**
```yaml
name: CI Pipeline

on:
  push:
    branches:
      - main
      - develop
  pull_request:
    branches:
      - main

jobs:
  build:
    runs-on: ubuntu-latest

    steps:
      - name: Checkout Repository
        uses: actions/checkout@v3

      - name: Setup .NET
        uses: actions/setup-dotnet@v3
        with:
          dotnet-version: '8.0.x'

      - name: Restore Dependencies
        run: dotnet restore

      - name: Build Application
        run: dotnet build --no-restore

      - name: Run Unit Tests
        run: dotnet test --no-build --verbosity normal
```
### **What This Pipeline Does**
✅ **Runs on every `push` and `pull_request` to `main` or `develop`.**  
✅ **Checks out the repository.**  
✅ **Sets up .NET 8 in GitHub Actions.**  
✅ **Restores dependencies and builds the project.**  
✅ **Runs unit tests for verification.**  

### **To Enable CI/CD in GitHub**
1. **Commit & push your changes** to GitHub.
2. **Check the "Actions" tab in GitHub** for build status.
3. If all tests pass, the code is ready for deployment.

---

## **Deployment**
For production, use:
```sh
docker-compose -f docker-compose.prod.yml up --build -d
```

---

## **Health Check**
Check service health at:
```sh
http://localhost:8080/health
```

---

## **License**
MIT License
