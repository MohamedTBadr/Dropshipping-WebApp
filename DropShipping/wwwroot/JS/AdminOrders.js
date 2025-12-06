// ==================== AdminOrders.js ====================
const API_ORDERS = "https://localhost:7000/api/order";
const tbody = document.getElementById('tbody');
const searchInput = document.getElementById('searchInput');
const paginationContainer = document.getElementById('pagination');
const loadingEl = document.getElementById('loading');

const overlayView = document.getElementById('viewOverlay');
const overlayEdit = document.getElementById('editOverlay');
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

const editStatus = document.getElementById('editStatus');
const editShippedDate = document.getElementById('editShippedDate');
const cancelEditBtn = document.getElementById('cancelEdit');
const saveEditBtn = document.getElementById('saveEdit');

const deleteMsg = document.getElementById('deleteMsg');
const confirmDeleteBtn = document.getElementById('confirmDelete');
const cancelDeleteBtn = document.getElementById('cancelDelete');

let deleteId = null;
let editId = null;
let currentPage = 1;
const pageSize = 1;
let allOrders = [];
let filteredOrders = [];
let currentSearch = '';

// ----------------- Auth Headers -----------------
function authHeaders() {
    const token = localStorage.getItem("token");
    return {
        "Authorization": `Bearer ${token}`,
        "Content-Type": "application/json"
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

// ----------------- Get Status Badge Class -----------------
function getStatusClass(status) {
    switch (parseInt(status)) {
        case 0: return 'status-declined';
        case 1: return 'status-delivering';
        case 2: return 'status-delivered';
        default: return 'status-declined';
    }
}

// ----------------- Get Status Text -----------------
function getStatusText(status) {
    switch (parseInt(status)) {
        case 0: return 'Declined';
        case 1: return 'Delivering';
        case 2: return 'Delivered';
        default: return 'Declined';
    }
}

// ----------------- Show Loading -----------------
function showLoading() {
    if (loadingEl) loadingEl.style.display = 'flex';
}

// ----------------- Hide Loading -----------------
function hideLoading() {
    if (loadingEl) loadingEl.style.display = 'none';
}

// ----------------- Show Notification -----------------
function showNotification(message, type = 'success') {
    const notification = document.createElement('div');
    notification.className = `notification ${type}`;
    notification.innerHTML = `
                                <i class="fas fa-${type === 'success' ? 'check-circle' : 'exclamation-circle'}"></i>
                                <span>${message}</span>
                            `;

    document.body.appendChild(notification);

    setTimeout(() => {
        notification.classList.add('show');
    }, 10);

    setTimeout(() => {
        notification.classList.remove('show');
        setTimeout(() => notification.remove(), 300);
    }, 4000);
}

// ----------------- Render Row with Edit Icons -----------------
function renderRow(order, index) {
    const startIndex = (currentPage - 1) * pageSize;
    const actualIndex = startIndex + index + 1;

    const productNames = order.items?.length > 0
        ? order.items.map(i => i.productName || "Unnamed").join(", ")
        : "No products";

    const shippedDate = order.shippedDate ? new Date(order.shippedDate).toLocaleDateString() : "Not shipped";

    const tr = document.createElement('tr');
    tr.innerHTML = `
                                <td>${actualIndex}</td>
                                <td title="${escapeHtml(productNames)}">${escapeHtml(productNames.length > 50 ? productNames.substring(0, 50) + '...' : productNames)}</td>
                                <td>${escapeHtml(order.dropshipperName || "Unknown")}</td>
                                <td>${escapeHtml(order.customerName || "Unknown")}</td>
                                <td>${escapeHtml(order.customerAddress || "Unknown")}</td>
                                <td>$${order.orderPrice?.toFixed(2) || "0.00"}</td>
                                <td>$${order.orderDiscount?.toFixed(2) || "0.00"}</td>
                                <td>
                                    <span class="status-badge ${getStatusClass(order.orderStatus)}">
                                        ${getStatusText(order.orderStatus)}
                                    </span>
                                    <button class="btn-status-edit" data-id="${order.id}" title="Edit Status">
                                        <i class="fas fa-edit"></i>
                                    </button>
                                </td>
                                <td>
                                    ${shippedDate}
                                    ${order.shippedDate ? `
                                        <button class="btn-status-edit" data-id="${order.id}" data-field="shippedDate" title="Edit Shipped Date">
                                            <i class="fas fa-edit"></i>
                                        </button>
                                    ` : `
                                        <button class="btn-status-edit" data-id="${order.id}" data-field="shippedDate" title="Add Shipped Date">
                                            <i class="fas fa-plus"></i>
                                        </button>
                                    `}
                                </td>
                                <td style="text-align:right">
                                    <button class="btn-icon viewBtn" data-id="${order.id}" title="View">
                                        <i class="fas fa-eye"></i>
                                    </button>
                                    <button class="btn-icon delBtn" data-id="${order.id}" title="Delete">
                                        <i class="fas fa-trash"></i>
                                    </button>
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
    // View buttons
    tbody.querySelectorAll(".viewBtn").forEach(btn =>
        btn.addEventListener('click', () => openViewModal(btn.dataset.id))
    );

    // Delete buttons
    tbody.querySelectorAll(".delBtn").forEach(btn =>
        btn.addEventListener('click', () => openDeleteModal(btn.dataset.id))
    );

    // Status edit buttons
    tbody.querySelectorAll(".btn-status-edit").forEach(btn =>
        btn.addEventListener('click', (e) => {
            const orderId = btn.dataset.id;
            const field = btn.dataset.field;

            if (field === 'shippedDate') {
                openEditModal(orderId, 'shippedDate');
            } else {
                openEditModal(orderId, 'status');
            }
        })
    );
}

// ----------------- Fetch Orders -----------------
async function fetchOrders(page = 1) {
    try {
        showLoading();
        const res = await fetch(`${API_ORDERS}?pageIndex=${page}&pageSize=${pageSize}`, {
            headers: authHeaders()
        });
        if (!res.ok) throw new Error("Failed to fetch orders");

        const data = await res.json();
        allOrders = data.result || [];

        // Apply search filter if any
        if (currentSearch) {
            filteredOrders = allOrders.filter(order =>
                (order.dropshipperName && order.dropshipperName.toLowerCase().includes(currentSearch)) ||
                (order.customerName && order.customerName.toLowerCase().includes(currentSearch)) ||
                (order.customerAddress && order.customerAddress.toLowerCase().includes(currentSearch)) ||
                (order.items && order.items.some(item =>
                    item.productName && item.productName.toLowerCase().includes(currentSearch)
                ))
            );
        } else {
            filteredOrders = [...allOrders];
        }

        renderTable(filteredOrders);

        const totalCount = currentSearch ? filteredOrders.length : (data.totalCount || 0);
        renderPagination(page, totalCount, pageSize);
    } catch (err) {
        console.error(err);
        showNotification('Failed to load orders.', 'error');
        tbody.innerHTML = `<tr><td colspan="10" style="text-align:center;color:red;padding:20px;">Failed to load orders.</td></tr>`;
    } finally {
        hideLoading();
    }
}

// ----------------- Render Pagination -----------------
// ====================== PAGINATION ======================
function renderPagination(current, totalPages) {
    paginationContainer.innerHTML = "";

    if (totalPages <= 1) return;

    const createBtn = (label, page, disabled = false, active = false) => {
        const btn = document.createElement("button");
        btn.textContent = label;
        btn.disabled = disabled;
        btn.className = `page-btn ${active ? "active" : ""}`;
        btn.onclick = () => {
            currentPage = page;
            fetchOrders(page);
        };
        return btn;
    };

    // Prev
    paginationContainer.appendChild(
        createBtn("«", current - 1, current === 1)
    );

    // Pages
    for (let i = 1; i <= totalPages; i++) {
        paginationContainer.appendChild(
            createBtn(i, i, false, i === current)
        );
    }

    // Next
    paginationContainer.appendChild(
        createBtn("»", current + 1, current === totalPages)
    );
}


// ----------------- View Modal -----------------
function openViewModal(id) {
    const order = allOrders.find(o => o.id === id);
    if (!order) return;

    const productNames = order.items?.map(i => i.productName).join(", ") || "No products";
    const shippedDate = order.shippedDate ? new Date(order.shippedDate).toLocaleDateString() : "Not shipped";

    viewProducts.textContent = productNames;
    viewDropshipper.textContent = order.dropshipperName || "Unknown";
    viewCustomer.textContent = order.customerName || "Unknown";
    viewAddress.textContent = order.customerAddress || "Unknown";
    viewPrice.textContent = order.orderPrice?.toFixed(2) || "0.00";
    viewDiscount.textContent = order.orderDiscount?.toFixed(2) || "0.00";
    viewStatus.textContent = getStatusText(order.orderStatus);
    viewStatus.className = `status-badge ${getStatusClass(order.orderStatus)}`;
    viewShipped.textContent = shippedDate;

    overlayView.style.display = 'flex';
    setTimeout(() => overlayView.classList.add('show'), 10);
}

// ----------------- Edit Modal -----------------
function openEditModal(id, field = 'status') {
    const order = allOrders.find(o => o.id === id);
    if (!order) return;

    editId = id;

    // Set current values
    editStatus.value = order.orderStatus || 0;

    if (order.shippedDate) {
        const date = new Date(order.shippedDate);
        editShippedDate.value = date.toISOString().split('T')[0];
    } else {
        editShippedDate.value = '';
    }

    overlayEdit.style.display = 'flex';
    setTimeout(() => overlayEdit.classList.add('show'), 10);
}

// ----------------- Save Edit -----------------
saveEditBtn.addEventListener('click', async () => {
    if (!editId) return;

    try {
        showLoading();
        const order = allOrders.find(o => o.id === editId);
        if (!order) throw new Error('Order not found');

        const updatedData = {
            orderStatus: parseInt(editStatus.value),
            shippedDate: editShippedDate.value || null
        };

        const res = await fetch(`${API_ORDERS}/${editId}`, {
            headers: authHeaders(),
            method: 'PUT',
            body: JSON.stringify(updatedData)
        });

        if (!res.ok) {
            const errorText = await res.text();
            throw new Error(`HTTP ${res.status}: ${errorText}`);
        }

        overlayEdit.classList.remove('show');
        setTimeout(() => {
            overlayEdit.style.display = 'none';
            editId = null;
        }, 300);

        showNotification('Order updated successfully!');
        fetchOrders(currentPage);
    } catch (err) {
        console.error(err);
        showNotification('Failed to update order.', 'error');
    } finally {
        hideLoading();
    }
});

// ----------------- Delete Modal -----------------
function openDeleteModal(id) {
    const order = allOrders.find(o => o.id === id);
    if (!order) return;

    deleteId = id;
    deleteMsg.textContent = `Are you sure you want to delete order from ${order.customerName || 'customer'}?`;

    overlayDelete.style.display = 'flex';
    setTimeout(() => overlayDelete.classList.add('show'), 10);
}

confirmDeleteBtn.addEventListener('click', async () => {
    if (!deleteId) return;

    try {
        showLoading();
        const res = await fetch(`${API_ORDERS}/${deleteId}`, {
            method: 'DELETE',
            headers: authHeaders()
        });
        if (!res.ok) throw new Error("Failed to delete order");

        overlayDelete.classList.remove('show');
        setTimeout(() => {
            overlayDelete.style.display = 'none';
            deleteId = null;
        }, 300);

        showNotification('Order deleted successfully!');
        fetchOrders(currentPage);
    } catch (err) {
        console.error(err);
        showNotification('Failed to delete order.', 'error');
    } finally {
        hideLoading();
    }
});

// ----------------- Close Modals -----------------
closeViewBtn.addEventListener('click', () => {
    overlayView.classList.remove('show');
    setTimeout(() => {
        overlayView.style.display = 'none';
    }, 300);
});

cancelEditBtn.addEventListener('click', () => {
    overlayEdit.classList.remove('show');
    setTimeout(() => {
        overlayEdit.style.display = 'none';
        editId = null;
    }, 300);
});

cancelDeleteBtn.addEventListener('click', () => {
    overlayDelete.classList.remove('show');
    setTimeout(() => {
        overlayDelete.style.display = 'none';
        deleteId = null;
    }, 300);
});

// Click outside modal closes it
[overlayView, overlayEdit, overlayDelete].forEach(ov => {
    ov.addEventListener('click', e => {
        if (e.target === ov) {
            ov.classList.remove('show');
            setTimeout(() => {
                ov.style.display = 'none';
            }, 300);
        }
    });
});

// ----------------- Search -----------------
function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

searchInput.addEventListener('input', debounce(() => {
    currentSearch = searchInput.value.trim().toLowerCase();
    currentPage = 1;

    if (currentSearch) {
        filteredOrders = allOrders.filter(order =>
            (order.dropshipperName && order.dropshipperName.toLowerCase().includes(currentSearch)) ||
            (order.customerName && order.customerName.toLowerCase().includes(currentSearch)) ||
            (order.customerAddress && order.customerAddress.toLowerCase().includes(currentSearch)) ||
            (order.items && order.items.some(item =>
                item.productName && item.productName.toLowerCase().includes(currentSearch)
            ))
        );
        renderTable(filteredOrders);
        renderPagination(currentPage, filteredOrders.length, pageSize);
    } else {
        filteredOrders = [...allOrders];
        renderTable(filteredOrders);
        renderPagination(currentPage, allOrders.length, pageSize);
    }
}, 300));

// ----------------- Initial Fetch -----------------
fetchOrders(currentPage);

// Optional: toggle sidebar on key press
document.addEventListener('keydown', (e) => {
    if (e.key.toLowerCase() === 'm') document.getElementById('sidebar').classList.toggle('collapsed');
});