// AdminDropshipper.js - Full CRUD for Dropshipper Management
document.addEventListener('DOMContentLoaded', function () {
    // API Base URL
    const API_BASE_URL = 'https://localhost:7000/api';

    // State variables
    let currentPage = 1;
    const pageSize = 10;
    let totalPages = 1;
    let currentSearch = '';
    let currentDropshipperId = null;

    // DOM Elements
    const tbody = document.getElementById('tbody');
    const paginationEl = document.getElementById('pagination');
    const searchInput = document.getElementById('searchInput');
    const addProductBtn = document.getElementById('addProductBtn');
    const modalOverlay = document.getElementById('modalOverlay');
    const modalBox = document.getElementById('modalBox');
    const modalTitle = document.getElementById('modalTitle');
    const modalForm = document.getElementById('modalForm');
    const deleteOverlay = document.getElementById('deleteOverlay');
    const cancelDeleteBtn = document.getElementById('cancelDelete');
    const confirmDeleteBtn = document.getElementById('confirmDelete');
    const cancelModalBtn = document.getElementById('cancelModal');
    const viewOverlay = document.getElementById('viewOverlay');
    const closeViewBtn = document.getElementById('closeView');

    // Form fields
    const productIdField = document.getElementById('productId');
    const userNameField = document.getElementById('userName');
    const emailField = document.getElementById('email');
    const phoneNumberField = document.getElementById('phoneNumber');
    const passwordField = document.getElementById('password');
    const streetField = document.getElementById('street');
    const cityField = document.getElementById('city');
    const countryField = document.getElementById('country');

    // Initialize
    fetchDropshippers();

    // Event Listeners
    searchInput.addEventListener('input', debounce(() => {
        currentSearch = searchInput.value.trim();
        currentPage = 1;
        fetchDropshippers();
    }, 300));

    addProductBtn.addEventListener('click', () => openAddModal());
    cancelModalBtn.addEventListener('click', () => closeModal());
    cancelDeleteBtn.addEventListener('click', () => closeDeleteModal());
    confirmDeleteBtn.addEventListener('click', () => deleteDropshipper());
    closeViewBtn.addEventListener('click', () => closeViewModal());

    modalForm.addEventListener('submit', function (e) {
        e.preventDefault();
        saveDropshipper();
    });

    modalOverlay.addEventListener('click', function (e) {
        if (e.target === modalOverlay) closeModal();
    });

    deleteOverlay.addEventListener('click', function (e) {
        if (e.target === deleteOverlay) closeDeleteModal();
    });

    viewOverlay.addEventListener('click', function (e) {
        if (e.target === viewOverlay) closeViewModal();
    });

    // Fetch all dropshippers
    async function fetchDropshippers() {
        try {
            showLoading();

            const url = `${API_BASE_URL}/DropShipper?page=${currentPage}&pageSize=${pageSize}`;
            if (currentSearch) {
                //url += `&search=${encodeURIComponent(currentSearch)}`;
            }

            const response = await fetch(url);

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const data = await response.json();
            currentPage = data.pageIndex + 1; // <-- FIX

            // FIXED: Your backend returns { result: [], totalCount: 0, pageIndex: 1 }
            const dropshippers = data.result || [];

            totalPages = Math.ceil((data.totalCount || 0) / pageSize);
            if (totalPages < 1) totalPages = 1;

            displayDropshippers(dropshippers);
            renderPagination();

        } catch (error) {
            console.error('Error fetching dropshippers:', error);
            showError('Failed to load dropshippers.');
        }
    }


    // Display dropshippers in table
    function displayDropshippers(dropshippers) {
        tbody.innerHTML = '';

        if (dropshippers.length === 0) {
            tbody.innerHTML = `
                <tr>
                    <td colspan="5" style="text-align: center; padding: 40px;">
                        <div style="color: var(--muted); font-size: 14px;">
                            <i class="fas fa-users" style="font-size: 24px; margin-bottom: 10px; display: block;"></i>
                            No dropshippers found
                        </div>
                    </td>
                </tr>
            `;
            return;
        }

        dropshippers.forEach(dropshipper => {
            const row = document.createElement('tr');
            row.innerHTML = `
                <td>
                    <div style="font-weight: 500;">${escapeHtml(dropshipper.userName || 'N/A')}</div>
                </td>
                <td>${escapeHtml(dropshipper.email || 'N/A')}</td>
                <td>${escapeHtml(dropshipper.phoneNumber || 'N/A')}</td>
                <td>
                    <div>${escapeHtml(dropshipper.address?.street || 'N/A')}</div>
                    <div style="font-size: 12px; color: var(--muted);">
                        ${escapeHtml(dropshipper.address?.city || '')} 
                        ${dropshipper.address?.city && dropshipper.address?.country ? ', ' : ''}
                        ${escapeHtml(dropshipper.address?.country || '')}
                    </div>
                </td>
                <td style="text-align:right">
                    <button class="btn-icon view-btn" data-id="${dropshipper.id}" title="View">
                     👁️
                    </button>
                    <button class="btn-icon edit-btn" data-id="${dropshipper.id}" title="Edit">
                        ✏️
                    </button>
                    <button class="btn-icon delete-btn" data-id="${dropshipper.id}" title="Delete">
🗑️
                    </button>

             

                </td>
            `;

            tbody.appendChild(row);
        });

        // Add event listeners to action buttons
        document.querySelectorAll('.view-btn').forEach(btn => {
            btn.addEventListener('click', () => viewDropshipper(btn.dataset.id));
        });

        document.querySelectorAll('.edit-btn').forEach(btn => {
            btn.addEventListener('click', () => editDropshipper(btn.dataset.id));
        });

        document.querySelectorAll('.delete-btn').forEach(btn => {
            btn.addEventListener('click', () => openDeleteModal(btn.dataset.id));
        });
    }

    // Render pagination controls
    function renderPagination() {
        paginationEl.innerHTML = '';

        if (totalPages <= 1) return;

        // Previous button
        const prevBtn = document.createElement('button');
        prevBtn.className = 'btn-pagination';
        prevBtn.innerHTML = '<i class="fas fa-chevron-left"></i>';
        prevBtn.disabled = currentPage === 1;
        prevBtn.addEventListener('click', () => {
            if (currentPage > 1) {
                currentPage--;
                fetchDropshippers();
            }
        });
        paginationEl.appendChild(prevBtn);

        // Page numbers
        const maxVisible = 5;
        let startPage = Math.max(1, currentPage - Math.floor(maxVisible / 2));
        let endPage = Math.min(totalPages, startPage + maxVisible - 1);

        if (endPage - startPage + 1 < maxVisible) {
            startPage = Math.max(1, endPage - maxVisible + 1);
        }

        if (startPage > 1) {
            const firstBtn = document.createElement('button');
            firstBtn.className = 'btn-pagination';
            firstBtn.textContent = '1';
            firstBtn.addEventListener('click', () => {
                currentPage = 1;
                fetchDropshippers();
            });
            paginationEl.appendChild(firstBtn);

            if (startPage > 2) {
                const ellipsis = document.createElement('span');
                ellipsis.className = 'pagination-ellipsis';
                ellipsis.textContent = '...';
                paginationEl.appendChild(ellipsis);
            }
        }

        for (let i = startPage; i <= endPage; i++) {
            const pageBtn = document.createElement('button');
            pageBtn.className = `btn-pagination ${i === currentPage ? 'active' : ''}`;
            pageBtn.textContent = i;
            pageBtn.addEventListener('click', () => {
                currentPage = i;
                fetchDropshippers();
            });
            paginationEl.appendChild(pageBtn);
        }

        if (endPage < totalPages) {
            if (endPage < totalPages - 1) {
                const ellipsis = document.createElement('span');
                ellipsis.className = 'pagination-ellipsis';
                ellipsis.textContent = '...';
                paginationEl.appendChild(ellipsis);
            }

            const lastBtn = document.createElement('button');
            lastBtn.className = 'btn-pagination';
            lastBtn.textContent = totalPages;
            lastBtn.addEventListener('click', () => {
                currentPage = totalPages;
                fetchDropshippers();
            });
            paginationEl.appendChild(lastBtn);
        }

        // Next button
        const nextBtn = document.createElement('button');
        nextBtn.className = 'btn-pagination';
        nextBtn.innerHTML = '<i class="fas fa-chevron-right"></i>';
        nextBtn.disabled = currentPage === totalPages;
        nextBtn.addEventListener('click', () => {
            if (currentPage < totalPages) {
                currentPage++;
                fetchDropshippers();
            }
        });
        paginationEl.appendChild(nextBtn);
    }

    // Open add modal
    function openAddModal() {
        modalTitle.textContent = 'Add New Dropshipper';
        modalForm.reset();
        productIdField.value = '';

        // Show password field for new entries
        passwordField.closest('div').style.display = 'block';
        passwordField.required = true;

        modalOverlay.style.display = 'flex';
        setTimeout(() => modalOverlay.classList.add('show'), 10);
    }

    // Open edit modal
    async function editDropshipper(id) {
        try {
            showLoading();
            const response = await fetch(`${API_BASE_URL}/DropShipper/${id}`);

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const dropshipper = await response.json();

            modalTitle.textContent = 'Edit Dropshipper';
            productIdField.value = dropshipper.id;
            userNameField.value = dropshipper.userName || '';
            emailField.value = dropshipper.email || '';
            phoneNumberField.value = dropshipper.phoneNumber || '';

            // Hide password field for edits (optional to change)
            passwordField.closest('div').style.display = 'none';
            passwordField.required = false;
            passwordField.value = '';

            // Address fields
            streetField.value = dropshipper.address?.street || '';
            cityField.value = dropshipper.address?.city || '';
            countryField.value = dropshipper.address?.country || '';

            modalOverlay.style.display = 'flex';
            setTimeout(() => modalOverlay.classList.add('show'), 10);

        } catch (error) {
            console.error('Error fetching dropshipper:', error);
            showError('Failed to load dropshipper details.');
        }
    }

    // View dropshipper details
    async function viewDropshipper(id) {
        try {
            showLoading();
            const response = await fetch(`${API_BASE_URL}/DropShipper/${id}`);

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const dropshipper = await response.json();

            // Update view modal content
            document.getElementById('viewTitle').textContent = dropshipper.userName || 'Dropshipper Details';

            // Hide image section for dropshipper view
            document.querySelector('.image-section').style.display = 'none';

            // Update details section
            document.getElementById('viewName').textContent = `Name: ${dropshipper.userName || 'N/A'}`;
            document.getElementById('viewDescription').textContent = `Email: ${dropshipper.email || 'N/A'}`;
            document.getElementById('viewPrice').textContent = `Phone: ${dropshipper.phoneNumber || 'N/A'}`;
            document.getElementById('viewBrand').textContent = `Street: ${dropshipper.address?.street || 'N/A'}`;
            document.getElementById('viewCategory').textContent = `City: ${dropshipper.address?.city || 'N/A'}`;
            document.getElementById('viewYear').textContent = `Country: ${dropshipper.address?.country || 'N/A'}`;

            viewOverlay.style.display = 'flex';
            setTimeout(() => viewOverlay.classList.add('show'), 10);

        } catch (error) {
            console.error('Error fetching dropshipper:', error);
            showError('Failed to load dropshipper details.');
        }
    }

    // Save dropshipper (add or update)
    async function saveDropshipper() {
        const id = productIdField.value;
        const isEdit = !!id;

        const dropshipperData = {
            userName: userNameField.value.trim(),
            email: emailField.value.trim(),
            phoneNumber: phoneNumberField.value.trim(),
            address: {
                street: streetField.value.trim(),
                city: cityField.value.trim(),
                country: countryField.value.trim()
            }
        };

        // Only include password for new dropshippers or if changed
        if (!isEdit || passwordField.value) {
            dropshipperData.password = passwordField.value;
        }

        try {
            showLoading();
            const url = isEdit ? `${API_BASE_URL}/DropShipper/${id}` : `${API_BASE_URL}/DropShipper`;
            const method = isEdit ? 'PUT' : 'POST';

            const response = await fetch(url, {
                method: method,
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify(dropshipperData)
            });

            if (!response.ok) {
                const errorData = await response.json().catch(() => ({}));
                throw new Error(errorData.message || `HTTP error! status: ${response.status}`);
            }

            closeModal();
            showSuccess(isEdit ? 'Dropshipper updated successfully!' : 'Dropshipper added successfully!');
            fetchDropshippers();

        } catch (error) {
            console.error('Error saving dropshipper:', error);
            showError(`Failed to save dropshipper: ${error.message}`);
        }
    }

    // Open delete confirmation modal
    function openDeleteModal(id) {
        currentDropshipperId = id;
        deleteOverlay.style.display = 'flex';
        setTimeout(() => deleteOverlay.classList.add('show'), 10);
    }

    // Close delete modal
    function closeDeleteModal() {
        deleteOverlay.classList.remove('show');
        setTimeout(() => {
            deleteOverlay.style.display = 'none';
            currentDropshipperId = null;
        }, 300);
    }

    // Delete dropshipper
    async function deleteDropshipper() {
        if (!currentDropshipperId) return;

        try {
            showLoading();
            const response = await fetch(`${API_BASE_URL}/DropShipper/${currentDropshipperId}`, {
                method: 'DELETE'
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            closeDeleteModal();
            showSuccess('Dropshipper deleted successfully!');
            fetchDropshippers();

        } catch (error) {
            console.error('Error deleting dropshipper:', error);
            showError('Failed to delete dropshipper. Please try again.');
        }
    }

    // Close modal
    function closeModal() {
        modalOverlay.classList.remove('show');
        setTimeout(() => {
            modalOverlay.style.display = 'none';
            modalForm.reset();
        }, 300);
    }

    // Close view modal
    function closeViewModal() {
        viewOverlay.classList.remove('show');
        setTimeout(() => {
            viewOverlay.style.display = 'none';
        }, 300);
    }

    // Utility functions
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

    function escapeHtml(text) {
        const div = document.createElement('div');
        div.textContent = text;
        return div.innerHTML;
    }

    function showLoading() {
        // You can add a loading spinner here if needed
        console.log('Loading...');
    }

    function showSuccess(message) {
        // Create and show success notification
        const notification = document.createElement('div');
        notification.className = 'notification success';
        notification.innerHTML = `
            <i class="fas fa-check-circle"></i>
            <span>${message}</span>
        `;

        document.body.appendChild(notification);

        setTimeout(() => {
            notification.classList.add('show');
        }, 10);

        setTimeout(() => {
            notification.classList.remove('show');
            setTimeout(() => notification.remove(), 300);
        }, 3000);
    }

    function showError(message) {
        // Create and show error notification
        const notification = document.createElement('div');
        notification.className = 'notification error';
        notification.innerHTML = `
            <i class="fas fa-exclamation-circle"></i>
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

    // Add notification styles dynamically
    const style = document.createElement('style');
    style.textContent = `
        .notification {
            position: fixed;
            top: 20px;
            right: 20px;
            padding: 12px 16px;
            border-radius: 4px;
            color: white;
            display: flex;
            align-items: center;
            gap: 8px;
            z-index: 10000;
            opacity: 0;
            transform: translateX(100%);
            transition: opacity 0.3s, transform 0.3s;
        }
        
        .notification.show {
            opacity: 1;
            transform: translateX(0);
        }
        
        .notification.success {
            background-color: #4CAF50;
        }
        
        .notification.error {
            background-color: #f44336;
        }
        
        .btn-icon {
            background: none;
            border: 1px solid var(--border);
            border-radius: 4px;
            width: 32px;
            height: 32px;
            display: inline-flex;
            align-items: center;
            justify-content: center;
            cursor: pointer;
            color: var(--text);
            margin-left: 4px;
            transition: all 0.2s;
        }
        
        .btn-icon:hover {
            background-color: var(--hover-bg);
            border-color: var(--primary);
        }
        
        .btn-pagination {
            padding: 6px 12px;
            border: 1px solid var(--border);
            background: white;
            border-radius: 4px;
            cursor: pointer;
            color: var(--text);
            transition: all 0.2s;
        }
        
        .btn-pagination:hover:not(:disabled) {
            background-color: var(--hover-bg);
            border-color: var(--primary);
        }
        
        .btn-pagination.active {
            background-color: var(--primary);
            color: white;
            border-color: var(--primary);
        }
        
        .btn-pagination:disabled {
            opacity: 0.5;
            cursor: not-allowed;
        }
        
        .pagination-ellipsis {
            padding: 6px;
            color: var(--muted);
        }
        
        .overlay {
            position: fixed;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background-color: rgba(0, 0, 0, 0.5);
            display: none;
            justify-content: center;
            align-items: center;
            z-index: 1000;
            opacity: 0;
            transition: opacity 0.3s;
        }
        
        .overlay.show {
            opacity: 1;
        }
        
        .modal {
            background: white;
            border-radius: 8px;
            padding: 24px;
            max-width: 90%;
            max-height: 90vh;
            overflow-y: auto;
            transform: translateY(-20px);
            transition: transform 0.3s;
        }
        
        .overlay.show .modal {
            transform: translateY(0);
        }
        
        .view-modal .view-content {
            display: flex;
            gap: 30px;
            margin: 20px 0;
        }
        
        .view-modal .image-section {
            flex: 0 0 300px;
        }
        
        .view-modal .details-section {
            flex: 1;
        }
        
        .view-modal .product-name {
            font-size: 24px;
            font-weight: 600;
            margin-bottom: 15px;
        }
        
        .view-modal .product-description {
            margin-bottom: 15px;
            line-height: 1.6;
        }
        
        .view-modal .product-price {
            font-size: 20px;
            color: var(--primary);
            font-weight: 600;
            margin-bottom: 10px;
        }
        
        .view-modal .product-brand,
        .view-modal .product-category,
        .view-modal .product-year {
            margin-bottom: 8px;
            font-size: 14px;
        }
    `;
    document.head.appendChild(style);
});