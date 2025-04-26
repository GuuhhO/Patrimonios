document.addEventListener("DOMContentLoaded", function () {
    // Recupera o tema salvo do localStorage
    const savedTheme = localStorage.getItem("theme");

    if (savedTheme === "cores-invertidas") {
        aplicarTemaInvertido();
    }

    const btnMudarCores = document.querySelector("#toggleButton");
    btnMudarCores.addEventListener("click", function () {
        // Alterna o tema
        if (document.body.classList.toggle("cores-invertidas")) {
            localStorage.setItem("theme", "cores-invertidas"); // Salva o tema como 'cores-invertidas'
        } else {
            localStorage.removeItem("theme"); // Remove o tema salvo
        }

        // Atualiza ícones e estilos com base no estado atual
        atualizarElementos();
    });

    // Funções auxiliares
    function aplicarTemaInvertido() {
        document.body.classList.add("cores-invertidas");

        const tabela = document.getElementById("table");
        tabela?.classList.add("table-dark");

        const breadcrumbitemPatrimonios = document.getElementById("breadcrumbitemPatrimonios");
        breadcrumbitemPatrimonios?.classList.add("text-white");

        const iconInverter = document.getElementById("toggleButtonIcon");
        iconInverter?.classList.remove("bi-moon");
        iconInverter?.classList.add("bi-sun");
    }

    function atualizarElementos() {
        const tabela = document.getElementById("table");
        tabela?.classList.toggle("table-dark");

        const breadcrumbitemPatrimonios = document.getElementById("breadcrumbitemPatrimonios");
        if (document.body.classList.contains("cores-invertidas")) {
            breadcrumbitemPatrimonios?.classList.add("text-white");
        } else {
            breadcrumbitemPatrimonios?.classList.remove("text-white");
        }

        const iconInverter = document.getElementById("toggleButtonIcon");
        if (iconInverter.classList.contains("bi-moon")) {
            iconInverter.classList.remove("bi-moon");
            iconInverter.classList.add("bi-sun");
        } else {
            iconInverter.classList.remove("bi-sun");
            iconInverter.classList.add("bi-moon");
        }
    }
});
