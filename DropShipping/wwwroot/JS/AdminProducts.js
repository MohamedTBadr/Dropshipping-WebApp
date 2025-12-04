
const API_BASE = 'https://localhost:7000/api/Products';
const CAT_API = 'https://localhost:7000/api/Category';
const BRAND_API = 'https://localhost:7000/api/Brands';
const PAGE_SIZE = 10;

let currentPage = 1;
let totalPages = 1;
let selectedCategoryId = '';
let selectedBrandId = '';
let selectedSort = 'default';
let categories = [];  // Global for mapping names to IDs
let brands = [];      // Global for mapping names to IDs

const tbody = document.getElementById('tbody');
const paginationEl = document.getElementById('pagination');
const searchInput = document.getElementById('searchInput');

function qS(sel) { return document.querySelector(sel); }
function qSA(sel) { return Array.from(document.querySelectorAll(sel)); }


// --- Get JWT token from localStorage ---
function authHeaders() {
    const token = localStorage.getItem("token");
    return {
        "Authorization": `Bearer ${token}`
    };
}




// Build API URL (inline parameters style)
function buildProductsUrl(searchTerm = '') {
    const cat = selectedCategoryId || '';
    const brand = selectedBrandId || '';
    const term = searchTerm || '';
    const url = `${API_BASE}?CategoryId=${encodeURIComponent(cat)}&BrandId=${encodeURIComponent(brand)}&SearchTerm=${encodeURIComponent(term)}&PageIndex=${encodeURIComponent(currentPage)}&PageSize=${PAGE_SIZE}`;
    return url;
}



// Escape HTML for safe rendering
function escapeHtml(str) {
    if (!str) return '';
    return String(str).replace(/[&<>"'`=\/]/g, s => ({
        '&': '&amp;', '<': '&lt;', '>': '&gt;', '"': '&quot;', "'": '&#39;', '/': '&#x2F;', '`': '&#x60;', '=': '&#x3D;'
    }[s]));
}

// Render a single product row (Image | Name | Category | Brand | Price | Actions)
function renderRow(p) {
    const tr = document.createElement('tr');
    const img = (Array.isArray(p.images) && p.images.length) ? p.images[0] : '';
    const priceText = (typeof p.price === 'number' || !isNaN(Number(p.price))) ? `$${p.price}` : (p.price || '');
    tr.innerHTML = `
    <td><img src="${escapeHtml(img)}" alt="${escapeHtml(p.name)}" style="width:60px;height:60px;border-radius:8px;object-fit:cover;border:1px solid #eee"></td>
    <td>${escapeHtml(p.name)}</td>
    <td>${escapeHtml(p.categoryName || '')}</td>
    <td>${escapeHtml(p.brandName || '')}</td>
    <td>${priceText}</td>
    <td style="text-align:right">
      <div class="actions">
        <button class="btn viewBtn" data-id="${escapeHtml(p.id)}" title="View">👁️</button>
        <button class="btn editBtn" data-id="${escapeHtml(p.id)}" title="Edit">✏️</button>
        <button class="btn delBtn" data-id="${escapeHtml(p.id)}" title="Delete">🗑️</button>
      </div>
    </td>
  `;
    return tr;
}

// Render full table
function renderTable(products, totalCount = 0) {
    tbody.innerHTML = '';
    if (!products || products.length === 0) {
        tbody.innerHTML = `<tr><td colspan="6" style="text-align:center;padding:20px;color:gray;">No products found.</td></tr>`;
        renderPagination(0);
        return;
    }

    products.forEach(p => tbody.appendChild(renderRow(p)));

    // compute total pages
    totalPages = Math.max(1, Math.ceil(totalCount / PAGE_SIZE));
    renderPagination(totalPages);
    attachRowButtons();
}



/* ---------- MODALS LOGIC ---------- */
const overlayAddEdit = qS('#modalOverlay');
const overlayView = qS('#viewOverlay');
const overlayDelete = qS('#deleteOverlay');

const modalTitle = qS('#modalTitle');
const modalForm = qS('#modalForm');
const productIdInput = qS('#productId');
const pName = qS('#pName');
const pYear = qS('#pYear');
const pPrice = qS('#pPrice');
const pImg = qS('#pImg');
const PDescription = qS("#pDescription");
const pCategory = qS('#pCategory');  // New: Category select
const pBrand = qS('#pBrand');        // New: Brand select

const viewTitle = qS('#viewTitle');
const viewImg = qS('#viewImg');
const viewName = qS('#viewName');
const viewYear = qS('#viewYear');
const viewPrice = qS('#viewPrice');
const viewDescription = qS("#viewDescription");
const viewCategory = qS("#viewCategory");
const viewBrand = qS("#viewBrand");
const viewThumbnails = qS('#viewThumbnails');  // Added for thumbnails

// Buttons inside modals
const cancelModalBtn = qS('#cancelModal');
const saveModalBtn = qS('#saveModal');
const closeViewBtn = qS('#closeView');
const cancelDeleteBtn = qS('#cancelDelete');
const confirmDeleteBtn = qS('#confirmDelete');

let currentEditId = null;
let deleteId = null;

/* ---------- Add Product ---------- */
qS('#addProductBtn').addEventListener('click', async () => {
    modalTitle.textContent = 'Add Product';
    modalForm.reset();
    productIdInput.value = '';
    currentEditId = null;
    // Populate category and brand dropdowns
    await populateModalDropdowns();
    overlayAddEdit.style.display = 'flex';
});

/* ---------- Edit Product ---------- */
async function openEditModal(id) {
    modalTitle.textContent = 'Edit Product';
    overlayAddEdit.style.display = 'flex';
    currentEditId = id;

    // Populate category and brand dropdowns first
    await populateModalDropdowns();

    try {
        const res = await fetch(`${API_BASE}/${id}`, {// --- Get JWT token from localStorage ---
            headers: authHeaders()
            });
        if (!res.ok) throw new Error('Failed to fetch product');
        const p = await res.json();

        productIdInput.value = p.id;
        pName.value = p.name || '';
        pYear.value = p.modelYear || '';
        pPrice.value = p.price || '';
        PDescription.value = p.description || '';

        // Find and set category ID by name
        const selectedCat = categories.find(cat => cat.name === p.categoryName);
        pCategory.value = selectedCat ? selectedCat.id : '';

        // Find and set brand ID by name
        const selectedBrand = brands.find(brand => brand.name === p.brandName);
        pBrand.value = selectedBrand ? selectedBrand.id : '';

        // Do not set pImg.value (file input)
    } catch (err) {
        console.error('Error loading product:', err);
        alert('Failed to load product details.');
    }
}

// Function to populate category and brand selects in modal
async function populateModalDropdowns() {
    // Categories
    try {
        const catRes = await fetch(CAT_API, {// --- Get JWT token from localStorage ---
            headers: authHeaders()
        });
        if (catRes.ok) {
            categories = await catRes.json();  // Store globally
            pCategory.innerHTML = '<option value="">Select Category</option>';
            categories.forEach(cat => {
                const option = document.createElement('option');
                option.value = cat.id;
                option.textContent = cat.name;
                pCategory.appendChild(option);
            });
        }
    } catch (err) {
        console.error('Failed to load categories:', err);
        pCategory.innerHTML = '<option value="">Select Category</option>';
        categories = [];  // Reset on error
    }

    // Brands
    try {
        const brandRes = await fetch(BRAND_API, {// --- Get JWT token from localStorage ---
            headers: authHeaders()
        });
        if (brandRes.ok) {
            brands = await brandRes.json();  // Store globally
            pBrand.innerHTML = '<option value="">Select Brand</option>';
            brands.forEach(brand => {
                const option = document.createElement('option');
                option.value = brand.id;
                option.textContent = brand.name;
                pBrand.appendChild(option);
            });
        }
    } catch (err) {
        console.error('Failed to load brands:', err);
        pBrand.innerHTML = '<option value="">Select Brand</option>';
        brands = [];  // Reset on error
    }
}

/* ---------- Save (Add / Edit) ---------- */
modalForm.addEventListener('submit', async e => {
    e.preventDefault();

    const data = {
        id: productIdInput.value,
        name: pName.value,
        description: PDescription.value,
        modelYear: Number(pYear.value),
        price: Number(pPrice.value),
        categoryId: pCategory.value,
        brandId: pBrand.value
    };

    try {
        let url = API_BASE;
        let formData = new FormData();

        if (currentEditId) {
            // For update (PUT): send id, name, description, modelYear, price, categoryId, brandId in query params; images in FormData body
            formData.append('id', data.id);
            formData.append('name', data.name);
            formData.append('description', data.description);
            formData.append('modelYear', data.modelYear.toString());
            formData.append('price', data.price.toString());
            formData.append('categoryId', data.categoryId.toString());
            formData.append('brandId', data.brandId.toString());
            if (pImg.files.length > 0) {
                // Loop through each file and append individually
                for (let i = 0; i < pImg.files.length; i++) {
                    formData.append('productImages', pImg.files[i]);
                }
            }
        } else {
            // For create (POST): all in FormData body
            formData.append('name', data.name);
            formData.append('description', data.description);
            formData.append('modelYear', data.modelYear.toString());
            formData.append('price', data.price.toString());
            formData.append('categoryId', data.categoryId.toString());
            formData.append('brandId', data.brandId.toString());
            if (pImg.files.length > 0) {
                // Loop through each file and append individually
                for (let i = 0; i < pImg.files.length; i++) {
                    formData.append('productImages', pImg.files[i]);
                }
            }
        }

        const res = await fetch(url, {
            headers: authHeaders(),
            method: currentEditId ? 'PUT' : 'POST',
        
            body: formData
       
          
        });
      

        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        overlayAddEdit.style.display = 'none';
        fetchAndRender();
    } catch (err) {
        console.error(err);
        document.getElementById("error-message").textContent = "Please enter all fields correctly.";
        // Optionally, style it: document.getElementById("error-message").style.color = "red";

    }
}
);

/* ---------- View Product ---------- */
async function openViewModal(id) {
    try {
        const res = await fetch(`${API_BASE}/${id}`, {
            headers: authHeaders()
        });
        if (!res.ok) throw new Error('Failed to fetch product');
        const p = await res.json();

        viewTitle.textContent = 'Product Details';
        viewName.textContent = p.name || '';
        viewYear.textContent = 'Model Year: ' + (p.modelYear || '-');
        viewDescription.textContent = 'Description: ' + (p.description || '');
        viewPrice.textContent = 'Price: $' + (p.price || 0);
        viewBrand.textContent = 'Brand: ' + (p.brandName || '');
        viewCategory.textContent = 'Category: ' + (p.categoryName || '');

        // Clear previous thumbnails
        viewThumbnails.innerHTML = '';

        // Set main image (first one, or a placeholder if none)
        const mainImageSrc = (p.images && p.images.length > 0) ? p.images[0] : '';
        viewImg.src = mainImageSrc;
        viewImg.alt = 'Main Product Image';

        // Add thumbnails for all images
        if (p.images && p.images.length > 0) {
            p.images.forEach((imgSrc, index) => {
                const thumb = document.createElement('img');
                thumb.src = imgSrc;
                thumb.alt = `Thumbnail ${index + 1}`;
                thumb.className = 'thumbnail';  // Use CSS class
                if (index === 0) thumb.classList.add('selected');  // Highlight first

                // Click to set as main image
                thumb.addEventListener('click', () => {
                    viewImg.src = imgSrc;
                    // Update selected state
                    document.querySelectorAll('.thumbnail').forEach(t => t.classList.remove('selected'));
                    thumb.classList.add('selected');
                });

                viewThumbnails.appendChild(thumb);
            });
        }
        overlayView.style.display = 'flex';
    } catch (err) {
        console.error(err);
        alert('Failed to view product.');
    }
}

/* ---------- Delete Product ---------- */
function openDeleteModal(id) {
    deleteId = id;
    qS('#deleteMsg').textContent = `Are you sure you want to delete this product?`;
    overlayDelete.style.display = 'flex';
}

confirmDeleteBtn.addEventListener('click', async () => {
    if (!deleteId) return;
    try {
        const res = await fetch(`${API_BASE}/${deleteId}`, { method: 'DELETE', headers: authHeaders() });
        if (!res.ok) throw new Error(`HTTP ${res.status}`);
        overlayDelete.style.display = 'none';
        fetchAndRender();
    } catch (err) {
        console.error(err);
        alert('Failed to delete product.');
    }
});

/* ---------- Close / Cancel Buttons ---------- */
[cancelModalBtn, closeViewBtn, cancelDeleteBtn].forEach(btn => {
    btn.addEventListener('click', () => {
        overlayAddEdit.style.display = 'none';
        overlayView.style.display = 'none';
        overlayDelete.style.display = 'none';
    });
});

// Click outside modal closes it
[qS('#modalOverlay'), qS('#viewOverlay'), qS('#deleteOverlay')].forEach(ov => {
    ov.addEventListener('click', e => {
        if (e.target === ov) ov.style.display = 'none';
    });
});

// Render pagination controls
function renderPagination(pages) {
    paginationEl.innerHTML = '';

    const createBtn = (label, cls, disabled = false, clickFn = null) => {
        const b = document.createElement('button');
        b.textContent = label;
        b.className = 'page-btn' + (cls ? ' ' + cls : '');
        if (disabled) { b.disabled = true; b.style.opacity = 0.6; }
        if (clickFn) b.addEventListener('click', clickFn);
        return b;
    };

    if (pages <= 1) return;

    // Prev
    paginationEl.appendChild(createBtn('◀️', '', currentPage <= 1, () => {
        if (currentPage > 1) { currentPage--; fetchAndRender(); }
    }));

    // Page numbers
    const gap = 1;
    const left = Math.max(1, currentPage - gap);
    const right = Math.min(pages, currentPage + gap);

    const addPage = i => {
        const btn = createBtn(i, i === currentPage ? 'active' : '', false, () => {
            if (i !== currentPage) { currentPage = i; fetchAndRender(); }
        });
        paginationEl.appendChild(btn);
    };

    if (pages <= 7) {
        for (let i = 1; i <= pages; i++) addPage(i);
    } else {
        addPage(1);
        if (left > 2) paginationEl.appendChild(document.createTextNode('...'));
        for (let i = left; i <= right; i++) addPage(i);
        if (right < pages - 1) paginationEl.appendChild(document.createTextNode('...'));
        addPage(pages);
    }

    // Next
    paginationEl.appendChild(createBtn('▶️', '', currentPage >= pages, () => {
        if (currentPage < pages) { currentPage++; fetchAndRender(); }
    }));
}

// Fetch and render products
async function fetchAndRender() {
    const term = (searchInput.value || '').trim();
    const url = buildProductsUrl(term);
    console.log('Fetching:', url);

    try {
        const res = await fetch(url, { headers: authHeaders()});
        if (!res.ok) throw new Error(`HTTP ${res.status}`);

        const json = await res.json();

        // Adjusted for your backend response shape
        const products = json.result || [];
        const totalCount = json.totalCount || 0;
        currentPage = json.pageIndex || currentPage;
        renderTable(products, totalCount);
    } catch (err) {
        console.error('Fetch error', err);
        renderTable([], 0);
    }
}

/* -------- Dropdowns (categories, brands) ---------- */
async function loadCategoriesAndBrands() {
    // categories
    try {
        const resC = await fetch(CAT_API, { headers: authHeaders()});
        const cats = await resC.json();
        populateDropdown('#categoryDropdown', '#categoryMenu', cats, 'All Categories');
    } catch { populateDropdown('#categoryDropdown', '#categoryMenu', [], 'All Categories'); }

    // brands
    try {
        const resB = await fetch(BRAND_API, { headers: authHeaders()});
        const brands = await resB.json();
        populateDropdown('#brandDropdown', '#brandMenu', brands, 'All Brands');
    } catch { populateDropdown('#brandDropdown', '#brandMenu', [], 'All Brands'); }

    wireDropdownBehaviour();
}

function populateDropdown(dropSelector, menuSelector, items = [], defaultLabel = 'All') {
    const drop = document.querySelector(dropSelector);
    const menu = document.querySelector(menuSelector);
    if (!drop || !menu) return;

    drop.childNodes[0].textContent = defaultLabel;
    menu.innerHTML = '';
    const def = document.createElement('div');
    def.className = 'opt'; def.dataset.id = ''; def.textContent = defaultLabel;
    menu.appendChild(def);

    (items || []).forEach(it => {
        const d = document.createElement('div');
        d.className = 'opt'; d.dataset.id = it.id ?? '';
        d.textContent = it.name ?? it.title ?? '—';
        menu.appendChild(d);
    });
}

function wireDropdownBehaviour() {
    const drops = qSA('.dropdown');
    drops.forEach(drop => {
        const menu = drop.querySelector('.dropdown-menu');
        drop.addEventListener('click', e => {
            e.stopPropagation();
            const wasOpen = drop.classList.contains('open');
            qSA('.dropdown').forEach(d => d.classList.remove('open'));
            if (!wasOpen) drop.classList.add('open');
        });

        const opts = menu.querySelectorAll('.opt');
        opts.forEach(opt => opt.addEventListener('click', e => {
            e.stopPropagation();
            const val = opt.textContent.trim();
            const id = opt.dataset.id ?? '';
            drop.childNodes[0].textContent = val;
            drop.classList.remove('open');

            const type = drop.dataset.type;
            if (type === 'category') selectedCategoryId = id;
            else if (type === 'brand') selectedBrandId = id;
            else if (type === 'sort') selectedSort = id;

            currentPage = 1;
            fetchAndRender();
        }));
    });

    document.addEventListener('click', () => qSA('.dropdown').forEach(d => d.classList.remove('open')));
}

/* -------- Row Actions / Modals (placeholders) ---------- */
function attachRowButtons() {
    qSA('.viewBtn').forEach(btn =>
        btn.addEventListener('click', () => openViewModal(btn.dataset.id))
    );
    qSA('.editBtn').forEach(btn =>
        btn.addEventListener('click', () => openEditModal(btn.dataset.id))
    );
    qSA('.delBtn').forEach(btn =>
        btn.addEventListener('click', () => openDeleteModal(btn.dataset.id))
    );
}


/* -------- Search Input (debounced) ---------- */
let searchTimer = null;
searchInput.addEventListener('input', () => {
    clearTimeout(searchTimer);
    searchTimer = setTimeout(() => {
        currentPage = 1;
        fetchAndRender();
    }, 300);
});

/* -------- Init ---------- */
(async function init() {
    await loadCategoriesAndBrands();
    fetchAndRender();
})();