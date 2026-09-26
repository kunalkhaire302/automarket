import { Account } from "@/components/account";
export const metadata={title:"Your account",robots:{index:false}};
export default async function Profile({params}:{params:Promise<{section?:string[]}>}){return <Account section={(await params).section?.[0]??"overview"}/>;}
