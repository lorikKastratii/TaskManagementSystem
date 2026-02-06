import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import { useTasks } from '../context/TaskContext';
import TaskCard from '../components/TaskCard';
import TaskFilter from '../components/TaskFilter';
import TaskForm from '../components/TaskForm';
import './Tasks.css';

const Tasks = () => {
  const navigate = useNavigate();
  const { user, logout } = useAuth();
  const { getFilteredTasks, addTask, updateTask, deleteTask, loading, filter } = useTasks();
  const [showForm, setShowForm] = useState(false);
  const [editingTask, setEditingTask] = useState(null);

  const filteredTasks = getFilteredTasks();

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const handleCreateTask = () => {
    setEditingTask(null);
    setShowForm(true);
  };

  const handleEditTask = (task) => {
    setEditingTask(task);
    setShowForm(true);
  };

  const handleDeleteTask = async (id) => {
    if (window.confirm('Are you sure you want to delete this task?')) {
      const result = await deleteTask(id);
      if (!result.success) {
        alert(result.error);
      }
    }
  };

  const handleSaveTask = async (taskData) => {
    let result;
    if (editingTask) {
      result = await updateTask(editingTask.id, taskData);
    } else {
      result = await addTask(taskData);
    }

    if (result.success) {
      setShowForm(false);
      setEditingTask(null);
    } else {
      alert(result.error);
    }
  };

  const handleCloseForm = () => {
    setShowForm(false);
    setEditingTask(null);
  };

  return (
    <div className="tasks-page">
      <header className="tasks-header">
        <div className="header-content">
          <h1>Task Manager</h1>
          <div className="header-actions">
            <span className="user-name">Welcome, {user?.email}</span>
            <button onClick={handleLogout} className="btn-secondary">
              Logout
            </button>
          </div>
        </div>
      </header>

      <main className="tasks-main">
        <div className="tasks-container">
          <div className="tasks-toolbar">
            <h2>My Tasks</h2>
            <button onClick={handleCreateTask} className="btn-primary">
              + New Task
            </button>
          </div>

          <TaskFilter />

          {loading ? (
            <div className="loading">Loading tasks...</div>
          ) : filteredTasks.length === 0 ? (
            <div className="no-tasks">
              <p>No tasks found. Create your first task to get started!</p>
            </div>
          ) : (
            <div className="tasks-grid">
              {filteredTasks.map((task) => (
                <TaskCard
                  key={task.id}
                  task={task}
                  onEdit={handleEditTask}
                  onDelete={handleDeleteTask}
                />
              ))}
            </div>
          )}
        </div>
      </main>

      {showForm && (
        <TaskForm
          task={editingTask}
          onClose={handleCloseForm}
          onSave={handleSaveTask}
        />
      )}
    </div>
  );
};

export default Tasks;
