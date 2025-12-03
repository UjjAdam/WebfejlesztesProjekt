// Destiny 2 Loadout Manager - JavaScript Functions

document.addEventListener('DOMContentLoaded', function() {
    initializeLoadoutBuilder();
    initializeFormValidation();
});

// Loadout Builder Functions
function initializeLoadoutBuilder() {
    const slotSelects = document.querySelectorAll('[data-slot-selector]');
    slotSelects.forEach(select => {
        select.addEventListener('change', function() {
            updateLoadoutPreview();
        });
    });
}

function updateLoadoutPreview() {
    const loadoutData = {
        primary: document.getElementById('primary-weapon')?.value,
        special: document.getElementById('special-weapon')?.value,
        heavy: document.getElementById('heavy-weapon')?.value
    };
    
    console.log('Loadout updated:', loadoutData);
}

// Form Validation
function initializeFormValidation() {
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function(e) {
            if (!form.checkValidity() === false) {
                e.preventDefault();
                e.stopPropagation();
            }
            form.classList.add('was-validated');
        });
    });
}

// Delete confirmation
function confirmDelete(name) {
    return confirm(`Are you sure you want to delete "${name}"? This action cannot be undone.`);
}

// Recommendation form handler
function submitRecommendationForm(event) {
    event.preventDefault();
    
    const formData = new FormData(event.target);
    const surge = formData.get('surgeName');
    const champions = formData.getAll('selectedChampionIds');

    if (!surge) {
        alert('Please select a surge');
        return false;
    }

    // Show loading indicator
    const resultsContainer = document.getElementById('resultsContainer');
    resultsContainer.innerHTML = '<div class="loading"></div> Loading recommendations...';

    return true;
}

// Weapon filter function
function filterWeapons(filterType) {
    const table = document.querySelector('table');
    const rows = table.querySelectorAll('tbody tr');
    
    rows.forEach(row => {
        const weaponType = row.getAttribute('data-weapon-type');
        if (filterType === 'all' || weaponType === filterType) {
            row.style.display = '';
        } else {
            row.style.display = 'none';
        }
    });
}

// Loadout summary generator
function generateLoadoutSummary(loadoutId) {
    const loadoutCard = document.querySelector(`[data-loadout-id="${loadoutId}"]`);
    if (!loadoutCard) return;

    const weapons = loadoutCard.querySelectorAll('.weapon-info');
    const summary = Array.from(weapons).map(w => w.textContent.trim()).join(', ');
    
    return summary;
}

// Element badge color mapping
function getElementColor(element) {
    const colorMap = {
        'Arc': '#0066FF',
        'Solar': '#FF6600',
        'Void': '#6600CC',
        'Kinetic': '#999999'
    };
    return colorMap[element] || '#333333';
}

// Champion effectiveness checker
function checkChampionEffectiveness(weapons, champions) {
    const effectiveness = {};
    
    champions.forEach(champion => {
        const effectiveWeapons = weapons.filter(w => {
            return isWeaponEffectiveAgainstChampion(w, champion);
        });
        effectiveness[champion] = {
            count: effectiveWeapons.length,
            weapons: effectiveWeapons
        };
    });
    
    return effectiveness;
}

function isWeaponEffectiveAgainstChampion(weapon, champion) {
    // This maps to the server-side logic
    const championEffectiveness = {
        'Anti-Barrier': ['SniperRifle', 'ScoutRifle', 'PulseRifle', 'LinearFusionRifle'],
        'Overload': ['AutoRifle', 'SubmachineGun', 'MachineGun'],
        'Unstoppable': ['FusionRifle', 'RocketLauncher', 'GrenadeGauncher']
    };
    
    return championEffectiveness[champion]?.includes(weapon.type) || false;
}

// Surge element matcher
function getSurgeMatchingWeapons(weapons, surge) {
    return weapons.filter(w => w.element === surge.element);
}

// Smooth scroll to element
function smoothScrollToElement(selector) {
    const element = document.querySelector(selector);
    if (element) {
        element.scrollIntoView({ behavior: 'smooth' });
    }
}

// Toast notification
function showNotification(message, type = 'info') {
    const notification = document.createElement('div');
    notification.className = `alert alert-${type} position-fixed`;
    notification.style.top = '20px';
    notification.style.right = '20px';
    notification.style.zIndex = '1050';
    notification.textContent = message;
    
    document.body.appendChild(notification);
    
    setTimeout(() => {
        notification.remove();
    }, 3000);
}

// Export loadout as JSON
function exportLoadout(loadoutId) {
    const loadoutData = {
        id: loadoutId,
        exportDate: new Date().toISOString(),
        // Add more data as needed
    };
    
    const jsonString = JSON.stringify(loadoutData, null, 2);
    const blob = new Blob([jsonString], { type: 'application/json' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = `loadout-${loadoutId}.json`;
    link.click();
}

// Import loadout from JSON
function importLoadout(file) {
    const reader = new FileReader();
    reader.onload = function(e) {
        try {
            const loadoutData = JSON.parse(e.target.result);
            // Process imported data
            console.log('Loadout imported:', loadoutData);
            showNotification('Loadout imported successfully!', 'success');
        } catch (error) {
            showNotification('Error importing loadout: ' + error.message, 'danger');
        }
    };
    reader.readAsText(file);
}

// Initialize tooltips and popovers (Bootstrap 5)
function initializeTooltips() {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function(tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
}

// Keyboard shortcuts
document.addEventListener('keydown', function(event) {
    // Ctrl+K: Focus search
    if (event.ctrlKey && event.key === 'k') {
        event.preventDefault();
        const searchInput = document.querySelector('input[type="search"]');
        if (searchInput) searchInput.focus();
    }
    
    // Escape: Close modals or focus management
    if (event.key === 'Escape') {
        const modals = document.querySelectorAll('.modal.show');
        modals.forEach(modal => {
            const bootstrapModal = bootstrap.Modal.getInstance(modal);
            if (bootstrapModal) bootstrapModal.hide();
        });
    }
});

// Performance: Lazy load images
if ('IntersectionObserver' in window) {
    const imageObserver = new IntersectionObserver((entries, observer) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                const img = entry.target;
                img.src = img.dataset.src;
                img.classList.remove('lazy');
                imageObserver.unobserve(img);
            }
        });
    });

    document.querySelectorAll('img.lazy').forEach(img => imageObserver.observe(img));
}
