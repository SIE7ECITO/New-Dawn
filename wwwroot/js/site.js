document.addEventListener("DOMContentLoaded", function (event) {
    const showNavbar = (toggleId, navId, bodyId, headerId) => {
        const toggle = document.getElementById(toggleId),
            nav = document.getElementById(navId),
            bodypd = document.getElementById(bodyId),
            headerpd = document.getElementById(headerId);

        // Validar que existan todos los elementos
        if (toggle && nav && bodypd && headerpd) {
            toggle.addEventListener('click', () => {
                // Alternar la visibilidad de la sidebar
                nav.classList.toggle('show');

                // Reemplazar la clase para cambiar el ícono:
                if (toggle.classList.contains('bx-menu')) {
                    toggle.classList.replace('bx-menu', 'bx-x');
                } else {
                    toggle.classList.replace('bx-x', 'bx-menu');
                }

                // Alternar padding en el body y el header
                bodypd.classList.toggle('body-pd');
                headerpd.classList.toggle('body-pd');
            });
        }
    };

    showNavbar('header-toggle', 'nav-bar', 'body-pd', 'header');

    /*===== LINK ACTIVE =====*/
    const linkColor = document.querySelectorAll('.nav_link');
    function colorLink() {
        if (linkColor) {
            linkColor.forEach(l => l.classList.remove('active'));
            this.classList.add('active');
        }
    }
    linkColor.forEach(l => l.addEventListener('click', colorLink));
});
