class CategoryPage {
    constructor() {
        this.products = [];
        this.currentPage = 1;
        this.pageSize = 9;
        this.totalCount = 0;
        this.categoryId = this.getCategoryIdFromUrl();
        this.activeBrands = new Set();
        this.cartItems = this.loadCartItems();
        this.productCardImages = new Map();

        this.productsGrid = document.getElementById('categoryProducts');
        this.pagination = document.getElementById('pagination');
        this.brandFiltersContainer = document.getElementById('brandFilters');
        this.resultsCount = document.getElementById('resultsCount');
        this.clearFiltersBtn = document.getElementById('clearFilters');
        this.cartCountElement = document.getElementById('cartCount');
        this.categoryTitle = document.getElementById('categoryTitle');

        this.init();
    }

    async init() {
        if (!this.productsGrid) return;

        await this.loadProductsFromApi();
        await this.renderBrandFilters();
        this.attachEvents();
        this.updateCartCount();

        if (this.categoryId) {
            this.loadCategoryName();
        }
    }

    getCategoryIdFromUrl() {
        const params = new URLSearchParams(window.location.search);
        return params.get('categoryId');
    }

    async loadCategoryName() {
        try {
            const response = await fetch('/api/Category');
            if (response.ok) {
                const categories = await response.json();
                const category = categories.find(c => c.id === this.categoryId);
                if (category && this.categoryTitle) {
                    this.categoryTitle.textContent = category.name;
                }
            }
        } catch (error) {
            console.error("Error loading category name:", error);
        }
    }

    async loadProductsFromApi(page = 1) {
        try {
            this.currentPage = page;
            this.showLoading();

            const params = new URLSearchParams();
            params.append('PageIndex', this.currentPage);
            params.append('PageSize', this.pageSize);

            if (this.categoryId) {
                params.append('CategoryId', this.categoryId);
            }

            if (this.activeBrands.size > 0) {
                const brandId = this.getBrandIdByName([...this.activeBrands][0]);
                if (brandId) params.append('BrandId', brandId);
            }

            const response = await fetch(`/api/Products?${params.toString()}`);

            if (!response.ok) {
                throw new Error(`Failed to load products. Status: ${response.status}`);
            }

            const data = await response.json();

            this.products = (data.result || []).map(p => ({
                id: p.id,
                name: p.name,
                price: p.price,
                brand: { name: p.brandName },
                category: { name: p.categoryName },
                categoryId: p.categoryId,
                images: p.images ? p.images.map(img => ({ image: img })) : [],
                modelYear: p.modelYear || "N/A",
                description: p.description || "No description available."
            }));

            this.totalCount = data.totalCount || 0;
            this.currentPage = data.pageIndex || page;

            this.renderProducts();
            this.renderPagination();
            this.updateResultsCount();

        } catch (error) {
            console.error("Error loading products:", error);
            this.productsGrid.innerHTML = `<p class="error-message">Failed to load products. Please try again later.</p>`;
        }
    }

    showLoading() {
        this.productsGrid.innerHTML = '<div class="loading-spinner">Loading...</div>';
    }

    async renderBrandFilters() {
        if (!this.brandFiltersContainer) return;

        try {
            let url = '/api/Brands';
            if (this.categoryId) {
                url += `?categoryId=${this.categoryId}`;
            }

            const response = await fetch(url);
            if (!response.ok) throw new Error("Failed to load brands");

            const brands = await response.json();
            this.brandsMap = new Map(brands.map(b => [b.name, b.id]));

            const sortedBrands = brands.sort((a, b) => a.name.localeCompare(b.name));

            this.brandFiltersContainer.innerHTML = sortedBrands
                .map(brand => `
                    <label class="filter-option">
                        <input type="checkbox" value="${brand.name}" data-brand-id="${brand.id}">
                        <span>${brand.name}</span>
                    </label>
                `)
                .join('');

            this.brandFiltersContainer.querySelectorAll("input").forEach(cb => {
                cb.addEventListener("change", (e) => {
                    const { value, checked } = e.target;

                    if (checked) {
                        this.activeBrands.clear();
                        this.brandFiltersContainer.querySelectorAll("input").forEach(otherCb => {
                            if (otherCb !== cb) otherCb.checked = false;
                        });
                        this.activeBrands.add(value);
                    } else {
                        this.activeBrands.delete(value);
                    }

                    this.loadProductsFromApi(1);
                });
            });

        } catch (error) {
            console.error("Brand load failed:", error);
        }
    }

    getBrandIdByName(name) {
        return this.brandsMap ? this.brandsMap.get(name) : null;
    }

    renderProducts() {
        this.productsGrid.innerHTML = "";

        if (this.products.length === 0) {
            this.productsGrid.innerHTML = `<p>No products found.</p>`;
            return;
        }

        this.products.forEach(product => {
            const card = this.createProductCard(product);
            this.productsGrid.appendChild(card);
        });
    }

    renderPagination() {
        const totalPages = Math.ceil(this.totalCount / this.pageSize);
        this.pagination.innerHTML = "";

        if (totalPages <= 1) return;

        const addBtn = (label, page, disabled = false, isActive = false) => {
            const btn = document.createElement("button");
            btn.textContent = label;
            btn.disabled = disabled;
            if (isActive) btn.classList.add("active");

            btn.addEventListener("click", () => {
                if (!disabled && page !== this.currentPage) {
                    this.loadProductsFromApi(page);
                    window.scrollTo({ top: 0, behavior: 'smooth' });
                }
            });

            this.pagination.appendChild(btn);
        };

        addBtn("‹", this.currentPage - 1, this.currentPage === 1);

        if (totalPages <= 7) {
            for (let i = 1; i <= totalPages; i++) {
                addBtn(i, i, false, i === this.currentPage);
            }
        } else {
            addBtn(1, 1, false, 1 === this.currentPage);

            if (this.currentPage > 3) this.pagination.append("...");

            const start = Math.max(2, this.currentPage - 1);
            const end = Math.min(totalPages - 1, this.currentPage + 1);

            for (let i = start; i <= end; i++) {
                addBtn(i, i, false, i === this.currentPage);
            }

            if (this.currentPage < totalPages - 2) this.pagination.append("...");

            addBtn(totalPages, totalPages, false, totalPages === this.currentPage);
        }

        addBtn("›", this.currentPage + 1, this.currentPage === totalPages);
    }

    updateResultsCount() {
        if (this.totalCount === 0) {
            this.resultsCount.textContent = "No results";
            return;
        }

        const start = (this.currentPage - 1) * this.pageSize + 1;
        const end = Math.min(start + this.pageSize - 1, this.totalCount);

        this.resultsCount.textContent = `Showing ${start}-${end} of ${this.totalCount} products`;
    }

    createProductCard(product) {
        const card = document.createElement("div");
        card.className = "product-card";
        card.dataset.productId = product.id;

        this.productCardImages.set(product.id, 0);

        const firstImg = product.images?.[0]?.image || "https://via.placeholder.com/300";
        const hasMultiple = product.images && product.images.length > 1;

        card.innerHTML = `
            <div class="product-card-image">
                <img src="${firstImg}" alt="${product.name}">
                <button class="card-action-button" data-product-id="${product.id}">
                    <i class="fas fa-shopping-cart"></i>
                </button>

                ${hasMultiple ? `
                    <button class="card-arrow card-arrow-left" data-direction="-1">
                        <i class="fas fa-chevron-left"></i>
                    </button>
                    <button class="card-arrow card-arrow-right" data-direction="1">
                        <i class="fas fa-chevron-right"></i>
                    </button>
                ` : ""}
            </div>

            <div class="product-card-info">
                <h3>${product.name}</h3>
                <div>${product.brand.name}</div>
                <div class="product-card-price">$${product.price}</div>
            </div>
        `;

        this.setupCardArrows(card, product);
        this.setupCardActionButton(card, product);

        card.addEventListener("click", (e) => {
            if (e.target.closest("button")) return;
            this.viewProduct(product.id);
        });

        return card;
    }

    setupCardArrows(card, product) {
        if (!product.images || product.images.length <= 1) return;
        const imgEl = card.querySelector("img");

        card.querySelectorAll(".card-arrow").forEach(btn => {
            btn.addEventListener("click", e => {
                e.stopPropagation();
                const direction = parseInt(btn.dataset.direction);
                this.navigateCardImage(product.id, direction, imgEl, product);
            });
        });
    }

    navigateCardImage(productId, direction, imgEl, product) {
        const total = product.images.length;
        let index = this.productCardImages.get(productId) || 0;

        index = (index + direction + total) % total;
        this.productCardImages.set(productId, index);

        imgEl.src = product.images[index].image;
    }

    setupCardActionButton(card, product) {
        const btn = card.querySelector(".card-action-button");
        btn.addEventListener("click", e => {
            e.stopPropagation();
            this.addToCart(product);
        });
    }

    addToCart(product) {
        const existing = this.cartItems.find(i => i.id === product.id);

        if (existing) existing.quantity++;
        else {
            this.cartItems.push({
                id: product.id,
                name: product.name,
                price: product.price,
                image: product.images[0]?.image,
                quantity: 1
            });
        }

        this.persistCartItems();
        this.updateCartCount();
        this.showMessage(`${product.name} added to cart!`, "success");
    }

    updateCartCount() {
        const count = this.cartItems.reduce((s, i) => s + i.quantity, 0);
        if (this.cartCountElement) {
            this.cartCountElement.textContent = count;
        }
    }

    persistCartItems() {
        localStorage.setItem("cartItems", JSON.stringify(this.cartItems));
    }

    loadCartItems() {
        return JSON.parse(localStorage.getItem("cartItems") || "[]");
    }

    viewProduct(productId) {
        const item = this.products.find(p => p.id === productId);
        if (item) {
            localStorage.setItem("selectedProduct", JSON.stringify(item));
            window.location.href = "productpage.html";
        }
    }

    showMessage(text, type) {
        const msg = document.createElement("div");
        msg.className = "toast";
        msg.textContent = text;
        if (type === 'error') msg.style.backgroundColor = '#ff4444';

        document.body.appendChild(msg);

        setTimeout(() => msg.remove(), 3000);
    }

    attachEvents() {
        if (this.clearFiltersBtn) {
            this.clearFiltersBtn.addEventListener("click", () => {
                this.activeBrands.clear();
                this.brandFiltersContainer.querySelectorAll("input").forEach(cb => cb.checked = false);
                this.loadProductsFromApi(1);
            });
        }
    }
}

document.addEventListener("DOMContentLoaded", () => new CategoryPage());
