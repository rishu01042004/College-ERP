import { useCallback, useEffect, useState } from 'react';
import { api } from '../api/client.js';
import { useAuth } from '../context/AuthContext.jsx';
import { useToast } from '../context/ToastContext.jsx';
import DataTable from '../components/DataTable.jsx';
import Icon from '../components/Icon.jsx';

const columns=[['type','Type'],['requestedBy','Requested By'],['department','Department'],['details','Details'],['requestDate','Request Date'],['status','Status','status']];
export default function HodAuthorizationPage(){const {user}=useAuth();const {showToast}=useToast();const [rows,setRows]=useState([]);const [loading,setLoading]=useState(true);const canDecide=['Admin','Principal','HOD'].includes(user.role);const canDelete=['Admin','Principal'].includes(user.role);
 const load=useCallback(async()=>{setLoading(true);try{const r=await api.hodAuth.list({page:1,pageSize:100});setRows(r.items||[]);}catch(e){showToast(e.message,'error');}finally{setLoading(false);}},[showToast]);useEffect(()=>{load();},[load]);
 async function create(){const type=prompt('Request type');if(!type)return;const details=prompt('Details')||'';try{await api.hodAuth.create({type,requestedBy:user.name,department:user.department||prompt('Department code')||'',details});showToast('Request created');await load();}catch(e){showToast(e.message,'error');}}
 async function decide(row){const decision=confirm('Approve this request? Click Cancel to reject.')?'Approved':'Rejected';try{await api.hodAuth.decide(row.id,decision);showToast(`Request ${decision.toLowerCase()}`);await load();}catch(e){showToast(e.message,'error');}}
 async function remove(row){if(!confirm('Delete this request?'))return;try{await api.hodAuth.remove(row.id);await load();}catch(e){showToast(e.message,'error');}}
 return <><div className="page-header page-header-row"><div><h1>HOD Authorization</h1><p>Approval requests and decisions</p></div><button className="btn btn-accent" onClick={create}><Icon name="add"/>New Request</button></div><div className="table-card"><DataTable columns={columns} rows={rows} loading={loading} canEdit={canDecide} canDelete={canDelete} onEdit={decide} onDelete={remove}/></div></>}
