// Backend taban adresin:
const API_BASE = ""; 

function setToken(t){ localStorage.setItem("markt_token", t); }
function getToken(){ return localStorage.getItem("markt_token"); }
function clearToken(){ localStorage.removeItem("markt_token"); }

// küçük jwt decode (sadece bilgi göstermek için)
function decodeJwt(token) {
  try {
    const payload = token.split(".")[1];
    const json = atob(payload.replace(/-/g, "+").replace(/_/g, "/"));
    return JSON.parse(json);
  } catch { return null; }
}

function authHeaders() {
  const t = getToken();
  const h = { "Content-Type": "application/json" };
  if (t) h["Authorization"] = `Bearer ${t}`;
  return h;
}

async function apiGet(path) {
  const res = await fetch(`${API_BASE}${path}`, { headers: authHeaders() });
  if (!res.ok) throw new Error(await res.text() || `${res.status} error`);
  return await res.json();
}

async function apiPost(path, body) {
  const res = await fetch(`${API_BASE}${path}`, {
    method:"POST", headers: authHeaders(), body: JSON.stringify(body)
  });
  if (!res.ok) throw new Error(await res.text() || `${res.status} error`);
  return await res.json();
}

async function apiPut(path, body) {
  const res = await fetch(`${API_BASE}${path}`, {
    method:"PUT", headers: authHeaders(), body: JSON.stringify(body)
  });
  if (!res.ok) {
    if (res.status === 204) return true;
    throw new Error(await res.text() || `${res.status} error`);
  }
  try { return await res.json(); } catch { return true; }
}

async function apiDelete(path) {
  const res = await fetch(`${API_BASE}${path}`, { method:"DELETE", headers: authHeaders() });
  if (!res.ok) {
    if (res.status === 204) return true;
    throw new Error(await res.text() || `${res.status} error`);
  }
  return true;
}
