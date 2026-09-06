const symbols = {
  dashboard: '▦', departments: '⌂', courses: '▤', subjects: '▧', semesters: '◫', exams: '✎',
  students: '♟', faculty: '♜', attendance: '✓', marks: '★', results: '◉', fees: '₹',
  books: '▥', categories: '⌗', announcements: '◖', notifications: '●', users: '♛', roles: '⚿',
  hodAuth: '✥', loginHistory: '◷', settings: '⚙', menu: '☰', search: '⌕', logout: '↪',
  add: '+', edit: '✎', delete: '×', close: '×', refresh: '↻', save: '✓', api: '⌘', home: '⌂',
};
export default function Icon({ name, className = '' }) {
  return <span className={`icon ${className}`} aria-hidden="true">{symbols[name] || '•'}</span>;
}
