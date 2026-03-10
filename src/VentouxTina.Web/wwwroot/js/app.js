// App initialization and utilities

window.setDarkMode = function (isDark) {
    const html = document.documentElement;
    if (isDark) {
        html.classList.add('dark');
    } else {
        html.classList.remove('dark');
    }
};

// Initialize dark mode from localStorage on page load
document.addEventListener('DOMContentLoaded', function () {
    const stored = localStorage.getItem('darkMode');
    if (stored === 'True') {
        window.setDarkMode(true);
    }
});

window.scrollToBottom = function () {
    window.scrollTo({top: document.body.scrollHeight, behavior: 'instant'});
};

