const unidadeMap = {
    "Itabira/MG": [
        "01 - Galpão Matriz Itabira",
        "02 - Câmara Municipal de Itabira",
        "03 - Prefeitura Municipal de Itabira",
        "04 - HNSD"
    ],
    "Ribeirão das Neves/MG": [
        "05 - Galpão Ribeirão das Neves",
        "06 - Prefeitura Municipal de Ribeirão das Neves"
    ],
    "Alfenas/MG": [
        "07 - Unifenas",
        "08 - Galpão HUAV"
    ],
    "Manutenção": [
        "Macrosolution",
        "DataFilme",
        "ScanSystem"
    ]
};

document.addEventListener("DOMContentLoaded", function () {
    const localidadeSelect = document.getElementById("localidade");
    const unidadeSelect = document.getElementById("unidade");

    // Obtém a unidade salva no banco de dados
    const unidadeAtual = unidadeSelect.getAttribute("data-selecionado");

    // Se houver uma localidade selecionada, carrega as unidades correspondentes
    if (localidadeSelect.value) {
        atualizarUnidades(localidadeSelect.value, unidadeAtual);
    }

    function atualizarUnidades(localidadeSelecionada, unidadeSalva = null) {
        const unidades = unidadeMap[localidadeSelecionada] || [];

        unidadeSelect.innerHTML = "<option value='0'>Selecione uma opção...</option>";

        unidades.forEach(unidade => {
            const option = document.createElement("option");
            option.value = unidade;
            option.textContent = unidade;
            if (unidade === unidadeSalva) {
                option.selected = true; // Mantém a unidade salva selecionada
            }
            unidadeSelect.appendChild(option);
        });
    }

    // Evento ao alterar a localidade manualmente
    localidadeSelect.addEventListener("change", function () {
        atualizarUnidades(this.value);
    });
});

document.getElementById("status").addEventListener("change", function () {
    const status = this.value;
    const localidadeSelect = document.getElementById("localidade");
    const unidadeSelect = document.getElementById("unidade");

    if (status === "Manutenção") {
        localidadeSelect.innerHTML = "<option value='Manutenção' selected>Manutenção</option>";

        unidadeSelect.innerHTML = `
            <option value="Macrosolution">Macrosolution</option>
            <option value="DataFilme">DataFilme</option>
            <option value="ScanSystem">ScanSystem</option>
        `;
    } else {
        localidadeSelect.innerHTML = `
            <option value="0" selected>Selecione uma opção...</option>
            <option value="Itabira/MG">Itabira/MG</option>
            <option value="Ribeirão das Neves/MG">Ribeirão das Neves/MG</option>
            <option value="Alfenas/MG">Alfenas/MG</option>
        `;

        unidadeSelect.innerHTML = "<option value='0' selected>Selecione uma opção...</option>";
    }
});

document.getElementById("submitButton").addEventListener("click", function () {
    const localidade = document.getElementById("localidade").value;
    const unidade = document.getElementById("unidade").value;

    if (localidade === "0" || localidade === "" || unidade === "0" || unidade === "") {
        alert("Por favor selecione uma localidade ou unidade válida.");
    }
});
