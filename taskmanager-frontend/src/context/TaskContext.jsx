import { createContext, useState, useContext, useEffect } from 'react';
import taskService from '../services/taskService';
import authService from '../services/authService';

const TaskContext = createContext(null);

export const TaskProvider = ({ children }) => {
  const [tasks, setTasks] = useState([]);
  const [statuses, setStatuses] = useState([]);
  const [priorities, setPriorities] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [filter, setFilter] = useState({
    priority: '',
    searchTerm: ''
  });

  useEffect(() => {
    // Only load tasks if user is authenticated
    if (authService.getCurrentUser()) {
      loadTasksData();
    }
  }, []);

  const loadTasksData = async () => {
    try {
      setLoading(true);
      const [tasksData, statusesData, prioritiesData] = await Promise.all([
        taskService.getAllTasks(),
        taskService.getTaskStatuses(),
        taskService.getTaskPriorities()
      ]);
      setTasks(tasksData);
      setStatuses(statusesData);
      setPriorities(prioritiesData);
      setError(null);
    } catch (err) {
      setError(err.response?.data?.message || 'Failed to load tasks');
    } finally {
      setLoading(false);
    }
  };

  const addTask = async (taskData) => {
    try {
      const newTask = await taskService.createTask(taskData);
      setTasks([...tasks, newTask]);
      return { success: true, task: newTask };
    } catch (err) {
      return {
        success: false,
        error: err.response?.data?.message || 'Failed to create task'
      };
    }
  };

  const updateTask = async (id, taskData) => {
    try {
      const updatedTask = await taskService.updateTask(id, taskData);
      setTasks(tasks.map(task => task.id === id ? updatedTask : task));
      return { success: true, task: updatedTask };
    } catch (err) {
      return {
        success: false,
        error: err.response?.data?.message || 'Failed to update task'
      };
    }
  };

  const deleteTask = async (id) => {
    try {
      await taskService.deleteTask(id);
      setTasks(tasks.filter(task => task.id !== id));
      return { success: true };
    } catch (err) {
      return {
        success: false,
        error: err.response?.data?.message || 'Failed to delete task'
      };
    }
  };

  const getFilteredTasks = () => {
    const filtered = tasks.filter(task => {
      // Convert to number for comparison, handle empty string
      const filterPriorityId = filter.priority === '' ? null : Number(filter.priority);
      
      const matchesPriority = filterPriorityId === null || task.priorityId === filterPriorityId;
      const matchesSearch = !filter.searchTerm ||
        task.title?.toLowerCase().includes(filter.searchTerm.toLowerCase()) ||
        task.description?.toLowerCase().includes(filter.searchTerm.toLowerCase());

      return matchesPriority && matchesSearch;
    });
    
    return filtered;
  };

  const value = {
    tasks,
    statuses,
    priorities,
    loading,
    error,
    filter,
    setFilter,
    addTask,
    updateTask,
    deleteTask,
    refreshTasks: loadTasksData,
    getFilteredTasks
  };

  return <TaskContext.Provider value={value}>{children}</TaskContext.Provider>;
};

export const useTasks = () => {
  const context = useContext(TaskContext);
  if (!context) {
    throw new Error('useTasks must be used within a TaskProvider');
  }
  return context;
};
