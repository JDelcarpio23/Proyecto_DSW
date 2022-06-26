$('#myModal').on('shown.bs.modal', function () {
    $('#myInput').trigger('focus')
});

$(document).ready(function () {
    $('#contactar').click(function () {
        re = /^([\da-z_\.-]+)@([\da-z\.-]+)\.([a-z\.]{2,6})$/
        var porId = document.getElementById("email").value;
        if ($('#nombre').val() == "") {
            document.getElementById("nombre").focus();
            alert("Debes Ingresar tu Nombre ");
            return false;
        } else if ($('#mail').val() == "") {
            document.getElementById("mail").focus();
            alert("Debes Ingresar Email");
            return false;
        }
        else if (!re.exec(porId)) {
            document.getElementById("mail").focus();
            alert("Email No Valido");
            return false;
        } else if ($('#campo').val() == "") {
            document.getElementById("campo").focus();
            alert("Debes Ingresar un Mensaje");
            return false;
        }
    });
