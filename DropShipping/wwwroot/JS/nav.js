// =========================================
// 2. NAVBAR UI LOGIC
// =========================================
document.addEventListener('DOMContentLoaded', function () {
    // Initialize Dynamic Categories
    new NavigationCategories();

    // Elements
    const menuToggle = document.getElementById('menuToggle');
    const mainNav = document.getElementById('mainNav');
    const searchBar = document.getElementById('searchBar');
    const allProductsDropdown = document.getElementById('allProductsDropdown');
    const logoutBtn = document.getElementById('logoutBtn');
    const notification = document.getElementById('notification');
    const cartCount = document.getElementById('cartCount');

    // Mobile menu toggle
    menuToggle.addEventListener('click', function () {
        mainNav.classList.toggle('active');
        searchBar.classList.toggle('active');
        menuToggle.innerHTML = mainNav.classList.contains('active') ?
            '<i class="fas fa-times"></i>' : '<i class="fas fa-bars"></i>';
    });

    // Mobile dropdown toggle
    if (window.innerWidth <= 768) {
        allProductsDropdown.addEventListener('click', function (e) {
            if (e.target.closest('.dropdown-menu')) return;
            e.preventDefault();
            this.classList.toggle('active');
        });
    }

    // Close menu when clicking outside
    document.addEventListener('click', function (e) {
        if (!e.target.closest('.main-header')) {
            mainNav.classList.remove('active');
            searchBar.classList.remove('active');
            menuToggle.innerHTML = '<i class="fas fa-bars"></i>';
        }
    });

    // Cart Functionality
    function updateCartCount() {
        const savedCart = localStorage.getItem('dropzoneCart');
        let cartItems = 0;
        if (savedCart) {
            cartItems = JSON.parse(savedCart).length;
        }
        cartCount.textContent = cartItems;
    }
    updateCartCount();

    // Notification Helper
    function showNotification(message) {
        document.getElementById('notificationText').textContent = message;
        notification.classList.add('show');
        setTimeout(() => {
            notification.classList.remove('show');
        }, 3000);
    }

    // Logout Logic
    logoutBtn.addEventListener('click', function () {
        if (confirm('Are you sure you want to logout?')) {
            showNotification('Logged out successfully!');
            setTimeout(() => {
                window.location.href = '../loginregistration.html';
            }, 1500);
        }
    });

    // Search Bar Logic
    const searchInput = document.getElementById('navSearchInput');
    const searchButton = document.getElementById('navSearchBtn');

    function performSearch() {
        const searchTerm = searchInput.value.trim();
        if (searchTerm) {
            window.location.href = `../search.html?q=${encodeURIComponent(searchTerm)}`;
        }
    }

    searchButton.addEventListener('click', performSearch);
    searchInput.addEventListener('keypress', function (e) {
        if (e.key === 'Enter') performSearch();
    });
});