// ==========================================================================
// Stammzellen-Projekt: Globale JavaScript-Steuerung (site.js)
// ==========================================================================

document.addEventListener("DOMContentLoaded", function () {

    // --- 1. HELL / DUNKEL SCHALTER LOGIK ---
    const darkModeToggle = document.getElementById("js-dark-mode-toggle");
    // Prüfen, ob der Nutzer bereits vorher den Dark Mode aktiviert hatte
    const aktuellesTheme = localStorage.getItem("theme");

    if (aktuellesTheme === "dark") {
        document.documentElement.setAttribute("data-theme", "dark");
    }

    if (darkModeToggle) {
        darkModeToggle.addEventListener("click", function () {
            // Aktuellen Zustand abfragen
            let theme = document.documentElement.getAttribute("data-theme");

            if (theme === "dark") {
                // Wechsel zu Hell
                document.documentElement.removeAttribute("data-theme");
                localStorage.setItem("theme", "light");
            } else {
                // Wechsel zu Dunkel
                document.documentElement.setAttribute("data-theme", "dark");
                localStorage.setItem("theme", "dark");
            }
        });
    }

    // --- 2. EIGENES HAMBURGERMENÜ LOGIK (ABGESTIMMT AUF DEIN LAYOUT) ---
    const menuToggle = document.getElementById("js-nav-toggle"); // Matcht exakt deine Button-ID
    const navMenu = document.getElementById("js-nav-menu");     // Matcht exakt deine <ul>-ID

    if (menuToggle && navMenu) {
        menuToggle.addEventListener("click", function () {
            // Schaltet die CSS-Klasse "active" bei Klick an beiden Elementen an/aus
            navMenu.classList.toggle("active");
            menuToggle.classList.toggle("active");
        });
    }
});
