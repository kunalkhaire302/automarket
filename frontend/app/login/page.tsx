import { Suspense } from "react";
import { AuthForm } from "@/components/auth-form";
export const metadata={title:"Sign in",robots:{index:false}};
export default function Login(){return <Suspense><AuthForm/></Suspense>;}
