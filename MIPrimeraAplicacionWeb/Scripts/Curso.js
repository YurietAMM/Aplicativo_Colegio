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
        contenido += "<button class='btn btn-info' data-bs-toggle='modal' onclick='abrirModal(" + data[i].IIDCURSO + ")' data-bs-target='#agregarEditarModal'><i class='bi bi-pencil-square'></i></button>";
        contenido += "<button  class='btn btn-danger' data-bs-toggle='modal' onclick='Eliminar(" + data[i].IIDCURSO + ")'><i class='bi bi-trash3-fill'></i></button>";
        contenido += "</td>";
        contenido += "</tr>";
    }

    contenido += "</tbody>";
    contenido += "</table>";

    document.getElementById("divTabla").innerHTML = contenido;
}

function Listar() {
    $.get("Curso/ListarCurso", function (data) {

        CrearListado(["#", "Nombre", "Descripción", "Acciones"], data);

    });
}

Listar();

var btnBuscar = document.getElementById("btnBuscar");

btnBuscar.onclick = function () {
    console.log("click en buscar");
    var nombre = document.getElementById("txtNombre").value;
    $.get("Curso/BuscarCursoPorNombre/?nombreCurso="+nombre, function (data) {
        CrearListado(["#", "Nombre", "Descripción", "Acciones"], data);
    });
}

var btnLimpiar = document.getElementById("btnLimpiar");

btnLimpiar.onclick = function () {
    var nombre = document.getElementById("txtNombre").value;
    $.get("Curso/ListarCurso", function (data) {

        CrearListado(["#", "Nombre", "Descripción", "Acciones"], data);

    });
    document.getElementById("txtNombre").value = "";

}

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

    //Pedro Pedrin
}

function Agregar() {
    if (datosObliga() == true) {
        var frm = new FormData();
        var id = document.getElementById("txtModalId").value;
        var nombre = document.getElementById("txtModalNombre").value;
        var descripcion = document.getElementById("txtModalDescripcion").value;
        frm.append("IIDCURSO", id);
        frm.append("NOMBRE", nombre);
        frm.append("DESCRIPCION", descripcion);
        frm.append("BHABILITADO", 1);

        if (confirm("¿Desea realmente guardar?") == 1) {
            $.ajax({
                type: "POST",
                url: "Curso/GuardarDatos",
                data: frm,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data != 0) {
                        Listar();
                    } else {
                        alert("ocurrio un error");
                    }
                }
            });
        }
        else {

        }

    } else {

    }
    document.getElementById("btn-cerrarModal").onclick;
    quitarError();
}

function Editar(Id) {
    $.get("Curso/RecuperarDatos/?id=" + Id, function (data) {
        document.getElementById("txtModalId").value = data[0].IIDCURSO;
        document.getElementById("txtModalNombre").value = data[0].NOMBRE;
        document.getElementById("txtModalDescripcion").value = data[0].DESCRIPCION;
        var btn = document.getElementById("btnAgregarEditar");
        btn.classList.remove("btn-success");
        btn.classList.add("btn-info");
        btn.value = "Editar";
    });
    Agregar();
}

function abrirModal(ID) {
    quitarError();
    if (ID !== 0) {
        Editar(ID);
    }
}

function Eliminar(id) {
    var frm = new FormData();
    frm.append("IIDCURSO", id)

    if (confirm("¿Desea realmente borrar?") == 1) {
        $.ajax({
            type: "POST",
            url: "Curso/EliminarCurso",
            data: frm,
            contentType: false,
            processData: false,
            success: function (data) {
                if (data != 0) {
                    Listar();
                } else {
                    alert("ocurrio un error");
                }
            }
        });
    }
    else {

    }

}