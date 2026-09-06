import { useEffect, useMemo, useState } from 'react';
import Icon from './Icon.jsx';

function initialValue(field, item, lookups) {
  if (!item) return field.default ?? '';
  if (field.match) {
    const options = lookups[field.lookup.source] || [];
    const match = options.find((option) => String(option[field.match.optionKey]) === String(item[field.match.itemKey]));
    return match?.[field.lookup.valueKey] ?? '';
  }
  return item[field.key] ?? field.default ?? '';
}

export default function EntityModal({ title, fields, item, lookups, onClose, onSave, saving }) {
  const defaults = useMemo(()=>Object.fromEntries(fields.map((field)=>[field.key,initialValue(field,item,lookups)])),[fields,item,lookups]);
  const [form,setForm] = useState(defaults);
  useEffect(()=>setForm(defaults),[defaults]);
  function change(key,value){setForm((old)=>({...old,[key]:value}));}
  async function submit(e){e.preventDefault(); const payload={}; fields.forEach((field)=>{let value=form[field.key]; if(field.type==='number') value=value===''?0:Number(value); payload[field.key]=value;}); await onSave(payload);}
  return <div className="modal-overlay" onMouseDown={(e)=>{if(e.target===e.currentTarget)onClose();}}><form className="modal" onSubmit={submit}>
    <div className="modal-header"><h3>{title}</h3><button type="button" className="modal-close" onClick={onClose}><Icon name="close"/></button></div>
    <div className="modal-body"><div className="dynamic-form">{fields.map((field)=><div className={`input-group ${field.type==='textarea'?'wide':''}`} key={field.key}><label>{field.label}{field.required&&' *'}</label>
      {field.type==='textarea' ? <textarea rows="4" value={form[field.key]??''} required={field.required} onChange={(e)=>change(field.key,e.target.value)}/>
      : field.type==='select' ? <select value={form[field.key]??''} required={field.required} onChange={(e)=>change(field.key,e.target.value)}><option value="">Select {field.label}</option>{field.options.map((x)=><option value={x} key={x}>{x||'None'}</option>)}</select>
      : field.type==='lookup' ? <select value={form[field.key]??''} required={field.required} onChange={(e)=>change(field.key,e.target.value)}><option value="">Select {field.label}</option>{(lookups[field.lookup.source]||[]).map((option)=><option value={option[field.lookup.valueKey]} key={option.id || option[field.lookup.valueKey]}>{field.lookup.labelKeys.map((k)=>option[k]).filter(Boolean).join(' — ')}</option>)}</select>
      : <input type={field.type||'text'} step={field.step||undefined} placeholder={field.placeholder||''} value={form[field.key]??''} required={field.required} onChange={(e)=>change(field.key,e.target.value)}/>}</div>)}</div></div>
    <div className="modal-footer"><button type="button" className="btn btn-outline" onClick={onClose}>Cancel</button><button className="btn btn-accent" disabled={saving}><Icon name="save"/>{saving?'Saving...':'Save'}</button></div>
  </form></div>;
}
