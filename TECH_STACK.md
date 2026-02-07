# Task Management System - Technical Documentation

## Core Requirements

### Backend (.NET Core)

**Framework:**
- .NET 8.0
- ASP.NET Core Web API

**Architecture:**
- Clean Architecture (4 layers: Domain, Application, Infrastructure, API)
- Repository Pattern
- Dependency Injection

**Database:**
- SQL Server
- Entity Framework Core with Code-First migrations
- Seeded lookup tables for statuses and priorities

**Libraries:**
- FluentValidation - Request validation
- AutoMapper - DTO mapping
- Serilog - Logging
- Swashbuckle - Swagger/OpenAPI documentation

**API Design:**
- RESTful endpoints for CRUD operations
- JWT Bearer authentication
- Request/Response DTOs
- Proper HTTP status codes

---

### Frontend (React)

**Framework:**
- React 19.2.0
- Vite 7.2.4 (build tool)

**State Management:**
- React Context API
- Custom hooks

**Routing & HTTP:**
- React Router v7.13.0
- Axios with interceptors

**UI:**
- Responsive CSS (mobile, tablet, desktop)
- Kanban board layout
- Custom styling (no UI framework)

---

## Bonus Features Implemented

### Authentication
- JWT tokens for stateless authentication
- ASP.NET Core Identity for user management
- Password hashing and validation
- Protected API endpoints with [Authorize] attribute
- Token persistence in localStorage
- User-scoped data access

### Drag-and-Drop Task Sorting
- @hello-pangea/dnd library
- Drag tasks between status lanes
- Optimistic UI updates with error rollback
- Visual feedback (cursor states, animations, drop zones)
- Touch support for mobile
- Keyboard accessibility

### Task Prioritization
- Three priority levels: Low, Medium, High
- Visual badges on task cards
- Filter tasks by priority
- Database-backed with lookup table

### Containerization
- Docker Compose orchestration
- Containers: SQL Server, Backend API, Frontend
- Environment-based configuration
- Volume persistence for database

---

## Architecture Details

### Backend Structure
```
TaskManager.Domain       - Core entities, enums, business rules
TaskManager.Application  - Services, DTOs, validators, mappings
TaskManager.Infrastructure - EF Core, repositories, database config
TaskManager.API          - Controllers, middleware, startup
```

### Database Schema
- Normalized relational design
- GUID primary keys
- Foreign key constraints
- Lookup tables for extensibility (TaskStatusEntity, TaskPriorityEntity)
- Audit fields (CreatedAt, UpdatedAt)

### Frontend Structure
```
pages/       - Route components (Tasks, Login, Register)
components/  - Reusable UI (TaskCard, TaskForm, TaskFilter, PrivateRoute)
context/     - State management (AuthContext, TaskContext)
services/    - API integration (taskService, authService, api)
```

### API Endpoints
- POST /api/auth/register
- POST /api/auth/login
- GET /api/tasks
- GET /api/tasks/{id}
- POST /api/tasks
- PUT /api/tasks/{id}
- DELETE /api/tasks/{id}
- GET /api/tasks/statuses
- GET /api/tasks/priorities

---

## Development Tools

**Version Control:**
- Git with feature branches
- GitHub repository

**API Documentation:**
- Swagger UI at /swagger
- OpenAPI specification

**Development:**
- Backend: dotnet run with hot reload
- Frontend: Vite dev server with HMR
- HTTPS support (self-signed certificate)

---

## Project Scope

**Completed:**
- Full CRUD operations for tasks
- User authentication and authorization
- Responsive Kanban board interface
- Drag-and-drop functionality
- Task filtering and search
- Priority management
- Docker containerization
- API documentation

**Not Implemented:**
- Unit tests (architecture supports it, not completed due to time)
