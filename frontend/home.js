// küçük yardımcılar
function authHeaders() {
  const h = { "Content-Type": "application/json" };
  const t = getToken();
  if (t) h["Authorization"] = `Bearer ${t}`;
  return h;
}
async function apiGet(path) {
  const res = await fetch(`${API_BASE}${path}`, { headers: authHeaders() });
  if (!res.ok) throw new Error(await res.text() || `${res.status} error`);
  return await res.json();
}
async function apiPost(path, body) {
  const res = await fetch(`${API_BASE}${path}`, { method:"POST", headers: authHeaders(), body: JSON.stringify(body) });
  if (!res.ok) throw new Error(await res.text() || `${res.status} error`);
  return await res.json();
}

// ------- PROFIL BUTONU --------
document.getElementById("btn-profile").addEventListener("click", () => {
  const t = getToken();
  if (!t) { alert("Önce giriş yap."); return; }
  try {
    const payload = JSON.parse(atob(t.split(".")[1].replace(/-/g,"+").replace(/_/g,"/")));
    const bid = payload?.bid || payload?.sub || "?";
    alert(`Profil — BusinessId: ${bid}\n(Şimdilik basit bilgi. İleride profil sayfasına yönlendirebilirsin.)`);
  } catch {
    alert("Token okunamadı.");
  }
});

// ------- SEARCH --------
const searchInput = document.getElementById("search-input");
const btnSearch = document.getElementById("btn-search");
const boxResults = document.getElementById("search-results");
const ulProd = document.getElementById("res-products");
const ulBiz = document.getElementById("res-businesses");

btnSearch.addEventListener("click", doSearch);
searchInput.addEventListener("keydown", (e) => { if (e.key === "Enter") doSearch(); });

async function doSearch() {
  const q = (searchInput.value || "").trim();
  if (!q) { boxResults.hidden = true; ulProd.innerHTML = ""; ulBiz.innerHTML = ""; return; }
  try {
    // backend: GET /search?q=
    const res = await apiGet(`/search?q=${encodeURIComponent(q)}`);
    renderSearchResults(res);
  } catch (e) {
    alert(e.message || "search failed");
  }
}
function renderSearchResults({ products, businesses }) {
  boxResults.hidden = false;
  ulProd.innerHTML = "";
  ulBiz.innerHTML = "";

  (products || []).forEach(p => {
    const li = document.createElement("li");
    li.innerHTML = `
      <div><strong>${p.title}</strong> — ${p.price ?? "-"} ₺</div>
      <div class="muted">${p.description ?? ""}</div>
      <div class="badge">Business #${p.businessId}</div>
    `;
    ulProd.appendChild(li);
  });

  (businesses || []).forEach(b => {
    const li = document.createElement("li");
    li.innerHTML = `
      <div><strong>${b.displayName}</strong></div>
      <div class="muted">${b.sector ?? ""} — ${b.city ?? ""} — ${b.phone ?? ""}</div>
    `;
    ulBiz.appendChild(li);
  });
}

// ------- FEED (POSTS) --------
const feed = document.getElementById("feed");

async function loadFeed() {
  try {
    // backend: GET /posts (opsiyonel: ?businessId=)
    const posts = await apiGet(`/posts`);
    renderFeed(posts);
  } catch (e) {
    feed.innerHTML = `<li class="error">${e.message || "feed yüklenemedi"}</li>`;
  }
}

function renderFeed(posts) {
  feed.innerHTML = "";
  (posts || []).forEach(p => {
    const li = document.createElement("li");
    li.innerHTML = `
      <div class="title">${p.title}</div>
      <div class="meta">Business #${p.businessId} • ${new Date(p.createdAt || Date.now()).toLocaleString()}</div>
      <div class="content">${(p.content || "")}</div>
      <div class="row">
        <button class="btn btn-like" data-id="${p.id}">Beğen / Geri Al</button>
        <span class="badge" id="lk-${p.id}">likes: ?</span>
      </div>
    `;
    feed.appendChild(li);
  });

  // like butonları
  document.querySelectorAll(".btn-like").forEach(btn => {
    btn.addEventListener("click", async (ev) => {
      const id = ev.target.dataset.id;
      const token = getToken();
      if (!token) { alert("Like için giriş yapmalısın."); return; }
      try {
        const j = await apiPost(`/likes/toggle/${id}`, {});
        const badge = document.getElementById(`lk-${id}`);
        badge.textContent = `likes: ${j.likes}`;
      } catch (e) {
        alert(e.message || "like failed");
      }
    });
  });

  // ilk açılışta mevcut like sayısını çekmek istersen: ayrı bir endpoint yok,
  // basit yol: toggle sonrası güncelleniyor. İstersen ReportsController ile de gösterebilirsin.
}

// ilk yükleme
loadFeed();
