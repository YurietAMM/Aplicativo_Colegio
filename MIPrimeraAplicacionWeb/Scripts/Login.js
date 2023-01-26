document.getElementById("btnIngresar").onclick = function () {
    var Usuario = document.getElementById("txtUsuario").value;
    var Contrasena = document.getElementById("txtContrasena").value;

    if (Usuario == "" && Contrasena == "") {
        alert("Porfavor llene ambos campos");
        console.log("Faltan llenar los campos");
        return;
    }

    $.get("Login/Ingresar?username=" + Usuario + "&password=" + Contrasena, function (data) {
            if (data == 1) {

                document.location.href = 'PaginaPrincipal/Index';

            } else {
                alert("Ocurrio Un Error");
            }
    });
    

}

document.getElementById("btnCerrarSecion").onclick = function () {
    document.location.href = 'Index';
}