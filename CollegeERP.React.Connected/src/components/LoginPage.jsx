import { useState } from 'react';
import { useAuth } from '../context/AuthContext.jsx';
import { useToast } from '../context/ToastContext.jsx';

const demoAccounts = [
  ['Admin','System Admin','admin@collegeerp.com','admin123'],
  ['Principal','Principal','principal@college.edu','principal123'],
  ['HOD','HOD - CSE','ramesh.k@college.edu','hod123'],
  ['Faculty','Faculty','amit.v@college.edu','faculty123'],
  ['Student','Student','aarav.m@college.edu','student123'],
];

export default function LoginPage() {
  const { login } = useAuth();
  const { showToast } = useToast();
  const [email, setEmail] = useState('admin@collegeerp.com');
  const [password, setPassword] = useState('admin123');
  const [loading, setLoading] = useState(false);

  async function submit(event) {
    event.preventDefault(); setLoading(true);
    try { await login(email, password); showToast('Login successful'); }
    catch (error) { showToast(error.message, 'error'); }
    finally { setLoading(false); }
  }

  return (
    <div className="login-bg">
      <section className="login-left">
        <div className="login-brand-mark">C</div>
        <h1>CollegeERP</h1>
        <p>One connected platform for academics, people, finance, library and communication.</p>
        
      </section>
      <section className="login-right">
        <form className="login-card" onSubmit={submit}>
          <div className="login-title"><div className="sidebar-avatar">CE</div><div><h2>Welcome back</h2><p>Sign in to CollegeERP</p></div></div>
          <div className="input-group"><label>Email</label><input value={email} onChange={(e)=>setEmail(e.target.value)} type="email" required /></div>
          <div className="input-group"><label>Password</label><input value={password} onChange={(e)=>setPassword(e.target.value)} type="password" required /></div>
          <button className="btn-primary" disabled={loading}>{loading ? 'Connecting...' : 'Sign In'}</button>
          <div className="login-divider"><span>Quick login</span></div>
          <div className="quick-login-grid">
            {demoAccounts.map(([role,label,account,pass]) => (
              <button type="button" className="quick-login-card" key={role} onClick={()=>{setEmail(account);setPassword(pass);}}>
                <span className="ql-icon">{role[0]}</span><span><span className="ql-label">{label}</span><span className="ql-sub">{account}</span></span>
              </button>
            ))}
          </div>
          <p className="api-hint">API: {import.meta.env.VITE_API_URL || 'http://localhost:5158/api'}</p>
        </form>
      </section>
    </div>
  );
}
