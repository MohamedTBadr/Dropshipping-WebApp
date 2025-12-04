document.addEventListener("DOMContentLoaded", () => {
    updateCartCount();
});

function updateCartCount() {
    const cartCountElement = document.getElementById("cartCount");
    if (!cartCountElement) return;

    const cart = JSON.parse(localStorage.getItem("cart")) || [];
    const totalQuantity = cart.reduce((sum, item) => sum + item.quantity, 0);

    cartCountElement.textContent = totalQuantity;
}
