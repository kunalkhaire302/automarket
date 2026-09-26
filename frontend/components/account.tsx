"use client";

import Link from "next/link";
import { useEffect, useState } from "react";
import { api, ApiError, label, money } from "@/lib/api";
import type { Appointment, Booking, Car, Notice, Submission, User, Wallet } from "@/lib/types";
import { CarCard, Empty, ErrorNotice, Loading } from "./ui";

type NotificationPreference = { category: string; inApp: boolean; push: boolean };

export function Account({ section }: { section: string }) {
  const [user, setUser] = useState<User>();
  const [cars, setCars] = useState<Car[]>([]);
  const [appointments, setAppointments] = useState<Appointment[]>([]);
  const [bookings, setBookings] = useState<Booking[]>([]);
  const [submissions, setSubmissions] = useState<Submission[]>([]);
  const [notices, setNotices] = useState<Notice[]>([]);
  const [preferences, setPreferences] = useState<NotificationPreference[]>([]);
  const [wallet, setWallet] = useState<Wallet>();
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [attempt, setAttempt] = useState(0);
  const [busy, setBusy] = useState(false);
  const [feedback, setFeedback] = useState("");

  useEffect(() => {
    let alive = true;
    setLoading(true);
    setError("");
    async function load() {
      try {
        const account = await api<User>("auth/me");
        if (!alive) return;
        setUser(account.data);
        if (section === "favourites") setCars((await api<Car[]>("favourites")).data);
        if (section === "appointments") setAppointments((await api<Appointment[]>("appointments/me")).data);
        if (section === "bookings") setBookings((await api<Booking[]>("bookings/me")).data);
        if (section === "seller-submissions") setSubmissions((await api<Submission[]>("seller/submissions")).data);
        if (section === "notifications") {
          const [notifications, settings] = await Promise.all([
            api<Notice[]>("notifications"), api<NotificationPreference[]>("notifications/preferences")
          ]);
          setNotices(notifications.data);
          setPreferences(settings.data);
        }
        if (section === "rewards") setWallet((await api<Wallet>("rewards/wallet")).data);
      } catch (reason) {
        if (reason instanceof ApiError && reason.status === 401) location.href = `/login?next=/profile/${section}`;
        else if (alive) setError(reason instanceof Error ? reason.message : "Please retry.");
      } finally {
        if (alive) setLoading(false);
      }
    }
    void load();
    return () => { alive = false; };
  }, [section, attempt]);

  async function action(path: string, confirm = false) {
    if (confirm && !window.confirm("Cancel this request? This cannot be undone.")) return;
    setBusy(true); setError("");
    try { await api(path, { method: "POST", body: "{}" }); setAttempt(value => value + 1); }
    catch (reason) { setError(reason instanceof Error ? reason.message : "Please retry."); }
    finally { setBusy(false); }
  }

  async function updatePreference(current: NotificationPreference, key: "inApp" | "push", checked: boolean) {
    setBusy(true); setError("");
    try {
      await api("notifications/preferences", { method: "PATCH", body: JSON.stringify({ ...current, [key]: checked }) });
      setPreferences(rows => rows.map(row => row.category === current.category ? { ...row, [key]: checked } : row));
    } catch (reason) { setError(reason instanceof Error ? reason.message : "Please retry."); }
    finally { setBusy(false); }
  }

  let content: React.ReactNode = null;
  if (section === "overview" && user) content = <div className="panel">
    <h2>Your details</h2>
    <dl className="details-specs"><div><dt>Name</dt><dd>{user.name}</dd></div><div><dt>Email</dt><dd>{user.email}</dd></div><div><dt>Phone</dt><dd>{user.phone || "Not provided"}</dd></div></dl>
    <div className="notice"><strong>Your referral code: {user.referralCode}</strong><p>Share this code with someone creating an account.</p><button className="text-button" onClick={async () => { try { await navigator.clipboard.writeText(user.referralCode); setFeedback("Code copied."); } catch { setFeedback("Copy the code shown above."); } }}>Copy code ↗</button>{feedback && <p role="status">{feedback}</p>}</div>
    <button style={{ marginTop: 25 }} className="button secondary" disabled={busy} onClick={async () => { setBusy(true); try { await api("auth/logout", { method: "POST", body: "{}" }); location.href = "/login"; } catch (reason) { setError(reason instanceof Error ? reason.message : "Sign out failed."); setBusy(false); } }}>Sign out</button>
  </div>;
  if (section === "favourites") content = cars.length ? <div className="car-grid">{cars.map(car => <div key={car.id}><CarCard car={car}/><button className="text-button" style={{ marginTop: 12 }} disabled={busy} onClick={async () => { setBusy(true); try { await api(`favourites/${car.id}`, { method: "DELETE" }); setAttempt(value => value + 1); } catch (reason) { setError(reason instanceof Error ? reason.message : "Please retry."); } finally { setBusy(false); } }}>Remove from saved cars</button></div>)}</div> : <Empty title="Keep the good ones close."><Link href="/buy">Find a car and save it for later.</Link></Empty>;
  if (section === "appointments") content = appointments.length ? <div className="panel">{appointments.map(item => <div className="record" key={item.id}><div><strong>{label(item.appointmentType)} · {new Date(item.startsAt).toLocaleString()}</strong><p>{item.notes}</p><Link className="text-link" href={`/buy/${item.carId}`}>View vehicle ↗</Link>{["CONFIRMED", "COMPLETED"].includes(item.status) && <p><Link className="button" href={`/booking?carId=${item.carId}`}>Continue to booking</Link></p>}</div><div><span className="tag">{label(item.status)}</span>{["PENDING", "CONFIRMED", "RESCHEDULED"].includes(item.status) && <p><button className="text-button" disabled={busy} onClick={() => action(`appointments/${item.id}/cancel`, true)}>Cancel appointment</button></p>}</div></div>)}</div> : <Empty title="Nothing on the calendar yet."><Link href="/buy">Find a car to arrange your first viewing.</Link></Empty>;
  if (section === "bookings") content = bookings.length ? <div className="panel">{bookings.map(item => <div className="record" key={item.id}><div><strong>{item.bookingReference}</strong><p>{money(item.amount)} · {new Date(item.createdAt).toLocaleDateString()}</p><Link href={`/buy/${item.carId}`} className="text-link">View vehicle ↗</Link></div><div><span className="tag">{label(item.status)}</span>{["PENDING", "CONFIRMED"].includes(item.status) && <p><button className="text-button" disabled={busy} onClick={() => action(`bookings/${item.id}/cancel`, true)}>Cancel booking</button></p>}</div></div>)}</div> : <Empty title="Your next chapter is waiting."><Link href="/buy">Browse cars to start your journey.</Link></Empty>;
  if (section === "seller-submissions") content = submissions.length ? <div className="panel">{submissions.map(item => <div className="record" key={item.id}><div><strong>Submission {item.id.slice(0, 8)}</strong><p>{item.reviewNotes || "Our team will update the review here."}</p>{["DRAFT", "NEEDS_INFORMATION"].includes(item.status) && <button className="button" disabled={busy} onClick={() => action(`seller/submissions/${item.id}/submit`)}>Submit for review</button>}</div><span className="tag">{label(item.status)}</span></div>)}</div> : <Empty title="Time for a new beginning?"><Link href="/sell">Start selling your car.</Link></Empty>;
  if (section === "rewards" && wallet) content = <div className="panel"><div className="eyebrow muted">AVAILABLE BALANCE</div><div className="price-big">{wallet.balance.toLocaleString()} points</div><p className="inline-status">Referral rewards are issued only after a referred buyer’s booking is completed by operations.</p>{wallet.balance >= 100 && <button className="button" disabled={busy} onClick={async () => { const raw = window.prompt("Points to redeem (increments of 100):", "100"); if (!raw) return; setBusy(true); try { await api("rewards/redeem", { method: "POST", body: JSON.stringify({ points: Number(raw), idempotencyKey: crypto.randomUUID() }) }); setAttempt(value => value + 1); } catch (reason) { setError(reason instanceof Error ? reason.message : "Please retry."); } finally { setBusy(false); } }}>Redeem points</button>}<div>{wallet.transactions.map(item => <div className="record" key={item.id}><div><strong>{label(item.type)}</strong><p>{new Date(item.createdAt).toLocaleString()}</p></div><strong>{item.points > 0 ? "+" : ""}{item.points}</strong></div>)}</div></div>;
  if (section === "notifications") content = <div className="panel">
    <h2>Notification preferences</h2>
    <p>In-app updates are available now. Push choices are saved and take effect when a delivery provider is configured.</p>
    {preferences.map(setting => <div className="record" key={setting.category}><strong>{label(setting.category)}</strong><div><label className="inline-status"><input type="checkbox" checked={setting.inApp} disabled={busy} onChange={event => updatePreference(setting, "inApp", event.target.checked)}/> In app</label> <label className="inline-status"><input type="checkbox" checked={setting.push} disabled={busy} onChange={event => updatePreference(setting, "push", event.target.checked)}/> Push</label></div></div>)}
    <h2 style={{ marginTop: 36 }}>Recent updates</h2>
    {notices.length ? <><button className="text-button" disabled={busy} onClick={() => action("notifications/read-all")}>Mark all as read</button>{notices.map(item => <div className="record" key={item.id}><div><strong>{label(item.title)}</strong><p>{item.message}</p><Link className="text-link" href={item.deepLink.startsWith("/") && !item.deepLink.startsWith("//") ? item.deepLink : "/profile"}>View details ↗</Link></div>{!item.readAt && <button className="text-button" disabled={busy} onClick={() => action(`notifications/${item.id}/read`)}>Mark read</button>}</div>)}</> : <Empty title="You’re all caught up.">Appointment, booking and listing updates will appear here.</Empty>}
  </div>;

  return <div className="container">
    <div className="page-heading"><div className="eyebrow muted">YOUR AUTOMARKET</div><h1>{user ? `Hello, ${user.name.split(" ")[0]}.` : "Your account."}</h1><p>Your cars, your appointments, your next chapter.</p></div>
    <nav className="tabs" aria-label="Account sections">{[["overview", "Overview"], ["favourites", "Saved cars"], ["appointments", "Appointments"], ["bookings", "Bookings"], ["seller-submissions", "My listings"], ["rewards", "Rewards"], ["notifications", "Notifications"]].map(([key, text]) => <Link className={section === key ? "active" : ""} key={key} href={`/profile/${key === "overview" ? "" : key}`}>{text}</Link>)}{user?.role === "ADMIN" && <Link href="/admin">Administration ↗</Link>}</nav>
    {error && <ErrorNotice message={error} retry={() => setAttempt(value => value + 1)}/>} {loading ? <Loading/> : content}
  </div>;
}
