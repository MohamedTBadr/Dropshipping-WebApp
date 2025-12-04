
// =======================
// CONFIG
// =======================
const API_BASE = 'https://localhost:7000/api/Brands';

// --- Get JWT token from localStorage ---
function authHeaders() {
    const token = localStorage.getItem("token");
    console.log(token);
    return {
        "Authorization": `Bearer ${token}`,
        
    };
}

// =======================
// DOM ELEMENTS
// =======================
const tbody = document.getElementById('tbody');
const searchInput = document.getElementById('searchInput');
const addProductBtn = document.getElementById('addProductBtn');

const overlayAddEdit = document.getElementById('modalOverlay');
const overlayView = document.getElementById('viewOverlay');
const overlayDelete = document.getElementById('deleteOverlay');

const modalTitle = document.getElementById('modalTitle');
const modalForm = document.getElementById('modalForm');
const productIdInput = document.getElementById('productId');
const pName = document.getElementById('pName');
const cancelModalBtn = document.getElementById('cancelModal');

const viewName = document.getElementById('viewName');
const closeViewBtn = document.getElementById('closeView');

const deleteMsg = document.getElementById('deleteMsg');
const confirmDeleteBtn = document.getElementById('confirmDelete');
const cancelDeleteBtn = document.getElementById('cancelDelete');

let currentEditId = null;
let deleteId = null;

// =======================
// HELPERS
// =======================
function escapeHtml(str) {
    if (!str) return '';
    return String(str).replace(/[&<>"'`=\/]/g, s => ({
        "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;", "/": "&#x2F;", "`": "&#x60;", "=": "&#x3D;"
    }[s]));
}

function renderRow(cat) {
    const tr = document.createElement('tr');
    tr.innerHTML = `
        <td>${escapeHtml(cat.name)}</td>
        <td style="text-align:right">
            <div class="actions">
                <button class="btn viewBtn" data-id="${escapeHtml(cat.id)}">👁️</button>
                <button class="btn editBtn" data-id="${escapeHtml(cat.id)}">✏️</button>
                <button class="btn delBtn" data-id="${escapeHtml(cat.id)}">🗑️</button>
            </div>
        </td>
    `;
    return tr;
}

function renderTable(categories) {
    tbody.innerHTML = '';
    if (!categories.length) {
        tbody.innerHTML = `<tr><td colspan="2" style="text-align:center;color:gray;padding:20px;">No Brands found.</td></tr>`;
        return;
    }
    categories.forEach(cat => tbody.appendChild(renderRow(cat)));
}

// =======================
// FETCH DATA (GET ALL)
// =======================
async function fetchAndRender() {
    try {
        const res = await fetch(API_BASE
            , {
            headers: authHeaders()
            }
        );

        if (!res.ok) throw new Error(res.statusText);

        const cats = await res.json();
        renderTable(cats.map(c => ({ id: c.id, name: c.name })));
    } catch (err) {
        console.error(err);
        renderTable([]);
    }
}

// =======================
// OPEN ADD MODAL
// =======================
addProductBtn.addEventListener('click', () => {
    currentEditId = null;
    modalTitle.textContent = 'Add Brand';
    productIdInput.value = '';
    pName.value = '';
    overlayAddEdit.classList.add('show');
});

// =======================
// OPEN EDIT MODAL
// =======================
async function openEditModal(id) {
    currentEditId = id;
    modalTitle.textContent = 'Edit Brand';

    try {
        const res = await fetch(`${API_BASE}/${id}`, {
            headers: authHeaders()
        });

        if (!res.ok) throw new Error(res.statusText);

        const cat = await res.json();
        productIdInput.value = cat.id;
        pName.value = cat.name || '';
        overlayAddEdit.classList.add('show');
    } catch (err) {
        console.error(err);
        alert('Failed to load Brand.');
    }
}

// =======================
// SUBMIT ADD / EDIT FORM
// =======================
modalForm.addEventListener('submit', async e => {
    e.preventDefault();

    const name = pName.value.trim();
    const id = currentEditId;
    if (!name) { alert('Name is required'); return; }

    const formData = new URLSearchParams();
    if (currentEditId) formData.append('id', id);
    formData.append('name', name);

    try {
        const method = currentEditId ? 'PUT' : 'POST';

        const res = await fetch(API_BASE, {
            method,
            headers: {
                ...authHeaders({}),
                'Content-Type': 'application/x-www-form-urlencoded'
            },
            body: formData.toString()
        });

        if (!res.ok) throw new Error(res.statusText);

        overlayAddEdit.classList.remove('show');
        currentEditId = null;
        fetchAndRender();
    } catch (err) {
        console.error(err);
        alert('Failed to save Brand.');
    }
});

// =======================
// VIEW BRAND MODAL
// =======================
function openViewModal(id) {
    fetch(`${API_BASE}/${id}`, {
        headers: authHeaders()
    })
        .then(res => res.json())
        .then(cat => {
            viewName.textContent = cat.name || '';
            overlayView.classList.add('show');
        })
        .catch(err => {
            console.error(err);
            alert('Failed to load Brand.');
        });
}

// =======================
// DELETE MODAL
// =======================
function openDeleteModal(id) {
    deleteId = id;
    deleteMsg.textContent = 'Are you sure you want to delete this category?';
    overlayDelete.classList.add('show');
}

confirmDeleteBtn.addEventListener('click', async () => {
    if (!deleteId) return;

    try {
        const res = await fetch(`${API_BASE}/${deleteId}`, {
            method: 'DELETE',
            headers: authHeaders()
        });

        if (!res.ok) throw new Error(res.statusText);

        overlayDelete.classList.remove('show');
        deleteId = null;
        fetchAndRender();
    } catch (err) {
        console.error(err);
        alert('Failed to delete Brand.');
    }
});

// =======================
// TABLE BUTTON ACTIONS
// =======================
tbody.addEventListener('click', e => {
    const btn = e.target.closest('button');
    if (!btn || !btn.dataset.id) return;

    const id = btn.dataset.id;

    if (btn.classList.contains('viewBtn')) openViewModal(id);
    else if (btn.classList.contains('editBtn')) openEditModal(id);
    else if (btn.classList.contains('delBtn')) openDeleteModal(id);
});

// =======================
// CLOSE MODALS
// =======================
[cancelModalBtn, closeViewBtn, cancelDeleteBtn].forEach(btn => {
    btn.addEventListener('click', () => {
        overlayAddEdit.classList.remove('show');
        overlayView.classList.remove('show');
        overlayDelete.classList.remove('show');
    });
});

[overlayAddEdit, overlayView, overlayDelete].forEach(ov =>
    ov.addEventListener('click', e => {
        if (e.target === ov) ov.classList.remove('show');
    })
);

// =======================
// SEARCH FUNCTION
// =======================
searchInput.addEventListener('input', () => {
    const term = searchInput.value.trim().toLowerCase();
    Array.from(tbody.children).forEach(tr => {
        const name = tr.children[0].textContent.toLowerCase();
        tr.style.display = name.includes(term) ? '' : 'none';
    });
});

// =======================
// INIT
// =======================
fetchAndRender();
