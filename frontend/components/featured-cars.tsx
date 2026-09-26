"use client";
import { useEffect, useState } from "react";
import { api } from "@/lib/api";
import type { Car } from "@/lib/types";
import { CarCard, Empty, ErrorNotice, Loading } from "./ui";
export function FeaturedCars() {
  const [cars, setCars] = useState<Car[] | null>(null);
  const [error, setError] = useState("");
  const [attempt, setAttempt] = useState(0);
  useEffect(() => {
    const c = new AbortController();
    setError("");
    const load = async () => {
      try {
        const featured = await api<Car[]>("cars?featured=true&pageSize=12&sort=newest", { signal: c.signal });
        if (!c.signal.aborted) setCars(featured.data);
      } catch {
        try {
          const newest = await api<Car[]>("cars?pageSize=12&sort=newest", { signal: c.signal });
          if (!c.signal.aborted) setCars(newest.data);
        } catch (e) {
          if (!c.signal.aborted) setError(e instanceof Error ? e.message : "Unable to load vehicles right now.");
        }
      }
    };
    void load();
    return () => c.abort();
  }, [attempt]);
  if (error) return <ErrorNotice message={error} retry={() => setAttempt(x => x + 1)}/>;
  if (!cars) return <Loading/>;
  if (!cars.length) return <Empty title="The next arrival could be yours.">New listings will appear here after review. Explore our service locations or start selling your car.</Empty>;
  return <div className="car-grid">{cars.map(c => <CarCard key={c.id} car={c}/>)}</div>;
}
