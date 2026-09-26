"use client";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { ArrowUpRight, Menu, X, CircleUserRound } from "lucide-react";
import { useRef, useState } from "react";
export function Header() {
  const path = usePathname();
  const [open, setOpen] = useState(false);
  const toggle = useRef<HTMLButtonElement>(null);
  return <header className="site-header"><Link href="/" className="wordmark" aria-label="AutoMarket home"><span className="brand-mark">a<span>m</span></span>auto<span>market</span><i>®</i></Link>
    <button ref={toggle} className="menu-toggle" aria-controls="main-navigation" aria-label={open ? "Close navigation" : "Open navigation"} aria-expanded={open} onClick={() => setOpen(!open)} onKeyDown={e=>{if(e.key==="Escape")setOpen(false);}}>{open ? <X/> : <Menu/>}</button>
    <nav id="main-navigation" aria-label="Main navigation" className={open ? "open" : ""} onKeyDown={e=>{if(e.key==="Escape"){setOpen(false);toggle.current?.focus();}}}>{[["/buy", "Find a car"], ["/sell", "Sell your car"], ["/compare", "Compare"], ["/locations", "Our locations"]].map(([url, text]) => <Link onClick={() => setOpen(false)} aria-current={path === url || path.startsWith(url+"/") ? "page" : undefined} key={url} href={url}>{text}</Link>)}</nav>
    <Link href="/profile" className="account-link"><CircleUserRound size={18}/><span>My account</span><ArrowUpRight size={15}/></Link></header>;
}
