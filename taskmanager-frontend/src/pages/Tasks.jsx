import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { DragDropContext, Droppable, Draggable } from '@hello-pangea/dnd';
import { useAuth } from '../context/AuthContext';
import { useTasks } from '../context/TaskContext';
import TaskCard from '../components/TaskCard';
import TaskFilter from '../components/TaskFilter';
import TaskForm from '../components/TaskForm';
import './Tasks.css';

const Tasks = () => {
  const navigate = useNavigate();
  const { user, logout } = useAuth();
  const { tasks, setTasks, getFilteredTasks, addTask, updateTask, deleteTask, loading, filter, statuses, dragError, setDragError, clearDragError } = useTasks();
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

  const handleDragEnd = async (result) => {
    const { source, destination, draggableId } = result;

    console.log('Drag ended:', { source, destination, draggableId });

    // Dropped outside valid area or no movement
    if (!destination || source.droppableId === destination.droppableId) {
      console.log('Drag cancelled or no movement');
      return;
    }

    // Extract status ID from lane ID (e.g., "lane-1" -> 1)
    const newStatusId = parseInt(destination.droppableId.split('-')[1]);
    const taskId = draggableId;

    console.log('Extracted values:', { newStatusId, taskId });

    // Find the task and store original status
    const task = filteredTasks.find(t => t.id === taskId);
    if (!task) {
      console.error('Task not found:', taskId);
      return;
    }

    const originalStatusId = task.statusId;
    console.log('Task found:', { task, originalStatusId, newStatusId });

    // Optimistically update UI
    const updatedTasks = tasks.map(t =>
      t.id === taskId ? { ...t, statusId: newStatusId } : t
    );
    setTasks(updatedTasks);

    // Call backend API
    console.log('Calling updateTask API with:', { taskId, statusId: newStatusId });
    const apiResult = await updateTask(taskId, { statusId: newStatusId });
    console.log('API result:', apiResult);

    // Rollback on error
    if (!apiResult.success) {
      console.error('Update failed, rolling back:', apiResult.error);
      const rolledBackTasks = tasks.map(t =>
        t.id === taskId ? { ...t, statusId: originalStatusId } : t
      );
      setTasks(rolledBackTasks);
      setDragError(apiResult.error || 'Failed to update task status');
    }
  };

  // Auto-dismiss drag error after 5 seconds
  useEffect(() => {
    if (dragError) {
      const timer = setTimeout(() => {
        clearDragError();
      }, 5000);
      return () => clearTimeout(timer);
    }
  }, [dragError, clearDragError]);

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
            <DragDropContext onDragEnd={handleDragEnd}>
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
                      <Droppable droppableId={`lane-${status.id}`}>
                        {(provided, snapshot) => (
                          <div
                            ref={provided.innerRef}
                            {...provided.droppableProps}
                            className={`lane-content ${snapshot.isDraggingOver ? 'lane-content-drag-over' : ''}`}
                          >
                            {tasksInLane.length === 0 ? (
                              <div className="lane-empty">
                                <p>No tasks</p>
                              </div>
                            ) : (
                              tasksInLane.map((task, index) => (
                                <Draggable key={task.id} draggableId={task.id} index={index}>
                                  {(provided, snapshot) => (
                                    <div
                                      ref={provided.innerRef}
                                      {...provided.draggableProps}
                                      {...provided.dragHandleProps}
                                    >
                                      <TaskCard
                                        task={task}
                                        onEdit={handleEditTask}
                                        onDelete={handleDeleteTask}
                                        isDragging={snapshot.isDragging}
                                      />
                                    </div>
                                  )}
                                </Draggable>
                              ))
                            )}
                            {provided.placeholder}
                          </div>
                        )}
                      </Droppable>
                    </div>
                  );
                })}
              </div>
            </DragDropContext>
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

      {dragError && (
        <div className="drag-error-notification">
          <span>{dragError}</span>
          <button onClick={clearDragError}>Dismiss</button>
        </div>
      )}
    </div>
  );
};

export default Tasks;
