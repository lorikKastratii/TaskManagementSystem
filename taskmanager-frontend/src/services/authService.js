import api from './api';

class AuthService {
  async login(email, password) {
    const response = await api.post('/auth/login', { email, password });
    if (response.data.token) {
      localStorage.setItem('token', response.data.token);
      const user = { email: response.data.email };
      localStorage.setItem('user', JSON.stringify(user));
    }
    return response.data;
  }

  async register(email, password, confirmPassword) {
    const response = await api.post('/auth/register', { email, password, confirmPassword });
    return response.data;
  }

  logout() {
    localStorage.removeItem('token');
    localStorage.removeItem('user');
  }

  getCurrentUser() {
    const userStr = localStorage.getItem('user');
    return userStr ? JSON.parse(userStr) : null;
  }

  getToken() {
    return localStorage.getItem('token');
  }

  isAuthenticated() {
    return !!this.getToken();
  }
}

export default new AuthService();
