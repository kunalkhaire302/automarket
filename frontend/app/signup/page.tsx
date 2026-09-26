import { Suspense } from "react";
import { AuthForm } from "@/components/auth-form";
export const metadata={title:"Create an account",robots:{index:false}};
export default function Signup(){return <Suspense><AuthForm register/></Suspense>;}
