// ==================== AdminOrders.js ====================

const API_ORDERS = "https://localhost:7000/api/order";
const tbody = document.getElementById('tbody');
const searchInput = document.getElementById('searchInput');
const paginationContainer = document.getElementById('pagination');

const overlayView = document.getElementById('viewOverlay');
const overlayDelete = document.getElementById('deleteOverlay');

const viewProducts = document.getElementById('viewProducts');
const viewDropshipper = document.getElementById('viewDropshipper');
const viewCustomer = document.getElementById('viewCustomer');
const viewAddress = document.getElementById('viewAddress');
const viewPrice = document.getElementById('viewPrice');
const viewDiscount = document.getElementById('viewDiscount');
const viewStatus = document.getElementById('viewStatus');
const viewShipped = document.getElementById('viewShipped');
const closeViewBtn = document.getElementById('closeView');

const deleteMsg = document.getElementById('deleteMsg');
const confirmDeleteBtn = document.getElementById('confirmDelete');
const cancelDeleteBtn = document.getElementById('cancelDelete');

let deleteId = null;
let currentPage = 1;
const pageSize = 10;
let allOrders = [];

// ----------------- Auth Headers -----------------
function authHeaders() {
    const token = localStorage.getItem("token");
    return {
        "Authorization": `Bearer ${token}`
    };
}

// ----------------- Escape HTML -----------------
function escapeHtml(str) {
    if (!str) return '';
    return String(str).replace(/[&<>"'`=\/]/g, s => ({
        "&": "&amp;",
        "<": "&lt;",
        ">": "&gt;",
        '"': "&quot;",
        "'": "&#39;",
        "/": "&#x2F;",
        "`": "&#x60;",
        "=": "&#x3D;"
    }[s]));
}

// ----------------- Render Row with Status Buttons -----------------
function renderRow(order, index) {
    const productNames = order.items?.length > 0
        ? order.items.map(i => i.productName || "Unnamed").join(", ")
        : "No products";

    const shippedDate = order.shippedDate ? new Date(order.shippedDate).toLocaleDateString() : "Not shipped";

    const tr = document.createElement('tr');
    tr.innerHTML = `
        <td>${index + 1}</td>
        <td>${escapeHtml(productNames)}</td>
        <td>${escapeHtml(order.dropshipperName || "Unknown")}</td>
        <td>${escapeHtml(order.customerName || "Unknown")}</td>
        <td>${escapeHtml(order.customerAddress || "Unknown")}</td>
        <td>${order.orderPrice?.toFixed(2) || "0.00"}</td>
        <td>${order.orderDiscount?.toFixed(2) || "0.00"}</td>
        <td>
            <button class="btn-status" data-id="${order.id}" data-status="0" style="background:#f44336;color:white;margin-right:4px;">Declined</button>
            <button class="btn-status" data-id="${order.id}" data-status="1" style="background:#ff9800;color:white;margin-right:4px;">Delivering</button>
            <button class="btn-status" data-id="${order.id}" data-status="2" style="background:#4caf50;color:white;">Delivered</button>
        </td>
        <td>${shippedDate}</td>
        <td style="text-align:right">
            <button class="btn viewBtn" data-id="${order.id}">👁️</button>
            <button class="btn delBtn" data-id="${order.id}">🗑️</button>
        </td>
    `;
    return tr;
}

// ----------------- Render Table -----------------
function renderTable(orders) {
    tbody.innerHTML = '';
    if (!orders.length) {
        tbody.innerHTML = `<tr><td colspan="10" style="text-align:center;color:gray;padding:20px;">No orders found.</td></tr>`;
        return;
    }
    orders.forEach((order, index) => tbody.appendChild(renderRow(order, index)));
    attachRowEvents();
}

// ----------------- Attach Row Events -----------------
function attachRowEvents() {
    tbody.querySelectorAll(".viewBtn").forEach(btn => btn.addEventListener('click', () => openViewModal(btn.dataset.id)));
    tbody.querySelectorAll(".delBtn").forEach(btn => btn.addEventListener('click', () => openDeleteModal(btn.dataset.id)));
    tbody.querySelectorAll(".btn-status").forEach(btn => btn.addEventListener('click', () => openStatusPrompt(btn.dataset.id, btn.dataset.status)));
}

// ----------------- Fetch Orders -----------------
async function fetchOrders(page = 1) {
    try {
        const res = await fetch(`${API_ORDERS}?page=${page}&pageSize=${pageSize}`, { headers: authHeaders() });
        if (!res.ok) throw new Error("Failed to fetch orders");

        const data = await res.json();
        allOrders = data.result || [];
        renderTable(allOrders);
        renderPagination(data.pageIndex || page, data.totalCount || 0, data.pageSize || pageSize);
    } catch (err) {
        console.error(err);
        tbody.innerHTML = `<tr><td colspan="10" style="text-align:center;color:red;padding:20px;">Failed to load orders.</td></tr>`;
    }
}

// ----------------- Render Pagination -----------------
function renderPagination(pageIndex, totalCount, pageSize) {
    paginationContainer.innerHTML = '';
    const totalPages = Math.ceil(totalCount / pageSize);
    if (totalPages <= 1) return;

    const createButton = (text, page) => {
        const btn = document.createElement('button');
        btn.textContent = text;
        btn.className = page === pageIndex ? 'active btn-page' : 'btn-page';
        btn.addEventListener('click', () => {
            currentPage = page;
            fetchOrders(page);
        });
        return btn;
    };

    if (pageIndex > 1) paginationContainer.appendChild(createButton('⟨', pageIndex - 1));
    for (let i = 1; i <= totalPages; i++) paginationContainer.appendChild(createButton(i, i));
    if (pageIndex < totalPages) paginationContainer.appendChild(createButton('⟩', pageIndex + 1));
}

// ----------------- View Modal -----------------
function openViewModal(id) {
    const order = allOrders.find(o => o.id === id);
    if (!order) return;

    viewProducts.textContent = order.items?.map(i => i.productName).join(", ") || "No products";
    viewDropshipper.textContent = order.dropshipperName || "Unknown";
    viewCustomer.textContent = order.customerName || "Unknown";
    viewAddress.textContent = order.customerAddress || "Unknown";
    viewPrice.textContent = order.orderPrice?.toFixed(2) || "0.00";
    viewDiscount.textContent = order.orderDiscount?.toFixed(2) || "0.00";
    viewStatus.textContent = ["Declined", "Delivering", "Delivered"][order.orderStatus] || "Pending";
    viewShipped.textContent = order.shippedDate ? new Date(order.shippedDate).toLocaleDateString() : "Not shipped";

    overlayView.classList.add('show');
}

// ----------------- Delete Modal -----------------
function openDeleteModal(id) {
    deleteId = id;
    deleteMsg.textContent = 'Are you sure you want to delete this order?';
    overlayDelete.classList.add('show');
}

confirmDeleteBtn.addEventListener('click', async () => {
    if (!deleteId) return;
    try {
        const res = await fetch(`${API_ORDERS}/${deleteId}`, { method: 'DELETE', headers: authHeaders() });
        if (!res.ok) throw new Error("Failed to delete order");
        overlayDelete.classList.remove('show');
        deleteId = null;
        fetchOrders(currentPage);
    } catch (err) {
        console.error(err);
        alert('Failed to delete order.');
    }
});

// ----------------- Close Modals -----------------
[closeViewBtn, cancelDeleteBtn].forEach(btn => btn.addEventListener('click', () => {
    overlayView.classList.remove('show');
    overlayDelete.classList.remove('show');
}));

// Click outside modal closes it
[overlayView, overlayDelete].forEach(ov =>
    ov.addEventListener('click', e => { if (e.target === ov) ov.classList.remove('show'); })
);

// ----------------- Update Order Status Prompt -----------------
async function openStatusPrompt(orderId, status) {
    let shippedDate = null;
    if (parseInt(status) === 1 || parseInt(status) === 2) {
        shippedDate = prompt("Enter shipped date (YYYY-MM-DD):", new Date().toISOString().split("T")[0]);
        if (!shippedDate) return;
    }
    try {
        const res = await fetch(`${API_ORDERS}/${orderId}`, {
            method: 'PUT',
            headers: {
                ...authHeaders(),
                'Content-Type': 'application/json'
            },
            body: JSON.stringify({
                orderStatus: parseInt(status),
                shippedDate: shippedDate
            })
        });
        if (!res.ok) throw new Error('Failed to update order');
        alert('Order updated successfully!');
        fetchOrders(currentPage);
    } catch (err) {
        console.error(err);
        alert('Failed to update order.');
    }
}

// ----------------- Search -----------------
searchInput.addEventListener('input', () => {
    const term = searchInput.value.trim().toLowerCase();
    Array.from(tbody.children).forEach(tr => {
        const text = tr.children[1].textContent.toLowerCase();
        tr.style.display = text.includes(term) ? '' : 'none';
    });
});

// ----------------- Initial Fetch -----------------
fetchOrders(currentPage);
