function uiAlert(err){ alert(err?.message || err || "error"); }

async function loadProducts(){
  const filter = document.getElementById("filter-business").value.trim();
  let path = "/products";
  if (filter) path += `?businessId=${encodeURIComponent(filter)}`;

  try {
    const list = await apiGet(path); // ProductDto[]
    const ul = document.getElementById("products-list");
    ul.innerHTML = "";
    list.forEach(p => {
      const li = document.createElement("li");
      li.innerHTML = `
        <div><strong>${p.title}</strong> — ${p.price} ₺</div>
        <div class="muted">By: <span class="badge">${p.businessName ?? p.businessId}</span></div>
        <div class="muted">${p.description ?? ""}</div>
        <div class="row" style="margin-top:6px">
          <input class="edit-title" data-eid="${p.id}" value="${p.title}" />
          <input class="edit-price" data-eid="${p.id}" type="number" value="${p.price}" />
          <input class="edit-desc"  data-eid="${p.id}" value="${p.description ?? ""}" />
          <button class="btn-update" data-id="${p.id}">Update</button>
          <button class="btn-delete" data-id="${p.id}">Delete</button>
        </div>
      `;
      ul.appendChild(li);
    });

    document.querySelectorAll(".btn-delete").forEach(btn => {
      btn.addEventListener("click", async (ev) => {
        const id = ev.target.dataset.id;
        try {
          await apiDelete(`/products/${id}`);
          loadProducts();
        } catch (e) { uiAlert(e); }
      });
    });

    document.querySelectorAll(".btn-update").forEach(btn => {
      btn.addEventListener("click", async (ev) => {
        const id = ev.target.dataset.id;
        const title = document.querySelector(`.edit-title[data-eid="${id}"]`).value.trim();
        const price = Number(document.querySelector(`.edit-price[data-eid="${id}"]`).value);
        const desc  = document.querySelector(`.edit-desc[data-eid="${id}"]`).value.trim();
        try {
          await apiPut(`/products/${id}`, { id:Number(id), title, description:desc, price });
          loadProducts();
        } catch (e) { uiAlert(e); }
      });
    });

  } catch (e) { uiAlert(e); }
}

document.getElementById("btn-refresh").addEventListener("click", loadProducts);
document.getElementById("btn-create").addEventListener("click", async () => {
  const title = document.getElementById("p-title").value.trim();
  const price = Number(document.getElementById("p-price").value);
  const desc  = document.getElementById("p-desc").value.trim();
  if (!title) return uiAlert("Title required");
  try {
    await apiPost(`/products`, { title, price, description: desc });
    document.getElementById("p-title").value = "";
    document.getElementById("p-price").value = "";
    document.getElementById("p-desc").value  = "";
    loadProducts();
  } catch (e) { uiAlert(e); }
});

window.addEventListener("load", loadProducts);
window.addEventListener("markt:login", loadProducts);
window.addEventListener("markt:logout", loadProducts);
