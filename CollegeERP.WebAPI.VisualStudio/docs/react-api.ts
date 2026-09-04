declare global {
  interface ImportMetaEnv { readonly VITE_API_URL?: string; }
  interface ImportMeta { readonly env: ImportMetaEnv; }
}

export const API_URL = import.meta.env.VITE_API_URL ?? "http://localhost:5158/api";
const TOKEN_KEY = "erp_jwt_token";
const USER_KEY = "erp_user";

export type AuthUser = {
  id: string;
  name: string;
  email: string;
  role: "Admin" | "Principal" | "HOD" | "Faculty" | "Student";
  department: string;
  avatar: string;
  color: string;
};

export type LoginResponse = {
  accessToken: string;
  expiresAtUtc: string;
  user: AuthUser;
};

export type PagedResult<T> = {
  items: T[];
  page: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
};

export class ApiError extends Error {
  constructor(public status: number, message: string, public data?: unknown) {
    super(message);
  }
}

async function request<T>(path: string, init: RequestInit = {}): Promise<T> {
  const token = localStorage.getItem(TOKEN_KEY);
  const response = await fetch(`${API_URL}${path}`, {
    ...init,
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...init.headers,
    },
  });

  if (response.status === 401) {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
  }

  if (!response.ok) {
    const data = await response.json().catch(() => null);
    throw new ApiError(response.status, data?.message ?? `API error ${response.status}`, data);
  }

  if (response.status === 204) return undefined as T;
  return response.json() as Promise<T>;
}

export const authApi = {
  async login(email: string, password: string) {
    const result = await request<LoginResponse>("/auth/login", {
      method: "POST",
      body: JSON.stringify({ email, password }),
    });
    localStorage.setItem(TOKEN_KEY, result.accessToken);
    localStorage.setItem(USER_KEY, JSON.stringify(result.user));
    return result;
  },
  me: () => request<AuthUser>("/auth/me"),
  logout() {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
  },
};

function crud<TRead, TWrite>(resource: string) {
  return {
    list: (query = "") => request<PagedResult<TRead>>(`/${resource}${query ? `?${query}` : ""}`),
    get: (id: string) => request<TRead>(`/${resource}/${id}`),
    create: (data: TWrite) => request<TRead>(`/${resource}`, { method: "POST", body: JSON.stringify(data) }),
    update: (id: string, data: TWrite) => request<void>(`/${resource}/${id}`, { method: "PUT", body: JSON.stringify(data) }),
    remove: (id: string) => request<void>(`/${resource}/${id}`, { method: "DELETE" }),
  };
}

export const api = {
  dashboard: () => request("/dashboard"),
  departments: crud("departments"),
  courses: crud("courses"),
  subjects: crud("subjects"),
  semesters: crud("semesters"),
  exams: crud("exams"),
  students: crud("students"),
  faculty: crud("faculty"),
  attendance: crud("attendance"),
  marks: crud("marks"),
  results: crud("results"),
  fees: crud("fees"),
  books: crud("books"),
  categories: crud("categories"),
  announcements: crud("announcements"),
  users: crud("users"),
  roles: crud("roles"),
  notifications: {
    list: (query = "") => request(`/${"notifications"}${query ? `?${query}` : ""}`),
    markRead: (id: string) => request<void>(`/notifications/${id}/read`, { method: "PATCH" }),
    markAllRead: () => request("/notifications/read-all", { method: "PATCH" }),
    remove: (id: string) => request<void>(`/notifications/${id}`, { method: "DELETE" }),
  },
  hodAuthorizations: {
    list: (query = "") => request(`/hod-authorizations${query ? `?${query}` : ""}`),
    create: (data: unknown) => request("/hod-authorizations", { method: "POST", body: JSON.stringify(data) }),
    decide: (id: string, decision: "Approved" | "Rejected") => request<void>(`/hod-authorizations/${id}/decision`, { method: "PATCH", body: JSON.stringify({ decision }) }),
  },
  loginHistory: (query = "") => request(`/login-history${query ? `?${query}` : ""}`),
  settings: {
    get: () => request("/settings"),
    update: (data: unknown) => request<void>("/settings", { method: "PUT", body: JSON.stringify(data) }),
  },
};
