# How to Run the Task Management System

## Prerequisites

Make sure you have the following installed:
- .NET 8.0 SDK
- Node.js (v18 or higher)
- SQL Server (or Docker for containerized setup)
- Git

## Option 1: Running Locally

### 1. Clone the Repository
```bash
git clone <repository-url>
cd TaskManagementSystem
```

### 2. Setup the Database

Update the connection string in `TaskManager.API/appsettings.json` if needed:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=TaskManagerDb;Trusted_Connection=True;TrustServerCertificate=True"
}
```

Apply database migrations:
```bash
cd TaskManager.API
dotnet ef database update
```

### 3. Start the Backend

```bash
cd TaskManager.API
dotnet run
```

The API will be available at:
- HTTPS: `https://localhost:7104`
- HTTP: `http://localhost:5000`
- Swagger: `https://localhost:7104/swagger`

### 4. Start the Frontend

Open a new terminal:
```bash
cd taskmanager-frontend
npm install
npm run dev
```

The frontend will be available at `http://localhost:5173`

### 5. Access the Application

Open your browser and navigate to `http://localhost:5173`

You can:
- Register a new account
- Login with your credentials
- Create, edit, delete tasks
- Drag tasks between status lanes
- Filter and search tasks

---

## Option 2: Running with Docker

### 1. Start All Services

From the project root directory:
```bash
docker-compose up -d
```

This will start:
- SQL Server container (database)
- Backend API container
- Frontend container

### 2. Wait for Services to Start

Give it about 30 seconds for all services to initialize.

### 3. Access the Application

- Frontend: `http://localhost:3000` (or configured port)
- Backend API: `https://localhost:7104`
- Swagger: `https://localhost:7104/swagger`

### 4. Stop Services

```bash
docker-compose down
```

To remove volumes as well:
```bash
docker-compose down -v
```

---

## API Documentation

While the backend is running, you can access the Swagger documentation at:
`https://localhost:7104/swagger`

This provides interactive API documentation where you can test all endpoints.

---

## Default Test User

After registration, you can create your own account. Each user only sees their own tasks.

Password requirements:
- Minimum 6 characters
- At least one uppercase letter
- At least one lowercase letter
- At least one number
- At least one special character

---

## Troubleshooting

**Backend won't start:**
- Check SQL Server is running
- Verify connection string in appsettings.json
- Run database migrations

**Frontend won't start:**
- Delete node_modules and package-lock.json
- Run `npm install` again
- Check that backend is running

**CORS errors:**
- Verify backend is running on the correct port
- Check VITE_API_URL in frontend/.env matches backend URL

**Database errors:**
- Make sure migrations are applied: `dotnet ef database update`
- Check SQL Server connection

**Docker issues:**
- Make sure Docker is running
- Check container logs: `docker logs <container-name>`
- Rebuild images: `docker-compose build --no-cache`
