import { CarDetail } from "@/components/car-detail";
export default async function Detail({params}:{params:Promise<{id:string}>}){return <CarDetail id={(await params).id}/>;}
