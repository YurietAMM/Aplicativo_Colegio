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
        contenido += "<button class='btn btn-info' data-bs-toggle='modal' onclick='abrirModal(" + data[i].IIDPERIODO + ")' data-bs-target='#agregarEditarModal'><i class='bi bi-pencil-square'></i></button>";
        contenido += "<button  class='btn btn-danger' data-bs-toggle='modal' onclick='Eliminar(" + data[i].IIDPERIODO + ")'><i class='bi bi-trash3-fill'></i></button>";
        contenido += "</td>";
        contenido += "</tr>";
    }

    contenido += "</tbody>";
    contenido += "</table>";

    document.getElementById("divTabla").innerHTML = contenido;
}

function Listar() {
    $.get("Periodo/ListarPeriodos", function (data) {

        CrearListado(["#", "Nombre", "Fecha Inicio", "Fecha Fin", "Acciones"], data);

    });
}

Listar();

var nombrePeriodo = document.getElementById("txtNombrePeriodo");

nombrePeriodo.onkeyup = function () {
    var nombre = document.getElementById("txtNombrePeriodo").value;
    $.get("Periodo/BuscarPeriodoPorNombre/?nombrePeriodo=" + nombre, function (data) {
        CrearListado(["#", "Nombre", "Fecha Inicio", "Fecha Fin", "Acciones"], data);
    });
}

$("#txtModalFechaInicio").datepicker(
    {
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true
    }
);

$("#txtModalFechaFin").datepicker(
    {
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true
    }
);

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
}

function Agregar() {
    if (datosObliga() == true) {
        var frm = new FormData();
        var id = document.getElementById("txtModalId").value;
        var nombre = document.getElementById("txtModalNombre").value;
        var fechaInicio = document.getElementById("txtModalFechaInicio").value;
        var fechaFin = document.getElementById("txtModalFechaFin").value;
        frm.append("IIDPERIODO", id);
        frm.append("NOMBRE", nombre);
        frm.append("FECHAINICIO", fechaInicio);
        frm.append("FECHAFIN", fechaFin);
        frm.append("BHABILITADO", 1);

        if (confirm("¿Desea realmente guardar?") == 1) {
            $.ajax({
                type: "POST",
                url: "Periodo/GuardarDatos",
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
        

    }
    document.getElementById("btn-cerrarModal").onclick;
    quitarError();
}

function Editar(Id) {

    $.get("Periodo/RecuperarDatos/?id=" + Id, function (data) {
        document.getElementById("txtModalId").value = data[0].IIDPERIODO;
        document.getElementById("txtModalNombre").value = data[0].NOMBRE;
        document.getElementById("txtModalFechaInicio").value = data[0].FECHAINICIO;
        document.getElementById("txtModalFechaFin").value = data[0].FECHAFIN;
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

function cerrarModal() {
    var controles = document.getElementsByClassName("borrar");
    var numControles = controles.length;
    for (var i = 0; i < numControles; i++) {
        controles[i].value = "";
    }
    var btn = document.getElementById("btnAgregarEditar");
    btn.classList.remove("btn-info");
    btn.classList.add("btn-success");
    btn.value = "Agregar";
}

function Eliminar(id) {
    var frm = new FormData();
    frm.append("IIDPERIODO", id)

    if (confirm("¿Desea realmente borrar?") == 1) {
        $.ajax({
            type: "POST",
            url: "Periodo/EliminarPeriodo",
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
}