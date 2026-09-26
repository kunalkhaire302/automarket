import Link from "next/link";
export default function NotFound(){return <div className="container section"><div className="eyebrow muted">A WRONG TURN</div><h1>This road ends here.</h1><p style={{margin:"20px 0"}}>The page you’re looking for isn’t available.</p><Link className="button" href="/buy">Back to the marketplace ↗</Link></div>;}
