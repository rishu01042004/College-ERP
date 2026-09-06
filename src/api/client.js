const API_URL = (import.meta.env.VITE_API_URL || 'http://localhost:7158/api').replace(/\/$/, '');
const TOKEN_KEY = 'erp_jwt_token';
const USER_KEY = 'erp_user';

export class ApiError extends Error {
  constructor(status, message, data = null) {
    super(message);
    this.name = 'ApiError';
    this.status = status;
    this.data = data;
  }
}

function getErrorMessage(data, status) {
  if (!data) return `API request failed (${status})`;
  if (typeof data === 'string') return data;
  if (data.message) return data.message;
  if (data.title && data.errors) {
    const validation = Object.values(data.errors).flat().join(' ');
    return validation || data.title;
  }
  return data.title || `API request failed (${status})`;
}

async function request(path, options = {}) {
  const token = localStorage.getItem(TOKEN_KEY);
  const headers = new Headers(options.headers || {});
  if (options.body !== undefined && !(options.body instanceof FormData)) {
    headers.set('Content-Type', 'application/json');
  }
  if (token) headers.set('Authorization', `Bearer ${token}`);

  let response;
  try {
    response = await fetch(`${API_URL}${path}`, { ...options, headers });
  } catch (error) {
    throw new ApiError(0, `Cannot connect to ASP.NET Core API at ${API_URL}. Start the backend and check .env.`, error);
  }

  if (response.status === 401) {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    window.dispatchEvent(new Event('collegeerp:unauthorized'));
  }

  if (!response.ok) {
    const data = await response.json().catch(() => null);
    throw new ApiError(response.status, getErrorMessage(data, response.status), data);
  }

  if (response.status === 204) return null;
  const contentType = response.headers.get('content-type') || '';
  return contentType.includes('application/json') ? response.json() : response.text();
}

function queryString(params = {}) {
  const query = new URLSearchParams();
  Object.entries(params).forEach(([key, value]) => {
    if (value !== undefined && value !== null && value !== '') query.set(key, String(value));
  });
  const text = query.toString();
  return text ? `?${text}` : '';
}

function resource(name) {
  return {
    list: (params = {}) =>
      request(`/${name}${queryString(params)}`),

    get: (id) =>
      request(`/${name}/${id}`),

    create: (data) =>
      request(`/${name}`, {
        method: "POST",
        body: JSON.stringify(data),
      }),

    update: (id, data) =>
      request(`/${name}/${id}`, {
        method: "PUT",
        body: JSON.stringify(data),
      }),

    remove: (id) =>
      request(`/${name}/${id}`, {
        method: "DELETE",
      }),
  };
}

export const authApi = {
  async login(email, password) {
    const result = await request('/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password }),
    });
    localStorage.setItem(TOKEN_KEY, result.accessToken);
    localStorage.setItem(USER_KEY, JSON.stringify(result.user));
    return result;
  },
  me: () => request('/auth/me'),
  logout() {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
  },
  storedUser() {
    try { return JSON.parse(localStorage.getItem(USER_KEY) || 'null'); }
    catch { return null; }
  },
  hasToken: () => Boolean(localStorage.getItem(TOKEN_KEY)),
};

export const api = {
  health: () => fetch(API_URL.replace(/\/api$/, '/health')).then((r) => r.json()),
  dashboard: () => request('/dashboard'),
  departments: resource('departments'),
  courses: resource('courses'),
  subjects: resource('subjects'),
  semesters: resource('semesters'),
  exams: resource('exams'),
  students: resource('students'),
  faculty: resource('faculty'),
  attendance: resource('attendance'),
  marks: resource('marks'),
  results: resource('results'),
  fees: resource('fees'),
  books: resource('books'),
  categories: resource('categories'),
  announcements: resource('announcements'),
  users: resource('users'),
  roles: resource('roles'),
  notifications: {
    list: (params = {}) => request(`/notifications${queryString(params)}`),
    create: (data) => request('/notifications', { method: 'POST', body: JSON.stringify(data) }),
    markRead: (id) => request(`/notifications/${id}/read`, { method: 'PATCH' }),
    markAllRead: () => request('/notifications/read-all', { method: 'PATCH' }),
    remove: (id) => request(`/notifications/${id}`, { method: 'DELETE' }),
  },
  hodAuth: {
    list: (params = {}) => request(`/hod-authorizations${queryString(params)}`),
    create: (data) => request('/hod-authorizations', { method: 'POST', body: JSON.stringify(data) }),
    decide: (id, decision) => request(`/hod-authorizations/${id}/decision`, { method: 'PATCH', body: JSON.stringify({ decision }) }),
    remove: (id) => request(`/hod-authorizations/${id}`, { method: 'DELETE' }),
  },
  loginHistory: { list: (params = {}) => request(`/login-history${queryString(params)}`) },
  settings: {
    get: () => request('/settings'),
    update: (data) => request('/settings', { method: 'PUT', body: JSON.stringify(data) }),
  },
};

export { API_URL, request, TOKEN_KEY, USER_KEY };
