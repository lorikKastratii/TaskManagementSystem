import api from './api';

class TaskService {
  async getAllTasks() {
    const response = await api.get('/tasks');
    return response.data;
  }

  async getTaskById(id) {
    const response = await api.get(`/tasks/${id}`);
    return response.data;
  }

  async createTask(taskData) {
    const response = await api.post('/tasks', taskData);
    return response.data;
  }

  async updateTask(id, taskData) {
    const response = await api.put(`/tasks/${id}`, taskData);
    return response.data;
  }

  async deleteTask(id) {
    const response = await api.delete(`/tasks/${id}`);
    return response.data;
  }

  async getTaskStatuses() {
    const response = await api.get('/tasks/statuses');
    return response.data;
  }

  async getTaskPriorities() {
    const response = await api.get('/tasks/priorities');
    return response.data;
  }
}

export default new TaskService();
