// ========================================
// DARK MODE TOGGLE
// ========================================

function initTheme() {
    const savedTheme = localStorage.getItem('theme') || 'light';
    document.documentElement.setAttribute('data-theme', savedTheme);
    updateThemeIcon(savedTheme);
}

function toggleTheme() {
    const currentTheme = document.documentElement.getAttribute('data-theme');
    const newTheme = currentTheme === 'dark' ? 'light' : 'dark';

    document.documentElement.setAttribute('data-theme', newTheme);
    localStorage.setItem('theme', newTheme);
    updateThemeIcon(newTheme);

    showToast(`Switched to ${newTheme} mode`, 'success');
}

function updateThemeIcon(theme) {
    const icon = document.querySelector('.theme-toggle-slider i');
    if (icon) {
        icon.className = theme === 'dark' ? 'fas fa-moon' : 'fas fa-sun';
    }
}

// ========================================
// SIDEBAR TOGGLE (MOBILE)
// ========================================

function toggleSidebar() {
    const sidebar = document.querySelector('.sidebar');
    if (sidebar) {
        sidebar.classList.toggle('open');
    }
}

// Close sidebar when clicking outside on mobile
document.addEventListener('click', function (event) {
    const sidebar = document.querySelector('.sidebar');
    const menuBtn = document.querySelector('.mobile-menu-btn');

    if (sidebar && menuBtn && window.innerWidth <= 768) {
        if (!sidebar.contains(event.target) && !menuBtn.contains(event.target)) {
            sidebar.classList.remove('open');
        }
    }
});

// ========================================
// TOAST NOTIFICATIONS
// ========================================

let toastContainer;

function initToastContainer() {
    if (!toastContainer) {
        toastContainer = document.createElement('div');
        toastContainer.className = 'toast-container';
        document.body.appendChild(toastContainer);
    }
}

function showToast(message, type = 'info', duration = 3000) {
    initToastContainer();

    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;

    const icon = {
        success: 'fa-check-circle',
        error: 'fa-exclamation-circle',
        warning: 'fa-exclamation-triangle',
        info: 'fa-info-circle'
    }[type] || 'fa-info-circle';

    toast.innerHTML = `
        <div class="flex items-center gap-md">
            <i class="fas ${icon}"></i>
            <span>${message}</span>
        </div>
    `;

    toastContainer.appendChild(toast);

    setTimeout(() => {
        toast.style.opacity = '0';
        toast.style.transform = 'translateX(100%)';
        setTimeout(() => toast.remove(), 300);
    }, duration);
}

// ========================================
// CONFIRMATION MODAL
// ========================================

function showConfirmModal(message, onConfirm) {
    const modal = document.createElement('div');
    modal.className = 'modal-overlay';
    modal.innerHTML = `
        <div class="modal-content card">
            <h3 class="mb-3">Confirm Action</h3>
            <p class="mb-4">${message}</p>
            <div class="flex gap-md justify-end">
                <button class="btn btn-secondary" onclick="closeModal(this)">Cancel</button>
                <button class="btn btn-danger" onclick="confirmAction(this)">Confirm</button>
            </div>
        </div>
    `;

    modal.querySelector('.btn-danger').addEventListener('click', function () {
        onConfirm();
        closeModal(this);
    });

    document.body.appendChild(modal);

    // Add modal styles if not already present
    if (!document.getElementById('modal-styles')) {
        const style = document.createElement('style');
        style.id = 'modal-styles';
        style.textContent = `
            .modal-overlay {
                position: fixed;
                top: 0;
                left: 0;
                right: 0;
                bottom: 0;
                background: rgba(0, 0, 0, 0.5);
                display: flex;
                align-items: center;
                justify-content: center;
                z-index: 10000;
                animation: fadeIn 0.3s ease-out;
            }
            .modal-content {
                max-width: 500px;
                width: 90%;
                animation: slideInRight 0.3s ease-out;
            }
        `;
        document.head.appendChild(style);
    }
}

function closeModal(btn) {
    const modal = btn.closest('.modal-overlay');
    if (modal) {
        modal.style.opacity = '0';
        setTimeout(() => modal.remove(), 300);
    }
}

// ========================================
// DELETE CONFIRMATION
// ========================================

function confirmDelete(event, itemName = 'this item') {
    event.preventDefault();
    const form = event.target;

    showConfirmModal(
        `Are you sure you want to delete ${itemName}? This action cannot be undone.`,
        () => form.submit()
    );
}

// ========================================
// FORM VALIDATION ENHANCEMENTS
// ========================================

function validateForm(formId) {
    const form = document.getElementById(formId);
    if (!form) return true;

    const inputs = form.querySelectorAll('[required]');
    let isValid = true;

    inputs.forEach(input => {
        if (!input.value.trim()) {
            isValid = false;
            input.classList.add('is-invalid');
            showFieldError(input, 'This field is required');
        } else {
            input.classList.remove('is-invalid');
            removeFieldError(input);
        }
    });

    return isValid;
}

function showFieldError(input, message) {
    removeFieldError(input);
    const error = document.createElement('div');
    error.className = 'form-error';
    error.textContent = message;
    input.parentNode.appendChild(error);
}

function removeFieldError(input) {
    const error = input.parentNode.querySelector('.form-error');
    if (error) error.remove();
}

// ========================================
// LOADING STATES
// ========================================

function showLoading(element) {
    if (typeof element === 'string') {
        element = document.querySelector(element);
    }
    if (element) {
        element.disabled = true;
        element.innerHTML = '<span class="spinner"></span> Loading...';
    }
}

function hideLoading(element, originalText) {
    if (typeof element === 'string') {
        element = document.querySelector(element);
    }
    if (element) {
        element.disabled = false;
        element.innerHTML = originalText;
    }
}

// ========================================
// NUMBER FORMATTING
// ========================================

function formatCurrency(amount) {
    return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: 'USD'
    }).format(amount);
}

function formatNumber(number) {
    return new Intl.NumberFormat('en-US').format(number);
}

// ========================================
// DATE FORMATTING
// ========================================

function formatDate(dateString) {
    const date = new Date(dateString);
    return new Intl.DateFormat('en-US', {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
    }).format(date);
}

// ========================================
// AUTOCOMPLETE
// ========================================

function initAutocomplete(inputId, suggestions) {
    const input = document.getElementById(inputId);
    if (!input) return;

    const container = document.createElement('div');
    container.className = 'autocomplete-container';
    container.style.cssText = `
        position: absolute;
        background: var(--bg-primary);
        border: 1px solid var(--border-color);
        border-radius: var(--radius-md);
        box-shadow: var(--shadow-lg);
        max-height: 200px;
        overflow-y: auto;
        z-index: 1000;
        display: none;
    `;

    input.parentNode.style.position = 'relative';
    input.parentNode.appendChild(container);

    input.addEventListener('input', function () {
        const value = this.value.toLowerCase();
        container.innerHTML = '';

        if (!value) {
            container.style.display = 'none';
            return;
        }

        const matches = suggestions.filter(s =>
            s.toLowerCase().includes(value)
        );

        if (matches.length === 0) {
            container.style.display = 'none';
            return;
        }

        matches.forEach(match => {
            const item = document.createElement('div');
            item.textContent = match;
            item.style.cssText = `
                padding: 0.5rem 1rem;
                cursor: pointer;
                transition: background-color 0.15s;
            `;
            item.addEventListener('mouseenter', function () {
                this.style.background = 'var(--bg-secondary)';
            });
            item.addEventListener('mouseleave', function () {
                this.style.background = 'transparent';
            });
            item.addEventListener('click', function () {
                input.value = match;
                container.style.display = 'none';
            });
            container.appendChild(item);
        });

        container.style.display = 'block';
    });

    document.addEventListener('click', function (e) {
        if (!input.contains(e.target) && !container.contains(e.target)) {
            container.style.display = 'none';
        }
    });
}

// ========================================
// FILE UPLOAD PREVIEW
// ========================================

function previewFile(input, previewId) {
    const file = input.files[0];
    const preview = document.getElementById(previewId);

    if (!file || !preview) return;

    if (file.type.startsWith('image/')) {
        const reader = new FileReader();
        reader.onload = function (e) {
            preview.innerHTML = `
                <img src="${e.target.result}" 
                     alt="Preview" 
                     style="max-width: 200px; max-height: 200px; border-radius: var(--radius-md);">
            `;
        };
        reader.readAsDataURL(file);
    } else {
        preview.innerHTML = `
            <div class="flex items-center gap-md">
                <i class="fas fa-file fa-2x"></i>
                <span>${file.name}</span>
            </div>
        `;
    }
}

// ========================================
// ACTIVE NAV HIGHLIGHTING
// ========================================

function highlightActiveNav() {
    const currentPath = window.location.pathname;
    const navItems = document.querySelectorAll('.nav-item');

    navItems.forEach(item => {
        const href = item.getAttribute('href');
        if (href && currentPath.includes(href)) {
            item.classList.add('active');
        } else {
            item.classList.remove('active');
        }
    });
}

// ========================================
// INITIALIZE ON PAGE LOAD
// ========================================

document.addEventListener('DOMContentLoaded', function () {
    initTheme();
    highlightActiveNav();

    // Add smooth scroll behavior
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const target = document.querySelector(this.getAttribute('href'));
            if (target) {
                target.scrollIntoView({ behavior: 'smooth' });
            }
        });
    });

    // Add fade-in animation to cards
    const cards = document.querySelectorAll('.card');
    cards.forEach((card, index) => {
        setTimeout(() => {
            card.classList.add('fade-in');
        }, index * 50);
    });
});

// ========================================
// EXPORT FUNCTIONS FOR GLOBAL USE
// ========================================

window.toggleTheme = toggleTheme;
window.toggleSidebar = toggleSidebar;
window.showToast = showToast;
window.showConfirmModal = showConfirmModal;
window.closeModal = closeModal;
window.confirmDelete = confirmDelete;
window.validateForm = validateForm;
window.showLoading = showLoading;
window.hideLoading = hideLoading;
window.formatCurrency = formatCurrency;
window.formatNumber = formatNumber;
window.formatDate = formatDate;
window.initAutocomplete = initAutocomplete;
window.previewFile = previewFile;
