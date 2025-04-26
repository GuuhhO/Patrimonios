const expandir = document.querySelector("#toggle-btn");
const iconExpandir = document.querySelector("#botaoRecolher");

expandir.addEventListener("click", function () {
    document.querySelector("#sidebar").classList.toggle("expand");

    if (iconExpandir.classList.contains("bi-list")) {
        iconExpandir.classList.remove("bi-list");
        iconExpandir.classList.add("bi-list-nested");
    } else {
        iconExpandir.classList.remove("bi-list-nested");
        iconExpandir.classList.add("bi-list");
    }
});