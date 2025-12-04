// Delete Dropshipper Row
document.querySelectorAll('.delete-btn').forEach(btn => {
    btn.addEventListener('click', function () {
        if (confirm('Are you sure you want to delete this dropshipper?')) {
            const row = this.closest('tr');
            row.remove();
        }
    });
});

// Edit Dropshipper Row
document.querySelectorAll('.edit-btn').forEach(btn => {
    btn.addEventListener('click', function () {
        const row = this.closest('tr');
        const name = row.children[1].innerText;
        const email = row.children[2].innerText;
        const phone = row.children[3].innerText;
        const status = row.children[4].innerText;

        const newName = prompt('Edit Name:', name);
        const newEmail = prompt('Edit Email:', email);
        const newPhone = prompt('Edit Phone:', phone);
        const newStatus = prompt('Edit Status (Active/Inactive):', status);

        if (newName) row.children[1].innerText = newName;
        if (newEmail) row.children[2].innerText = newEmail;
        if (newPhone) row.children[3].innerText = newPhone;
        if (newStatus) row.children[4].innerHTML = `<span class="badge ${newStatus.toLowerCase() === 'active' ? 'bg-success' : 'bg-secondary'}">${newStatus}</span>`;
    });
});
