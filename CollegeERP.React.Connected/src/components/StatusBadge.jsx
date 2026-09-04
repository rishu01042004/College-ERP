export default function StatusBadge({ value }) {
  const s = String(value ?? 'Unknown').toLowerCase();
  const cls = ['active','paid','passed','completed','sent','approved','available','present','success'].includes(s) ? 'active'
    : ['pending','upcoming','partial','queued','late','medium'].includes(s) ? 'pending'
    : ['inactive','overdue','failed','rejected','absent','all issued','on leave','high'].includes(s) ? 'inactive' : 'info';
  return <span className={`badge-status ${cls}`}>{String(value ?? '—')}</span>;
}
