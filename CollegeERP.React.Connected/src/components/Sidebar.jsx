import Icon from './Icon.jsx';
import { NAV_ITEMS, ROLE_PAGES } from '../config/modules.js';

export default function Sidebar({ user, currentPage, onNavigate, open }) {
  const visible = NAV_ITEMS.filter((item) => (ROLE_PAGES[user.role] || []).includes(item.id));
  let category = '';
  return (
    <aside className={`sidebar ${open ? 'open' : ''}`}>
      <div className="sidebar-logo"><div className="brand-row"><span className="brand-icon">C</span><div><strong>CollegeERP</strong><small>Management System</small></div></div></div>
      <nav className="sidebar-nav">
        {visible.map((item) => {
          const heading = item.category !== category; category = item.category;
          return <div key={item.id}>{heading && <div className="nav-category">{item.category}</div>}<button className={`nav-item ${currentPage===item.id?'active':''}`} onClick={()=>onNavigate(item.id)}><Icon name={item.id}/><span>{item.label}</span></button></div>;
        })}
      </nav>
      <div className="sidebar-footer"><div className="sidebar-user"><div className="sidebar-avatar">{user.avatar || user.name.split(' ').map(x=>x[0]).slice(0,2).join('')}</div><div className="user-mini"><strong>{user.name}</strong><span className={`role-tag ${user.role.toLowerCase()}`}>{user.role}</span></div></div></div>
    </aside>
  );
}
