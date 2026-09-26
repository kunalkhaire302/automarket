"use client";
import Link from "next/link";
import { ArrowUpRight, CarFront, Heart, GitCompareArrows, Gauge, Fuel, MapPin } from "lucide-react";
import { useState } from "react";
import { api, ApiError, label, money } from "@/lib/api";
import type { Car } from "@/lib/types";
export function ErrorNotice({ message, retry }: { message: string; retry?: () => void }) {
  return <div className="notice error" role="alert"><p>{message}</p>{retry && <button className="text-button" onClick={retry}>Try again ↗</button>}</div>;
}
export function Empty({ title, children }: { title: string; children: React.ReactNode }) {
  return <div className="empty"><CarFront size={38} strokeWidth={1.2}/><h3>{title}</h3><p>{children}</p></div>;
}
export function Loading() { return <div className="skeleton-grid" role="status" aria-label="Loading"><span/><span/><span/></div>; }
export function CarCard({ car }: { car: Car }) {
  const [message, setMessage] = useState("");
  const [busy, setBusy] = useState(false);
  async function save(kind: string) {
    setBusy(true);
    try { await api(`${kind}/${car.id}`, { method: "POST" }); setMessage(kind === "compare" ? "Added to comparison" : "Saved to your favourites"); }
    catch (e) { if (e instanceof ApiError && e.status === 401) location.href = `/login?next=/buy/${car.id}`; else setMessage(e instanceof Error ? e.message : "Please retry."); }
    finally { setBusy(false); }
  }
  return <article className="car-card"><Link href={`/buy/${car.id}`} className="car-visual" aria-label={`View ${car.brand} ${car.model}`}>
    {car.primaryImageUrl?<img src={car.primaryImageUrl} alt={`Illustrative view of ${car.brand} ${car.model}`} />:<><CarFront size={100} strokeWidth={0.75}/><span className="image-caption">Vehicle photos pending</span></>}<span className="tag">{label(car.bodyType)}</span>
  </Link><div className="card-body"><div className="eyebrow muted">{car.registrationYear} · {car.variant || label(car.transmission)}</div><Link href={`/buy/${car.id}`} className="car-title">{car.brand} {car.model}<ArrowUpRight size={19}/></Link>
    <div className="spec-line"><span><Gauge size={14}/>{car.kilometers.toLocaleString("en-IN")} km</span><span><Fuel size={14}/>{label(car.fuelType)}</span></div>
    <div className="card-bottom"><strong>{money(car.price)}</strong><span><MapPin size={13}/>{car.city}</span></div>
    <div className="card-actions"><button disabled={busy} onClick={() => save("favourites")}><Heart size={15}/>Save</button><button disabled={busy} onClick={() => save("compare")}><GitCompareArrows size={15}/>Compare</button></div>
    {message && <p className="feedback" role="status">{message}</p>}
  </div></article>;
}
