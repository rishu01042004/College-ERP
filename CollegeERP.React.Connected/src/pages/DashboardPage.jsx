import { useEffect, useState } from 'react';
import { api } from '../api/client.js';
import { useAuth } from '../context/AuthContext.jsx';
import { useToast } from '../context/ToastContext.jsx';
import StatusBadge from '../components/StatusBadge.jsx';

export default function DashboardPage({ onNavigate }) {
  const {user}=useAuth(); const {showToast}=useToast(); const [data,setData]=useState(null);
  useEffect(()=>{api.dashboard().then(setData).catch(e=>showToast(e.message,'error'));},[showToast]);
  const stats=data?.stats||{};
  const cards=[['Active Students',stats.totalStudents??'—','students'],['Active Faculty',stats.totalFaculty??'—','faculty'],['Departments',stats.totalDepartments??'—','departments'],['Attendance',stats.attendancePercentage===undefined?'—':`${stats.attendancePercentage}%`,'attendance'],['Average SGPA',stats.averageSgpa??'—','results'],['Unread Alerts',stats.unreadNotifications??'—','notifications']];
  return <div><div className="welcome-banner"><div><span className={`role-tag ${user.role.toLowerCase()}`}>{user.role}</span><h1>Welcome, {user.name}</h1><p>{user.department?`${user.department} Department · `:''}Your CollegeERP data is connected to the backend.</p></div><div className="welcome-orb">{user.avatar||user.name[0]}</div></div>
    <div className="stat-grid">{cards.map(([label,value,page],index)=><button className={`stat-card ${['emerald','cyan','amber','red','purple','cyan'][index]}`} key={label} onClick={()=>onNavigate(page)}><div className="stat-value">{value}</div><div className="stat-label">{label}</div><div className="stat-change up">Open module →</div></button>)}</div>
    <div className="dashboard-grid"><section className="chart-card"><h3>Recent Announcements</h3><div className="activity-list">{(data?.recentAnnouncements||[]).map((a)=><div className="activity-item" key={a.id}><div className="activity-dot"/><div><div className="activity-text"><strong>{a.title}</strong></div><div className="activity-time">{a.date} · {a.postedBy}</div></div><StatusBadge value={a.priority}/></div>)}{data&&!data.recentAnnouncements?.length&&<p className="empty-message">No announcements.</p>}</div></section>
    <section className="chart-card"><h3>Quick Actions</h3><div className="quick-grid">{[['students','Students'],['attendance','Attendance'],['results','Results'],['announcements','Announcements']].map(([id,label])=><button className="quick-btn" key={id} onClick={()=>onNavigate(id)}><span>{label[0]}</span>{label}</button>)}</div></section></div>
  </div>;
}
