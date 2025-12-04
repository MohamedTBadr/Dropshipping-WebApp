class CheckoutPage {
    constructor() {
        this.orderItemsList = document.getElementById('orderItemsList');
        this.subtotalValue = document.getElementById('subtotalValue');
        this.shippingValue = document.getElementById('shippingValue');
        this.totalValue = document.getElementById('totalValue');
        this.placeOrderBtn = document.getElementById('placeOrderBtn');
        this.checkoutForm = document.getElementById('checkoutForm');

        this.cartItems = this.loadCartItems();
        this.apiBaseUrl = '/api/Order'; // Update this if your API is hosted elsewhere

        this.init();
    }

    init() {
        // Check if cart is empty
        if (this.cartItems.length === 0) {
            this.showEmptyCartMessage();
            return;
        }

        this.renderOrderItems();
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

    showEmptyCartMessage() {
        const container = document.querySelector('.checkout-section .row');
        container.innerHTML = `
            <div class="col-12 text-center">
                <div class="card p-5">
                    <h3 class="mb-3">Your cart is empty</h3>
                    <p class="text-muted mb-4">Add some products to your cart before checking out.</p>
                    <a href="index.html" class="btn btn-primary">Continue Shopping</a>
                </div>
            </div>
        `;
    }

    renderOrderItems() {
        if (!this.orderItemsList) return;

        this.orderItemsList.innerHTML = this.cartItems
            .map(
                (item) => `
                <div class="order-item">
                    <img src="${item.image || 'images/product1.jpg'}" alt="${item.name}" class="order-item-image">
                    <div class="order-item-details">
                        <div class="order-item-name">${item.name}</div>
                        <div class="order-item-quantity">Qty: ${item.quantity}</div>
                    </div>
                    <div class="order-item-price">$${(item.price * item.quantity).toFixed(2)}</div>
                </div>
            `
            )
            .join('');
    }

    updateSummary() {
        const subtotal = this.cartItems.reduce((sum, item) => sum + item.price * item.quantity, 0);
        const shipping = this.cartItems.length > 0 ? 10 : 0;
        const total = subtotal + shipping;

        if (this.subtotalValue) this.subtotalValue.textContent = `$${subtotal.toFixed(2)}`;
        if (this.shippingValue) this.shippingValue.textContent = `$${shipping.toFixed(2)}`;
        if (this.totalValue) this.totalValue.textContent = `$${total.toFixed(2)}`;
    }

    attachEvents() {
        if (this.placeOrderBtn) {
            this.placeOrderBtn.addEventListener('click', () => this.handlePlaceOrder());
        }
    }

    async handlePlaceOrder() {
        // Validate form
        if (!this.checkoutForm.checkValidity()) {
            this.checkoutForm.reportValidity();
            return;
        }

        // Get form data
        const formData = new FormData(this.checkoutForm);
        const customerDetails = {
            name: formData.get('name'),
            email: formData.get('email'),
            phoneNumber: formData.get('phoneNumber'),
            address: formData.get('address')
        };

        // Prepare order items
        const orderItems = this.cartItems.map(item => ({
            productId: item.id,
            quantity: item.quantity
        }));

        // Get dropshipper ID from localStorage (you may need to adjust this based on your auth system)
        const dropshipperId = this.getDropshipperId();

        // Prepare order data according to OrderCreateDTO
        const orderData = {
            dropshipperId: dropshipperId,
            items: orderItems,
            customer: customerDetails
        };

        console.log('Order Data to be sent:', JSON.stringify(orderData, null, 2));

        // Show loading modal
        const loadingModal = new bootstrap.Modal(document.getElementById('loadingModal'));
        loadingModal.show();

        try {
            // Send order to backend
            const response = await fetch(this.apiBaseUrl, {
                method: 'POST',
                headers: getAuthHeaders({ 'Content-Type': 'application/json' }),
                body: JSON.stringify(orderData)
            });

            if (!response.ok) {
                const errorData = await response.json();
                throw new Error(errorData.message || 'Failed to create order');
            }

            const result = await response.json();
            console.log('Order created successfully:', result);

            // Clear cart
            localStorage.removeItem('cartItems');

            // Hide loading modal
            loadingModal.hide();

            // Show success message
            this.showSuccessMessage(result);

        } catch (error) {
            console.error('Error creating order:', error);
            loadingModal.hide();
            this.showErrorMessage(error.message);
        }
    }

    getDropshipperId() {
        // Try to get dropshipper ID from localStorage
        // This should be set during login/authentication
        const userId = localStorage.getItem('userId');
        if (userId) {
            return userId;
        }

        // For testing purposes, you can use a default ID
        // In production, this should come from authentication
        console.warn('No dropshipper ID found, using default. Please implement proper authentication.');
        return '00000000-0000-0000-0000-000000000000'; // Default GUID
    }

    showSuccessMessage(orderData) {
        const container = document.querySelector('.checkout-section .row');
        container.innerHTML = `
            <div class="col-12">
                <div class="card p-5 text-center">
                    <div class="mb-4">
                        <svg xmlns="http://www.w3.org/2000/svg" width="80" height="80" fill="currentColor" class="bi bi-check-circle-fill text-success" viewBox="0 0 16 16">
                            <path d="M16 8A8 8 0 1 1 0 8a8 8 0 0 1 16 0zm-3.97-3.03a.75.75 0 0 0-1.08.022L7.477 9.417 5.384 7.323a.75.75 0 0 0-1.06 1.06L6.97 11.03a.75.75 0 0 0 1.079-.02l3.992-4.99a.75.75 0 0 0-.01-1.05z"/>
                        </svg>
                    </div>
                    <h2 class="mb-3 text-success">Order Placed Successfully!</h2>
                    <p class="text-muted mb-2">Order ID: <strong>${orderData.id || 'N/A'}</strong></p>
                    <p class="text-muted mb-4">Thank you for your order. You will receive a confirmation email shortly.</p>
                    <div class="d-flex gap-3 justify-content-center">
                        <a href="index.html" class="btn btn-primary">Continue Shopping</a>
                        <a href="dashboard.html" class="btn btn-outline-secondary">View Orders</a>
                    </div>
                </div>
            </div>
        `;
    }

    showErrorMessage(errorMessage) {
        const alertDiv = document.createElement('div');
        alertDiv.className = 'alert alert-danger alert-dismissible fade show position-fixed top-0 start-50 translate-middle-x mt-3';
        alertDiv.style.zIndex = '9999';
        alertDiv.innerHTML = `
            <strong>Error!</strong> ${errorMessage}
            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
        `;
        document.body.appendChild(alertDiv);

        // Auto-dismiss after 5 seconds
        setTimeout(() => {
            alertDiv.remove();
        }, 5000);
    }
}

// Initialize checkout page when DOM is ready
document.addEventListener('DOMContentLoaded', () => {
    new CheckoutPage();
});
