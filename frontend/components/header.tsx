"use client";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { ArrowUpRight, Menu, X, CircleUserRound } from "lucide-react";
import { useState } from "react";
export function Header() {
  const path = usePathname();
  const [open, setOpen] = useState(false);
  return <header className="site-header"><Link href="/" className="wordmark" aria-label="AutoMarket home"><span className="brand-mark">a<span>m</span></span>auto<span>market</span><i>®</i></Link>
    <button className="menu-toggle" aria-label={open ? "Close navigation" : "Open navigation"} aria-expanded={open} onClick={() => setOpen(!open)}>{open ? <X/> : <Menu/>}</button>
    <nav aria-label="Main navigation" className={open ? "open" : ""}>{[["/buy", "Find a car"], ["/sell", "Sell your car"], ["/compare", "Compare"], ["/locations", "Our locations"]].map(([url, text]) => <Link onClick={() => setOpen(false)} aria-current={path === url ? "page" : undefined} key={url} href={url}>{text}</Link>)}</nav>
    <Link href="/profile" className="account-link"><CircleUserRound size={18}/><span>My account</span><ArrowUpRight size={15}/></Link></header>;
}
