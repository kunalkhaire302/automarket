import { Suspense } from "react";
import { Browse } from "@/components/browse";
import { Loading } from "@/components/ui";
export const metadata = { title: "Find your next car" };
export default function Buy() { return <Suspense fallback={<Loading/>}><Browse/></Suspense>; }
