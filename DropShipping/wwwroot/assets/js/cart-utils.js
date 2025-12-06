// 1. Helper to safely get the array from LocalStorage
loadCartItems() {
    try {
        const stored = localStorage.getItem('cartItems');
        return stored ? JSON.parse(stored) : [];
    } catch (error) {
        console.error('Failed to load cart items', error);
        return [];
    }
}

// 2. The Core Counter Logic
updateCartCount() {
    // Select the DOM element (make sure your HTML has id="cartCount")
    const cartCountElement = document.getElementById('cartCount');
    if (!cartCountElement) return;

    // Get current items
    const cartItems = this.loadCartItems();

    // Calculate total quantity (sums up the 'quantity' property of each item)
    // If you have 2 of "Item A", this counts as 2, not 1.
    const count = cartItems.reduce((total, item) => total + item.quantity, 0);

    // Update the UI
    cartCountElement.textContent = count;
}