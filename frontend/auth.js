document.getElementById("btn-to-login").addEventListener("click", ()=>{
  document.getElementById("signup-section").style.display="none";
  document.getElementById("login-section").style.display="block";
});

document.getElementById("btn-to-signup").addEventListener("click", ()=>{
  document.getElementById("login-section").style.display="none";
  document.getElementById("signup-section").style.display="block";
});

async function signUp({ displayName, email, password }) {
  // BusinessesController.Create → body Entity bekliyor
  const body = {
    displayName,
    email,
    passwordHash: password,    
    sector: "",
    city: "",
    phone: "",
    about: ""
  };
  const res = await fetch(`${API_BASE}/businesses`, {
    method: "POST",
    headers: { "Content-Type":"application/json" },
    body: JSON.stringify(body)
  });
  if (!res.ok) throw new Error(await res.text() || "sign up failed");
  return await res.json(); // Created dto dönüyor
}

// kayıt butonu ve inputlar
document.getElementById("btn-signup").addEventListener("click", async () => {
  const displayName = document.getElementById("su-display").value.trim();
  const email = document.getElementById("su-email").value.trim();
  const password = document.getElementById("su-pass").value;


  // alanlar dolu mu
  if (!displayName || !email || !password) { alert("display/email/password gerekli"); return; }
  
  // dto'dan sign up
  try {
    const dto = await signUp({ displayName, email, password });
    alert(`Kayıt oluşturuldu (#${dto.id}). Şimdi giriş yapabilirsin.`);
  } catch (e) {
    alert(e.message || "sign up failed");
  }
});


async function login(email, password) {
  const res = await fetch(`${API_BASE}/auth/login`, {
    method:"POST",
    headers:{ "Content-Type":"application/json" },
    body: JSON.stringify({ email, password })
  });
  if (!res.ok) throw new Error(await res.text() || "login failed");
  const j = await res.json(); 
  setToken(j.token);
  return j.token;
}

document.getElementById("btn-login").addEventListener("click", async () => {
  const email = document.getElementById("email").value.trim();
  const password = document.getElementById("password").value;
  const msg = document.getElementById("auth-msg");

  await login(email, password);
  refreshAuthUi();
  console.log("log in control");
  
});

async function getDevToken(businessId) {
  const j = await apiGet(`/auth/dev-token?businessId=${encodeURIComponent(businessId)}`);
  setToken(j.token);
  return j.token;
}

function refreshAuthUi() {
  const t = getToken();
  const info = document.getElementById("auth-info");
  const btnLogin = document.getElementById("btn-login");
  const btnLogout = document.getElementById("btn-logout");

  if (!t) {
    info.textContent = "Not logged in.";
    btnLogin.style.display = "inline-block";
    btnLogout.style.display = "none";
    return;
  }
  const payload = decodeJwt(t);
  const bid = payload?.bid || payload?.sub || "?";
  info.innerHTML = `<span class="ok">Logged in</span> — bid: <b>${bid}</b>`;
  btnLogin.style.display = "none";
  btnLogout.style.display = "inline-block";
}



document.getElementById("btn-logout").addEventListener("click", () => {
  clearToken();
  refreshAuthUi();
  window.dispatchEvent(new Event("markt:logout"));
});

document.getElementById("btn-devtoken").addEventListener("click", async () => {
  const bid = document.getElementById("dev-bid").value.trim();
  const msg = document.getElementById("auth-msg");
  msg.textContent = "";
  if (!bid) { msg.textContent = "dev bid required"; return; }
  try {
    await getDevToken(bid);
    refreshAuthUi();
    window.dispatchEvent(new Event("markt:login"));
  } catch (e) {
    msg.textContent = e.message || "dev token failed";
  }
});

// init
refreshAuthUi();
