import { NextRequest, NextResponse } from "next/server";
import { cookies } from "next/headers";

const origin = process.env.APP_ORIGIN ?? "http://localhost:3000";
const base = process.env.API_BASE_URL ?? "http://localhost:5100";
const safe = new Set(["GET", "HEAD", "OPTIONS"]);
type Context = { params: Promise<{ path: string[] }> };
async function handle(request: NextRequest, context: Context) {
  const path = (await context.params).path;
  if (path.some(p => !/^[a-zA-Z0-9_-]+$/.test(p))) return NextResponse.json({ success: false, error: { code: "BAD_PATH", message: "Invalid API path." } }, { status: 400 });
  if (!safe.has(request.method) && request.headers.get("origin") !== origin)
    return NextResponse.json({ success: false, error: { code: "CSRF_REJECTED", message: "Reload the page and try again." } }, { status: 403 });
  const jar = await cookies();
  const route = path.join("/");
  const headers: Record<string, string> = { "Content-Type": "application/json" };
  const access = jar.get("am_access")?.value;
  if (access) headers.Authorization = `Bearer ${access}`;
  const key = request.headers.get("idempotency-key");
  if (key) headers["Idempotency-Key"] = key;
  let body = safe.has(request.method) ? undefined : await request.text();
  if (body && Buffer.byteLength(body) > 65536) return NextResponse.json({ success: false, error: { code: "PAYLOAD_TOO_LARGE", message: "Request is too large." } }, { status: 413 });
  if (route === "auth/refresh" || route === "auth/logout") body = JSON.stringify({ refreshToken: jar.get("am_refresh")?.value ?? "" });
  try {
    const send = (token?: string) => {
      const outgoing = { ...headers };
      if (token) outgoing.Authorization = `Bearer ${token}`;
      else delete outgoing.Authorization;
      return fetch(`${base}/api/v1/${route}${request.nextUrl.search}`, { method: request.method, headers: outgoing, body, cache: "no-store", signal: AbortSignal.timeout(12000) });
    };
    let upstream = await send(access);
    let data = await upstream.json();
    let session = route.startsWith("auth/") && data.success && data.data?.accessToken ? data.data : null;
    let refreshRejected = false;
    const refreshToken = jar.get("am_refresh")?.value;
    if (upstream.status === 401 && refreshToken && !route.startsWith("auth/")) {
      const refresh = await fetch(`${base}/api/v1/auth/refresh`, {
        method: "POST", headers: { "Content-Type": "application/json" }, body: JSON.stringify({ refreshToken }),
        cache: "no-store", signal: AbortSignal.timeout(12000)
      });
      const refreshed = await refresh.json();
      if (refresh.ok && refreshed.success && refreshed.data?.accessToken) {
        session = refreshed.data;
        upstream = await send(session.accessToken);
        data = await upstream.json();
      } else refreshRejected = true;
    }
    if (route.startsWith("auth/") && session) data.data = session.user;
    const result = NextResponse.json(data, { status: upstream.status, headers: { "Cache-Control": "no-store" } });
    const cookieOptions = { httpOnly: true, secure: process.env.NODE_ENV === "production", sameSite: "lax" as const, path: "/" };
    if (session) {
      result.cookies.set("am_access", session.accessToken, { ...cookieOptions, maxAge: 15 * 60 });
      result.cookies.set("am_refresh", session.refreshToken, { ...cookieOptions, maxAge: 14 * 86400 });
    }
    if (route === "auth/logout" || refreshRejected) { result.cookies.delete("am_access"); result.cookies.delete("am_refresh"); }
    return result;
  } catch {
    return NextResponse.json({ success: false, error: { code: "SERVICE_UNAVAILABLE", message: "The marketplace service is unavailable. Please try again shortly." } }, { status: 503 });
  }
}
export const GET = handle;
export const POST = handle;
export const PATCH = handle;
export const DELETE = handle;
