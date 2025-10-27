function getAdminToken(){ return localStorage.getItem("markt_admin_token"); }
function clearAdminToken(){ localStorage.removeItem("markt_admin_token"); }

function adminHeaders(){
  const t = getAdminToken();
  const h = { "Content-Type": "application/json" };
  if (t) h["Authorization"] = `Bearer ${t}`;
  return h;
}

function ensureAdmin(){
  if (!getAdminToken()){
    window.location.replace("admin.html");
    return false;
  }
  return true;
}

// --- YENİ: basit flash/ toast ---
function flash(msg, type="ok"){
  const el = document.getElementById("flash");
  if (!el) return;
  el.className = `flash ${type}`;
  el.textContent = msg;
  el.style.display = "block";
  setTimeout(()=>{ el.style.display = "none"; }, 2200);
}

async function loadPending(){
  const res = await fetch(`/businesses?approved=false`, { headers: adminHeaders() });
  if (res.status === 401 || res.status === 403) { clearAdminToken(); return ensureAdmin(); }
  if (!res.ok) { flash(await res.text() || "load failed", "err"); return; }

  const list = await res.json();
  const ul = document.getElementById("biz-list");
  ul.innerHTML = "";
  if (!list.length){
    ul.innerHTML = `<li class="muted">No pending businesses</li>`;
    return;
  }
  list.forEach(b => {
    const li = document.createElement("li");
    li.dataset.id = b.id;
    li.dataset.name = b.displayName;
    li.innerHTML = `
      <div><strong>${b.displayName}</strong> — <span class="muted">${b.city || "-"}</span></div>
      <div class="muted">${b.sector || ""}</div>
      <div class="row" style="margin-top:6px">
        <button class="btn-approve">Approve</button>
        <button class="btn-reject">Reject</button>
      </div>
    `;
    ul.appendChild(li);
  });

  // APPROVE
  ul.querySelectorAll(".btn-approve").forEach(btn=>{
    btn.addEventListener("click", async ev=>{
      const li = ev.target.closest("li");
      const id = li.dataset.id;
      const name = li.dataset.name;

      btn.disabled = true;
      const orig = btn.textContent;
      btn.textContent = "Approving...";
      try {
        const r = await fetch(`/businesses/${id}/approve?value=true`, {
          method:"POST", headers: adminHeaders()
        });
        if (r.status === 401 || r.status === 403) { clearAdminToken(); return ensureAdmin(); }
        if (!r.ok) throw new Error(await r.text() || "approve failed");

        // pending listesinden kaldır
        li.remove();
        flash(`Approved: ${name}`, "ok");
        if (!document.querySelector("#biz-list li")) {
          document.getElementById("biz-list").innerHTML = `<li class="muted">No pending businesses</li>`;
        }
      } catch (e) {
        flash(e.message || "approve failed", "err");
        btn.disabled = false;
        btn.textContent = orig;
      }
    });
  });

  // REJECT
  ul.querySelectorAll(".btn-reject").forEach(btn=>{
    btn.addEventListener("click", async ev=>{
      const li = ev.target.closest("li");
      const id = li.dataset.id;
      const name = li.dataset.name;

      btn.disabled = true;
      const orig = btn.textContent;
      btn.textContent = "Rejecting...";
      try {
        const r = await fetch(`/businesses/${id}/approve?value=false`, {
          method:"POST", headers: adminHeaders()
        });
        if (r.status === 401 || r.status === 403) { clearAdminToken(); return ensureAdmin(); }
        if (!r.ok) throw new Error(await r.text() || "reject failed");

        // not-approved zaten pending; UI'dan kaldırmak istiyorsan:
        li.remove();
        flash(`Rejected: ${name}`, "ok");
        if (!document.querySelector("#biz-list li")) {
          document.getElementById("biz-list").innerHTML = `<li class="muted">No pending businesses</li>`;
        }
      } catch (e) {
        flash(e.message || "reject failed", "err");
        btn.disabled = false;
        btn.textContent = orig;
      }
    });
  });
}

document.addEventListener("DOMContentLoaded", ()=>{
  if (!ensureAdmin()) return;

  document.getElementById("a-logout")?.addEventListener("click", ()=>{
    clearAdminToken();
    window.location.replace("admin.html");
  });

  document.getElementById("btn-load")?.addEventListener("click", ()=>{
    loadPending().catch(e=>flash(e.message || "load failed", "err"));
  });

  document.getElementById("btn-reports")?.addEventListener("click", ()=>{
    Promise.resolve().then(loadReports).catch(e=>flash(e.message || "reports failed", "err"));
  });

  // Sayfa açılınca pending'i otomatik yüklemek istersen:
  // loadPending().catch(()=>{});
});

// Raporlar 
async function loadReports(){
  const r1 = await fetch(`/reports/top-offered?take=5`, { headers: adminHeaders() });
  if (r1.status === 401 || r1.status === 403) { clearAdminToken(); return ensureAdmin(); }
  if (!r1.ok) throw new Error(await r1.text() || "report1 failed");
  const topOffered = await r1.json();

  const list1 = document.getElementById("r-top-offered");
  list1.innerHTML = "";
  if (!topOffered.length) list1.innerHTML = `<li class="muted">No data</li>`;
  topOffered.forEach(x=>{
    const li = document.createElement("li");
    li.textContent = `businessId #${x.businessId} — ${x.count} offers`;
    list1.appendChild(li);
  });

  const r2 = await fetch(`/reports/top-offerers?take=5`, { headers: adminHeaders() });
  if (r2.status === 401 || r2.status === 403) { clearAdminToken(); return ensureAdmin(); }
  if (!r2.ok) throw new Error(await r2.text() || "report2 failed");
  const topOfferers = await r2.json();

  const list2 = document.getElementById("r-top-offerers");
  list2.innerHTML = "";
  if (!topOfferers.length) list2.innerHTML = `<li class="muted">No data</li>`;
  topOfferers.forEach(x=>{
    const li = document.createElement("li");
    li.textContent = `businessId #${x.businessId} — ${x.count} sent`;
    list2.appendChild(li);
  });
}
