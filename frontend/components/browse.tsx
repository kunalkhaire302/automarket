"use client";
import { useEffect, useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { Search, SlidersHorizontal } from "lucide-react";
import { api, label } from "@/lib/api";
import type { Car, City, Meta } from "@/lib/types";
import { CarCard, Empty, ErrorNotice, Loading } from "./ui";
const selectFields = [
  ["fuelType", "Fuel", ["PETROL", "DIESEL", "CNG", "EV", "HYBRID"]],
  ["transmission", "Transmission", ["MANUAL", "AUTOMATIC", "AMT", "CVT", "DCT"]],
  ["bodyType", "Body style", ["HATCHBACK", "SEDAN", "SUV", "MUV", "COUPE"]]
] as const;
export function Browse() {
  const params = useSearchParams(); const router = useRouter();
  const [cars,setCars] = useState<Car[]|null>(null); const [cities,setCities] = useState<City[]>([]);
  const [meta,setMeta] = useState<Meta>(); const [error,setError] = useState(""); const [attempt,setAttempt] = useState(0);
  const [query,setQuery] = useState(params.get("q") ?? ""); const [suggestions,setSuggestions] = useState<{id:string;text:string}[]>([]);
  const [active,setActive] = useState(-1); const [showFilters,setShowFilters] = useState(false); const [cityError,setCityError] = useState("");
  const queryString = params.toString();
  useEffect(() => { api<City[]>("locations/cities").then(r=>setCities(r.data)).catch(e=>setCityError(e.message)); }, []);
  useEffect(() => {setQuery(params.get("q") ?? "");}, [queryString, params]);
  useEffect(() => { const c=new AbortController();setCars(null);setError("");api<Car[]>(`cars?${queryString}`,{signal:c.signal}).then(r=>{setCars(r.data);setMeta(r.meta);}).catch(e=>{if(!c.signal.aborted)setError(e.message);});return()=>c.abort();},[queryString,attempt]);
  useEffect(()=>{ const c=new AbortController(); const timer=setTimeout(()=>{if(query.trim().length<2){setSuggestions([]);return;}api<{id:string;text:string}[]>(`search/suggestions?q=${encodeURIComponent(query)}&${params.get("cityId")?`cityId=${params.get("cityId")}`:""}`,{signal:c.signal}).then(r=>{setSuggestions(r.data);setActive(-1);}).catch(()=>setSuggestions([]));},300);return()=>{clearTimeout(timer);c.abort();};},[query,params]);
  function change(key:string,value:string){const next=new URLSearchParams(queryString);if(value)next.set(key,value);else next.delete(key);if(key!=="page")next.delete("page");router.push(`/buy?${next}`);}
  function search(text=query){change("q",text);setSuggestions([]);}
  return <div className="container"><div className="page-heading"><div className="eyebrow muted">THE RIGHT CAR IS OUT THERE</div><h1>Let’s find yours.</h1><p>Choose your city, set your budget, and make it your own.</p></div>
    <div className="browse-layout"><aside className={`filters ${showFilters?"open":""}`}><h3>Refine your search</h3><label className="field">Your city<select value={params.get("cityId")??""} onChange={e=>{localStorage.setItem("am_city",e.target.value);change("cityId",e.target.value);}}><option value="">All service cities</option>{cities.map(c=><option key={c.id} value={c.id}>{c.name}</option>)}</select></label>{cityError&&<p className="inline-status">Cities could not be loaded.</p>}
    {[['brand','Make'],['model','Model']].map(([key,name])=><label className="field" key={key}>{name}<input defaultValue={params.get(key)??""} key={params.get(key)??key} placeholder={`Any ${name.toLowerCase()}`} onBlur={e=>change(key,e.target.value)}/></label>)}
    {[["Price (₹)","minPrice","maxPrice"],["Year","minYear","maxYear"],["Distance (km)","minKilometers","maxKilometers"]].map(([name,min,max])=><div className="field" key={min}><span>{name}</span><div className="filter-pair"><input type="number" min="0" aria-label={`Minimum ${name}`} placeholder="Min" defaultValue={params.get(min)??""} key={params.get(min)??min} onBlur={e=>change(min,e.target.value)}/><input type="number" min="0" aria-label={`Maximum ${name}`} placeholder="Max" defaultValue={params.get(max)??""} key={params.get(max)??max} onBlur={e=>change(max,e.target.value)}/></div></div>)}
    {selectFields.map(([key,name,options])=><label className="field" key={key}>{name}<select value={params.get(key)??""} onChange={e=>change(key,e.target.value)}><option value="">Any</option>{options.map(o=><option value={o} key={o}>{label(o)}</option>)}</select></label>)}<button className="text-button" onClick={()=>router.push("/buy")}>Reset filters</button></aside>
    <div><form className="search-row" onSubmit={e=>{e.preventDefault();search();}}><input aria-label="Search make or model" role="combobox" aria-expanded={suggestions.length>0} aria-controls="suggestions" aria-activedescendant={active>=0?`suggestion-${active}`:undefined} value={query} onChange={e=>setQuery(e.target.value)} onKeyDown={e=>{if(e.key==="ArrowDown"){e.preventDefault();setActive(i=>Math.min(i+1,suggestions.length-1));}if(e.key==="ArrowUp"){e.preventDefault();setActive(i=>Math.max(i-1,0));}if(e.key==="Escape")setSuggestions([]);if(e.key==="Enter"&&active>=0&&suggestions[active]){e.preventDefault();setQuery(suggestions[active].text);search(suggestions[active].text);}}} placeholder="Make, model, or body style"/><button className="button" aria-label="Search"><Search size={18}/></button><select aria-label="Sort cars" value={params.get("sort")??"relevance"} onChange={e=>change("sort",e.target.value)}>{[["relevance","Most relevant"],["price-asc","Price: low to high"],["price-desc","Price: high to low"],["newest","Newest arrivals"],["mileage","Lowest mileage"]].map(([v,t])=><option key={v} value={v}>{t}</option>)}</select>
    {suggestions.length>0&&<div className="suggestions" id="suggestions" role="listbox">{suggestions.map((s,i)=><button role="option" aria-selected={i===active} className={i===active?"active":""} id={`suggestion-${i}`} key={s.id} type="button" onClick={()=>{setQuery(s.text);search(s.text);}}>{s.text}</button>)}</div>}</form>
    <div className="results-meta"><span>{meta?`${meta.totalItems} cars to explore`:"Finding your next car…"}</span><button className="text-button filter-toggle" onClick={()=>setShowFilters(!showFilters)}><SlidersHorizontal size={15}/>{showFilters?"Hide":"Show"} filters</button></div>
    <div className="chips">{Array.from(params.entries()).filter(([k])=>!["page","sort"].includes(k)).map(([k,v])=><button key={k} onClick={()=>change(k,"")}>{k==="cityId"?cities.find(c=>c.id===v)?.name??"Selected city":`${k}: ${v}`} ×</button>)}</div>
    {error?<ErrorNotice message={error} retry={()=>setAttempt(x=>x+1)}/>:!cars?<Loading/>:cars.length?<div className="car-grid">{cars.map(c=><CarCard key={c.id} car={c}/>)}</div>:<Empty title="No cars match just yet.">Try widening your budget, checking another city, or removing a filter. <button className="text-button" onClick={()=>router.push("/buy")}>Clear all filters</button></Empty>}
    {meta&&meta.totalPages>1&&<div className="pagination"><button className="button secondary" disabled={meta.page<=1} onClick={()=>change("page",String(meta.page-1))}>Previous</button><span>{meta.page} / {meta.totalPages}</span><button className="button secondary" disabled={!meta.hasNext} onClick={()=>change("page",String(meta.page+1))}>Next</button></div>}</div></div></div>;
}
