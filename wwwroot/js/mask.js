var telefone = document.getElementById("phoneNumberInput");
telefone.addEventListener("input", function () {
    var limparCampo = telefone.value.replace(/\D/g, "").substring(0, 11);
    var numerosArray = limparCampo.split("");
    var numeroFormatado = "";
    if (numerosArray.length > 0) {
        numeroFormatado += `(${numerosArray.slice(0,2).join("")})`;
    }

    if (numerosArray.length > 2) {
        numeroFormatado += ` ${numerosArray.slice(2, 7).join("")}`;
    }

    if (numerosArray.length > 7) {
        numeroFormatado += `-${numerosArray.slice(7, 11).join("")}`;
    }
    telefone.value = numeroFormatado;
});