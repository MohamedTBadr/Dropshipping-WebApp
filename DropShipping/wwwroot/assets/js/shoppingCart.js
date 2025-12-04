class ShoppingCartPage {
    constructor() {
        this.cartTableBody = document.getElementById('cartTableBody');
        this.subtotalValue = document.getElementById('subtotalValue');
        this.shippingValue = document.getElementById('shippingValue');
        this.totalValue = document.getElementById('totalValue');
        this.checkoutBtn = document.getElementById('checkoutBtn');
        this.headerCartSup = document.querySelector('.cart-icon sup');

        this.cartItems = this.loadCartItems();
        this.renderCart();
        this.updateSummary();
        this.attachEvents();
    }

    loadCartItems() {
        try {
            const stored = localStorage.getItem('cartItems');
            return stored ? JSON.parse(stored) : [];
        } catch (error) {
            console.error('Failed to parse cart items', error);
            return [];
        }
    }

    saveCartItems() {
        localStorage.setItem('cartItems', JSON.stringify(this.cartItems));
        this.updateSummary();
    }

    renderCart() {
        if (!this.cartTableBody) return;

        if (this.cartItems.length === 0) {
            this.cartTableBody.innerHTML = `
                <tr>
                    <td colspan="5" class="text-center py-4">Your cart is empty.</td>
                </tr>
            `;
            this.updateHeaderCartCount();
            return;
        }

        this.cartTableBody.innerHTML = this.cartItems
            .map(
                (item) => `
                <tr data-product-id="${item.id}">
                    <td>
                        <div class="d-flex align-items-center">
                            <img src="${item.image || 'images/product1.jpg'}" alt="${item.name}" width="60" class="me-3 rounded">
                            <span>${item.name}</span>
                        </div>
                    </td>
                    <td>$${item.price.toFixed(2)}</td>
                    <td>
                        <input type="number" class="form-control w-50 mx-auto quantity-input" value="${item.quantity}" min="1">
                    </td>
                    <td>$${(item.price * item.quantity).toFixed(2)}</td>
                    <td>
                        <button class="btn btn-danger btn-sm remove-item">Remove</button>
                    </td>
                </tr>
            `
            )
            .join('');

        this.updateHeaderCartCount();
    }

    attachEvents() {
        if (!this.cartTableBody) return;

        this.cartTableBody.addEventListener('change', (event) => {
            if (!event.target.classList.contains('quantity-input')) return;

            const row = event.target.closest('tr');
            const productId = row?.dataset.productId;
            const newQuantity = Math.max(1, parseInt(event.target.value, 10) || 1);
            event.target.value = newQuantity;
            this.updateItemQuantity(productId, newQuantity);
        });

        this.cartTableBody.addEventListener('click', (event) => {
            if (!event.target.classList.contains('remove-item')) return;
            const row = event.target.closest('tr');
            const productId = row?.dataset.productId;
            this.removeItem(productId);
        });

        if (this.checkoutBtn) {
            this.checkoutBtn.addEventListener('click', () => {
                if (this.cartItems.length === 0) {
                    alert('Your cart is empty. Please add items before checking out.');
                    return;
                }
                window.location.href = 'checkout.html';
            });
        }
    }

    updateItemQuantity(productId, quantity) {
        const item = this.cartItems.find((cartItem) => cartItem.id === productId);
        if (!item) return;
        item.quantity = quantity;
        this.saveCartItems();
        this.renderCart();
    }

    removeItem(productId) {
        this.cartItems = this.cartItems.filter((item) => item.id !== productId);
        this.saveCartItems();
        this.renderCart();
    }

    updateSummary() {
        const subtotal = this.cartItems.reduce((sum, item) => sum + item.price * item.quantity, 0);
        const shipping = this.cartItems.length > 0 ? 10 : 0;
        const total = subtotal + shipping;

        if (this.subtotalValue) this.subtotalValue.textContent = `$${subtotal.toFixed(2)}`;
        if (this.shippingValue) this.shippingValue.textContent = `$${shipping.toFixed(2)}`;
        if (this.totalValue) this.totalValue.textContent = `$${total.toFixed(2)}`;
    }

    updateHeaderCartCount() {
        if (!this.headerCartSup) return;
        const count = this.cartItems.reduce((sum, item) => sum + item.quantity, 0);
        this.headerCartSup.textContent = count;
    }
}

document.addEventListener('DOMContentLoaded', () => {
    new ShoppingCartPage();
});


