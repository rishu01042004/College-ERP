import { useState } from 'react';
import Icon from './Icon.jsx';
import { NAV_ITEMS } from '../config/modules.js';
import { useAuth } from '../context/AuthContext.jsx';

export default function Topbar({ page, onMenu, onNavigate }) {
  const { user, logout } = useAuth();
  const [query,setQuery] = useState('');
  const label = NAV_ITEMS.find((item)=>item.id===page)?.label || 'Dashboard';
  function submit(e){e.preventDefault(); const hit=NAV_ITEMS.find(x=>x.label.toLowerCase().includes(query.toLowerCase())); if(hit) onNavigate(hit.id);}
  return <header className="topbar">
    <button className="topbar-hamburger" onClick={onMenu}><Icon name="menu"/></button>
    <div className="topbar-breadcrumb">CollegeERP <span className="crumb-arrow">›</span> <span>{label}</span></div>
    <div className="topbar-right">
      <form className="topbar-search" onSubmit={submit}><Icon name="search"/><input value={query} onChange={(e)=>setQuery(e.target.value)} placeholder="Search modules..." /></form>
      <button className="topbar-icon" title="Notifications" onClick={()=>onNavigate('notifications')}><Icon name="notifications"/><span className="dot"/></button>
      <div className="topbar-profile"><div className="sidebar-avatar small">{user.avatar || 'U'}</div><div className="profile-name"><strong>{user.name}</strong><small>{user.role}</small></div><button className="logout-button" onClick={logout} title="Logout"><Icon name="logout"/></button></div>
    </div>
  </header>;
}
