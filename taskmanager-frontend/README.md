# Task Manager - Frontend

A modern React-based frontend for the Task Management System, built with Vite, React Router, and Context API for state management.

## Features

- User authentication (Login/Register)
- CRUD operations for tasks
- Task filtering by status and priority
- Real-time task management
- Responsive design for all devices
- Clean and intuitive UI

## Tech Stack

- **React** - UI library
- **Vite** - Build tool and dev server
- **React Router** - Client-side routing
- **Context API** - State management
- **Axios** - HTTP client for API calls
- **CSS3** - Styling with responsive design

## Prerequisites

- Node.js (v16 or higher)
- npm or yarn
- Backend API running (see backend documentation)

## Installation

1. Install dependencies:
```bash
npm install
```

2. Configure environment variables:
```bash
cp .env.example .env
```

Edit `.env` and set the API URL:
```
VITE_API_URL=https://localhost:7001/api
```

## Running the Application

### Development Mode

Start the development server:
```bash
npm run dev
```

The application will be available at `http://localhost:5173`

### Production Build

Build the application:
```bash
npm run build
```

Preview the production build:
```bash
npm run preview
```

## Project Structure

```
src/
├── components/          # Reusable React components
│   ├── PrivateRoute.jsx    # Protected route wrapper
│   ├── TaskCard.jsx        # Task card component
│   ├── TaskFilter.jsx      # Filter controls
│   └── TaskForm.jsx        # Task create/edit form
├── context/            # React Context providers
│   ├── AuthContext.jsx     # Authentication state
│   └── TaskContext.jsx     # Task management state
├── pages/              # Page components
│   ├── Login.jsx          # Login page
│   ├── Register.jsx       # Registration page
│   └── Tasks.jsx          # Main tasks page
├── services/           # API services
│   ├── api.js             # Axios instance & interceptors
│   ├── authService.js     # Authentication API calls
│   └── taskService.js     # Task API calls
├── App.jsx             # Main App component with routing
└── main.jsx            # Application entry point
```

## Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build
- `npm run lint` - Run ESLint

## Features in Detail

### Authentication
- Login with username and password
- Register new user account
- JWT token-based authentication
- Automatic token management and refresh

### Task Management
- Create new tasks with title, description, status, priority, and due date
- Edit existing tasks
- Delete tasks with confirmation
- View all tasks in a grid layout

### Filtering
- Filter tasks by status (Pending, In Progress, Completed)
- Filter tasks by priority (Low, Medium, High)
- Search tasks by title or description
- Clear all filters

### Responsive Design
- Mobile-first approach
- Tablet and desktop optimized layouts
- Touch-friendly interface

## API Integration

The frontend communicates with the backend API at the URL specified in `.env`:

- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `GET /api/tasks` - Get all tasks
- `POST /api/tasks` - Create new task
- `PUT /api/tasks/:id` - Update task
- `DELETE /api/tasks/:id` - Delete task
- `GET /api/tasks/statuses` - Get task statuses
- `GET /api/tasks/priorities` - Get task priorities

## Environment Variables

- `VITE_API_URL` - Backend API base URL (default: `https://localhost:7001/api`)

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## Troubleshooting

### CORS Issues
If you encounter CORS errors, make sure the backend API is configured to allow requests from the frontend URL (`http://localhost:5173`).

### API Connection Issues
Verify that:
1. The backend API is running
2. The `VITE_API_URL` in `.env` matches your backend URL
3. SSL certificate is trusted (if using HTTPS)

### Build Issues
Clear the build cache and reinstall dependencies:
```bash
rm -rf node_modules dist
npm install
npm run build
```

## License

This project is part of the Task Management System trial project.
