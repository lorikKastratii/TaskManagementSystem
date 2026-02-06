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
  const { getFilteredTasks, addTask, updateTask, deleteTask, loading, filter, statuses } = useTasks();
  const [showForm, setShowForm] = useState(false);
  const [editingTask, setEditingTask] = useState(null);

  const filteredTasks = getFilteredTasks();

  // Group tasks by status for Kanban lanes
  const getTasksByStatus = (statusId) => {
    return filteredTasks.filter(task => task.statusId === statusId);
  };

  const getStatusIcon = (statusName) => {
    switch(statusName) {
      case 'Todo': return '📋';
      case 'InProgress': return '⚙️';
      case 'Done': return '✅';
      default: return '📌';
    }
  };

  const getStatusColor = (statusName) => {
    switch(statusName) {
      case 'Todo': return '#6366f1';
      case 'InProgress': return '#f59e0b';
      case 'Done': return '#10b981';
      default: return '#64748b';
    }
  };

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
            <div className="kanban-board">
              {statuses.map((status) => {
                const tasksInLane = getTasksByStatus(status.id);
                return (
                  <div key={status.id} className="kanban-lane">
                    <div 
                      className="lane-header"
                      style={{ borderTopColor: getStatusColor(status.name) }}
                    >
                      <div className="lane-title">
                        <span className="lane-icon">{getStatusIcon(status.name)}</span>
                        <h3>{status.name}</h3>
                      </div>
                      <span className="lane-count">{tasksInLane.length}</span>
                    </div>
                    <div className="lane-content">
                      {tasksInLane.length === 0 ? (
                        <div className="lane-empty">
                          <p>No tasks</p>
                        </div>
                      ) : (
                        tasksInLane.map((task) => (
                          <TaskCard
                            key={task.id}
                            task={task}
                            onEdit={handleEditTask}
                            onDelete={handleDeleteTask}
                          />
                        ))
                      )}
                    </div>
                  </div>
                );
              })}
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
