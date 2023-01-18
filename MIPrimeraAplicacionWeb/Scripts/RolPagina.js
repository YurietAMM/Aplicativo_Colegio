function CrearListado(arrayColumna, data) {
    var contenido = "";

    contenido += "<table id='tablas' class='table'>";
    contenido += "<thead>";
    contenido += "<tr>";
    for (var i = 0; i < arrayColumna.length; i++) {
        contenido += "<th scope='col'>";
        contenido += arrayColumna[i];
        contenido += "</th>";
    }

    contenido += "</tr>";
    contenido += "</thead>";
    contenido += "<tbody>";
    var nfilas = data.length;
    var llaves = Object.keys(data[0]);
    for (var i = 0; i < nfilas; i++) {
        contenido += "    <tr>";
        for (var x = 0; x < llaves.length; x++) {
            var valorLlaves = llaves[x];
            contenido += "<td>"
            contenido += data[i][valorLlaves];
            contenido += "</td>"

        }
        contenido += "<td>";
        contenido += "<button class='btn btn-info' data-bs-toggle='modal' onclick='Editar(" + data[i].IIDROL + ")' data-bs-target='#agregarEditarModal'><i class='bi bi-pencil-square'></i></button>";
        contenido += "<button  class='btn btn-danger' data-bs-toggle='modal' onclick='AbrirEliminar(" + data[i].IIDROL + ")' data-bs-target='#borrarModal'><i class='bi bi-trash3-fill'></i></button>";
        contenido += "</td>";
        contenido += "</tr>";
    }

    contenido += "</tbody>";
    contenido += "</table>";

    document.getElementById("divTabla").innerHTML = contenido;
}

function Listar() {
    $.get("RolPagina/ListarRol", function (data) {
        CrearListado(["#", "Nombre Rol", "Descripción"], data);
    });

    $.get("RolPagina/ListarPaginas", function (data) {
        var contenido = "<tbody>"
        for (var i = 0; i < data.length; i++) {
            contenido += "<tr>";
            contenido += "<td>";
            contenido += "<input type='checkbox' id=" + data[i].IIDPAGINA + " class='checkBox form-check' />";
            contenido += "</td>";
            contenido += "<td>";
            contenido += data[i].MENSAJE;
            contenido += "</td>";
            contenido += "</tr>";
        }
        contenido += "</tbody>";

        document.getElementById("divModalTabla").innerHTML = contenido;
    });
}

Listar();

function quitarError() {
    var controlesObliga = document.getElementsByClassName("obligatorios");

    for (var i = 0; i < controlesObliga.length; i++) {
        controlesObliga[i].parentNode.classList.remove("error");
    }
}

function datosObliga() {
    var exitoso = true;
    var controlesObliga = document.getElementsByClassName("obligatorios");
    var numControlesObliga = controlesObliga.length;
    for (var i = 0; i < numControlesObliga; i++) {
        if (controlesObliga[i].value == "") {
            exitoso = false;
            controlesObliga[i].parentNode.classList.add("error");
        } else {
            controlesObliga[i].parentNode.classList.remove("error");
        }
    }
    return exitoso;
}

function cerrarModal() {
    var controles = document.getElementsByClassName("borrar");
    quitarError();
    var numControles = controles.length;
    for (var i = 0; i < numControles; i++) {
        controles[i].value = "";
    }
    var btn = document.getElementById("btnAgregarEditar");
    btn.classList.remove("btn-info");
    btn.classList.add("btn-success");
    btn.value = "Agregar";

    var checkBox = document.getElementsByClassName("checkBox");
    for (var i = 0; i < checkBox.length; i++) {
        checkBox[i].checked = false;
    }
}

function Agregar() {
    if (datosObliga() == true) {
        var frm = new FormData();
        var id = document.getElementById("txtModalId").value;
        var nombre = document.getElementById("txtModalNombre").value;
        var descripcion = document.getElementById("txtModalDescripcion").value;

        var valorEnviar = "";
        var box = document.getElementsByClassName("checkBox");
        var boxLength = box.length;
        for (var i = 0; i < boxLength; i++) {
            if (box[i].checked == true) {
                valorEnviar += box[i].id;
                valorEnviar += "$";
                console.log(valorEnviar);
            }
        }

        valorEnviar = valorEnviar.substring(0, valorEnviar.length - 1);

        frm.append("valorEnviar", valorEnviar);
        frm.append("IIDROL", id);
        frm.append("NOMBRE", nombre);
        frm.append("DESCRIPCION", descripcion);
        frm.append("BHABILITADO", 1);

        if (confirm("¿Desea realmente guardar?") == 1) {
            $.ajax({
                type: "POST",
                url: "RolPagina/GuardarDatos",
                data: frm,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data == 1) {
                        Listar();
                        document.getElementById("btn-cerrarModal").click();
                    } else {
                        alert("ocurrio un error");
                    }
                }
            });
        }


    }
    quitarError();
}

function Editar(Id) {

    $.get("RolPagina/RecuperarDatos/?id=" + Id, function (data) {
        document.getElementById("txtModalId").value = data.IIDROL;
        document.getElementById("txtModalNombre").value = data.NOMBRE;
        document.getElementById("txtModalDescripcion").value = data.DESCRIPCION;
    });

    $.get("RolPagina/RecuperarDatosCheckBox/?id=" + Id, function (data) {
        for (var i = 0; i < data.length; i++) {
            if (data[i].BHABILITADO == 1) {
                document.getElementById(data[i].IIDPAGINA).checked = true;
            } 
        }

    });


    var btn = document.getElementById("btnAgregarEditar");
    btn.classList.remove("btn-success");
    btn.classList.add("btn-info");
    btn.value = "Editar";
}

function Eliminar() {
    var id = document.getElementById("spanModalId").value;
    $.get("RolPagina/Eliminar/?id=" + id, function (data) {
        if (data == 0) {
            alert("Ocurrio un error");
        }
        else {
            Listar();
            document.getElementById("btn-cerrarModal").click();
        }
    });
}

function AbrirEliminar(id) {
    document.getElementById("spanModalId").value = id;
}
