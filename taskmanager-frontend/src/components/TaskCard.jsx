import { useTasks } from '../context/TaskContext';
import './TaskCard.css';

const TaskCard = ({ task, onEdit, onDelete }) => {
  const { statuses, priorities } = useTasks();

  const getStatusName = (statusId) => {
    const status = statuses.find(s => s.id === statusId);
    return status?.name || 'Unknown';
  };

  const getPriorityName = (priorityId) => {
    const priority = priorities.find(p => p.id === priorityId);
    return priority?.name || 'Unknown';
  };

  const getPriorityIcon = (priorityId) => {
    const priorityName = getPriorityName(priorityId);
    switch(priorityName) {
      case 'Low': return '🟢';
      case 'Medium': return '🟡';
      case 'High': return '🔴';
      default: return '⚪';
    }
  };

  const getStatusIcon = (statusId) => {
    const statusName = getStatusName(statusId);
    switch(statusName) {
      case 'Todo': return '📋';
      case 'InProgress': return '⚙️';
      case 'Done': return '✅';
      default: return '📌';
    }
  };

  const formatDate = (dateString) => {
    if (!dateString) return 'N/A';
    return new Date(dateString).toLocaleDateString();
  };

  return (
    <div className="task-card">
      <div className="task-card-header">
        <h3>{task.title}</h3>
        <div className="task-badges">
          <span className={`badge status-${getStatusName(task.statusId).toLowerCase()}`}>
            {getStatusIcon(task.statusId)} {getStatusName(task.statusId)}
          </span>
          <span className={`badge priority-${getPriorityName(task.priorityId).toLowerCase()}`}>
            {getPriorityIcon(task.priorityId)} {getPriorityName(task.priorityId)}
          </span>
        </div>
      </div>

      <p className="task-description">{task.description || 'No description'}</p>

      <div className="task-dates">
        <div>
          <strong>📅 Created:</strong> {formatDate(task.createdAt)}
        </div>
        {task.dueDate && (
          <div>
            <strong>⏰ Due:</strong> {formatDate(task.dueDate)}
          </div>
        )}
      </div>

      <div className="task-actions">
        <button onClick={() => onEdit(task)} className="btn-primary">
          ✏️ Edit
        </button>
        <button onClick={() => onDelete(task.id)} className="btn-danger">
          🗑️ Delete
        </button>
      </div>
    </div>
  );
};

export default TaskCard;
