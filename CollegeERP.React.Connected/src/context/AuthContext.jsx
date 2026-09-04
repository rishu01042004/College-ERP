import { createContext, useContext, useEffect, useMemo, useState } from 'react';
import { authApi } from '../api/client.js';

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(() => authApi.storedUser());
  const [checking, setChecking] = useState(authApi.hasToken());

  useEffect(() => {
    let active = true;
    async function verify() {
      if (!authApi.hasToken()) { setChecking(false); return; }
      try {
        const current = await authApi.me();
        if (active) setUser(current);
      } catch {
        if (active) setUser(null);
      } finally {
        if (active) setChecking(false);
      }
    }
    verify();
    const unauthorized = () => setUser(null);
    window.addEventListener('collegeerp:unauthorized', unauthorized);
    return () => { active = false; window.removeEventListener('collegeerp:unauthorized', unauthorized); };
  }, []);

  const value = useMemo(() => ({
    user,
    checking,
    async login(email, password) {
      const result = await authApi.login(email, password);
      setUser(result.user);
      return result.user;
    },
    logout() { authApi.logout(); setUser(null); },
  }), [user, checking]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const value = useContext(AuthContext);
  if (!value) throw new Error('useAuth must be used inside AuthProvider');
  return value;
}
