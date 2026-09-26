import type { Envelope } from "./types";
export class ApiError extends Error {
  constructor(message: string, public status: number, public code: string) { super(message); }
}
export async function api<T>(path: string, init?: RequestInit): Promise<Envelope<T>> {
  const response = await fetch(`/api/backend/${path}`, {
    ...init, headers: { "Content-Type": "application/json", ...init?.headers },
    cache: "no-store", signal: init?.signal ?? AbortSignal.timeout(15000)
  });
  const data = await response.json() as Envelope<T>;
  if (!response.ok || !data.success) throw new ApiError(data.error?.message ?? "We couldn't complete that request. Please retry.", response.status, data.error?.code ?? "NETWORK_ERROR");
  return data;
}
export const money = (value: number) => new Intl.NumberFormat("en-IN", { style: "currency", currency: "INR", maximumFractionDigits: 0 }).format(value);
export const label = (value: string) => value.toLowerCase().replaceAll("_", " ").replace(/^./, c => c.toUpperCase());
