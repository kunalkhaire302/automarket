"use client";
import Link from "next/link";
import { useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { api } from "@/lib/api";
import { ErrorNotice } from "./ui";
export function AuthForm({register=false}:{register?:boolean}){
 const router=useRouter();const params=useSearchParams();const [error,setError]=useState("");const [busy,setBusy]=useState(false);
 const [showPassword,setShowPassword]=useState(false);
 const destination=params.get("next");
 const returnTo=destination?.startsWith("/")&&!destination.startsWith("//")?destination:"/profile";
 const switchUrl=`${register?"/login":"/signup"}?next=${encodeURIComponent(returnTo)}`;
 async function submit(e:React.FormEvent<HTMLFormElement>){e.preventDefault();setBusy(true);setError("");const values=Object.fromEntries(new FormData(e.currentTarget));try{await api(`auth/${register?"register":"login"}`,{method:"POST",body:JSON.stringify({...values,phone:values.phone??""})});const next=params.get("next");router.push(next?.startsWith("/")&&!next.startsWith("//")?next:"/profile");router.refresh();}catch(e){setError(e instanceof Error?e.message:"Please retry.");}finally{setBusy(false);}}
 return <div className="auth-wrap"><div className="eyebrow muted">{register?"YOUR JOURNEY STARTS HERE":"WELCOME BACK"}</div><h1>{register?"Make your next move.":"Good to see you."}</h1><p>{register?"Create an account to save cars, arrange viewings, and start selling.":"Sign in to pick up where you left off."}</p><form onSubmit={submit} aria-busy={busy}>{register&&<label className="field">Full name<input name="name" required maxLength={100} autoComplete="name"/></label>}<label className="field">Email address<input name="email" type="email" required maxLength={254} autoComplete="email" autoCapitalize="none" spellCheck={false}/></label>{register&&<label className="field">Phone (optional)<input name="phone" type="tel" maxLength={30} autoComplete="tel"/></label>}<div className="field"><label htmlFor="account-password">Password</label><div className="password-control"><input id="account-password" name="password" type={showPassword?"text":"password"} required minLength={register?12:1} maxLength={128} autoComplete={register?"new-password":"current-password"}/><button type="button" aria-controls="account-password" aria-pressed={showPassword} aria-label={showPassword?"Hide password":"Show password"} onClick={()=>setShowPassword(!showPassword)}>{showPassword?"Hide":"Show"}</button></div>{register&&<small>Use at least 12 characters.</small>}</div>{register&&<label className="field">Referral code (optional)<input name="referralCode" maxLength={20}/></label>}{error&&<ErrorNotice message={error}/>}<button className="button" disabled={busy}>{busy?"Please wait…":register?"Create account ↗":"Sign in ↗"}</button></form><div className="auth-foot">{register?"Already have an account?":"New to AutoMarket?"} <Link href={switchUrl}>{register?"Sign in":"Create an account"}</Link></div></div>;
}
