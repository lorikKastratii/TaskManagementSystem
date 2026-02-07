import { useState, useEffect } from 'react';
import { useTasks } from '../context/TaskContext';
import './TaskForm.css';

const TaskForm = ({ task, onClose, onSave }) => {
  const { statuses, priorities } = useTasks();
  const [formData, setFormData] = useState({
    title: '',
    description: '',
    statusId: '',
    priorityId: '',
    dueDate: ''
  });
  const [errors, setErrors] = useState({});

  useEffect(() => {
    if (task) {
      setFormData({
        title: task.title || '',
        description: task.description || '',
        statusId: task.statusId || '',
        priorityId: task.priorityId || '',
        dueDate: task.dueDate ? task.dueDate.split('T')[0] : ''
      });
    } else {
      // Set default status to "Todo" (id: 0) for new tasks
      const todoStatus = statuses.find(s => s.name === 'Todo');
      if (todoStatus) {
        setFormData(prev => ({ ...prev, statusId: todoStatus.id }));
      }
    }
  }, [task, statuses]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData({ ...formData, [name]: value });
    if (errors[name]) {
      setErrors({ ...errors, [name]: '' });
    }
  };

  const validate = () => {
    const newErrors = {};
    if (!formData.title.trim()) {
      newErrors.title = 'Title is required';
    }
    if (!formData.priorityId) {
      newErrors.priorityId = 'Priority is required';
    }
    setErrors(newErrors);
    return Object.keys(newErrors).length === 0;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validate()) return;

    const taskData = {
      ...formData,
      statusId: parseInt(formData.statusId),
      priorityId: parseInt(formData.priorityId),
      dueDate: formData.dueDate || null
    };

    await onSave(taskData);
  };

  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <div className="modal-header">
          <h2>{task ? 'Edit Task' : 'Create New Task'}</h2>
          <button onClick={onClose} className="close-btn">&times;</button>
        </div>

        <form onSubmit={handleSubmit} className="task-form">
          <div className="form-group">
            <label htmlFor="title">Title *</label>
            <input
              type="text"
              id="title"
              name="title"
              value={formData.title}
              onChange={handleChange}
              className={errors.title ? 'error' : ''}
            />
            {errors.title && <span className="error-message">{errors.title}</span>}
          </div>

          <div className="form-group">
            <label htmlFor="description">Description</label>
            <textarea
              id="description"
              name="description"
              value={formData.description}
              onChange={handleChange}
              rows="4"
            />
          </div>

          {/* Hidden status field - defaults to Todo for new tasks */}
          <input type="hidden" name="statusId" value={formData.statusId} />

          <div className="form-row">
            <div className="form-group">
              <label htmlFor="priorityId">Priority *</label>
              <select
                id="priorityId"
                name="priorityId"
                value={formData.priorityId}
                onChange={handleChange}
                className={errors.priorityId ? 'error' : ''}
              >
                <option value="">Select Priority</option>
                {priorities.map((priority) => (
                  <option key={priority.id} value={priority.id}>
                    {priority.name}
                  </option>
                ))}
              </select>
              {errors.priorityId && <span className="error-message">{errors.priorityId}</span>}
            </div>

            <div className="form-group">
              <label htmlFor="dueDate">Due Date</label>
              <input
                type="date"
                id="dueDate"
                name="dueDate"
                value={formData.dueDate}
                onChange={handleChange}
              />
            </div>
          </div>

          <div className="form-actions">
            <button type="button" onClick={onClose} className="btn-secondary">
              Cancel
            </button>
            <button type="submit" className="btn-primary">
              {task ? 'Update' : 'Create'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default TaskForm;
