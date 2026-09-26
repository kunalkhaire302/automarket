import type { Metadata } from "next";
import Link from "next/link";
import { Header } from "@/components/header";
import "./globals.css";
export const metadata: Metadata = { title: { default: "AutoMarket — Your next chapter, on wheels", template: "%s · AutoMarket" }, description: "Discover used cars, compare your options, and arrange a viewing with AutoMarket." };
export default function Layout({ children }: Readonly<{ children: React.ReactNode }>) {
  return <html lang="en-IN"><body><a className="skip-link" href="#main">Skip to content</a><Header/><main id="main">{children}</main><footer><div><Link href="/" className="footer-brand">auto<span>market</span> ↗</Link><p>A better way to find your next car.</p></div><div className="footer-links"><Link href="/buy">Explore cars</Link><Link href="/sell">Sell a car</Link><Link href="/locations">Visit a center</Link></div><small>© {new Date().getFullYear()} AutoMarket. Vehicle availability and estimates are subject to verification.</small></footer></body></html>;
}
