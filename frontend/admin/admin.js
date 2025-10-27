function setAdminToken(t){ localStorage.setItem("markt_admin_token", t); }

async function adminLogin(email, password){
  const res = await fetch(`/admin/login`, {
    method:"POST",
    headers:{ "Content-Type":"application/json" },
    body: JSON.stringify({ email, password })
  });
  if (!res.ok) throw new Error(await res.text() || "admin login failed");
  const j = await res.json();
  setAdminToken(j.token);
  return j.token;
}

document.addEventListener("DOMContentLoaded", ()=>{
  document.getElementById("a-login")?.addEventListener("click", async ()=>{
    const email = document.getElementById("a-email")?.value.trim();
    const pass  = document.getElementById("a-pass")?.value;
    const msg = document.getElementById("a-msg"); if (msg) msg.textContent = "";
    try {
      await adminLogin(email, pass);
      // başarılı → admin paneline geç
      window.location.href = "admin-panel.html";
    } catch (e) {
      if (msg) msg.textContent = e.message || "admin login failed";
      else alert(e.message || "admin login failed");
    }
  });
});
