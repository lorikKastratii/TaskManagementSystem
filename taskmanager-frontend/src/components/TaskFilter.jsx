import { useTasks } from '../context/TaskContext';
import './TaskFilter.css';

const TaskFilter = () => {
  const { filter, setFilter, priorities } = useTasks();

  const handleFilterChange = (field, value) => {
    setFilter({ ...filter, [field]: value });
  };

  const clearFilters = () => {
    setFilter({ priority: '', searchTerm: '' });
  };

  return (
    <div className="task-filter">
      <div className="filter-group">
        <input
          type="text"
          placeholder="Search tasks..."
          value={filter.searchTerm}
          onChange={(e) => handleFilterChange('searchTerm', e.target.value)}
          className="search-input"
        />
      </div>

      <div className="filter-group">
        <select
          value={filter.priority}
          onChange={(e) => handleFilterChange('priority', e.target.value)}
          className="filter-select"
        >
          <option value="">All Priorities</option>
          {priorities.map((priority) => (
            <option key={priority.id} value={priority.id}>
              {priority.name}
            </option>
          ))}
        </select>
      </div>

      <button onClick={clearFilters} className="btn-secondary">
        Clear Filters
      </button>
    </div>
  );
};

export default TaskFilter;
