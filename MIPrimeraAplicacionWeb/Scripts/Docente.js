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
        contenido += "<button class='btn btn-info' data-bs-toggle='modal' onclick='abrirModal(" + data[i].IIDDOCENTE + ")' data-bs-target='#agregarEditarModal'><i class='bi bi-pencil-square'></i></button>";
        contenido += "<button  class='btn btn-danger' data-bs-toggle='modal' onclick='Eliminar(" + data[i].IIDDOCENTE + ")'><i class='bi bi-trash3-fill'></i></button>";
        contenido += "</td>";
        contenido += "</tr>";
    }

    contenido += "</tbody>";
    contenido += "</table>";

    document.getElementById("divTabla").innerHTML = contenido;
}

$("#txtModalFechaContrato").datepicker(
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

function LlenarComboBox(data, control, primerElemento) {
    var contenido = "";
    var nregistros = data.length;

    if (primerElemento == true) {
        contenido += "<option value='0'>";
        contenido += "--Seleccione--"
        contenido += "</option>"
    }

    for (var i = 0; i < nregistros; i++) {
        contenido += "<option value='" + data[i].IID + "'>";
        contenido += data[i].NOMBRE;
        contenido += "</option>";
    }

    control.innerHTML = contenido;
}

function Listar() {
    $.get("Docente/ListarDocentes", function (data) {
        CrearListado(["#", "Nombres", "Email", "Telefono Celular", "Acciones"], data);
    });
}

function ListarComboModalidad() {
    $.get("Docente/LlenarCombo", function (data) {
        LlenarComboBox(data, document.getElementById("cboModoPago"), true);
        LlenarComboBox(data, document.getElementById("cboModalContra"), true);
    });
}

var cboModoPago = document.getElementById("cboModoPago");

$.get("Docente/LlenarComboSexo", function (data) {
    LlenarComboBox(data, document.getElementById("cboModalSexo"), true);
});

cboModoPago.onchange = function (cboModoPago) {
    var modo = document.getElementById("cboModoPago").value;
    if (modo == 0) {
        Listar();
    } else {
        $.get("Docente/ListarDocentesPorModalidad/?modo=" + modo, function (data) {
            CrearListado(["#", "Nombres", "Email", "Telefono Celular", "Acciones"], data);
        });
    }
}

ListarComboModalidad();

Listar();



function abrirModal(ID) {
    quitarError();

    if (ID !== 0) {
        Editar(ID);
    }
}

function Eliminar(id) {
    var frm = new FormData();
    frm.append("IIDDOCENTE", id)

    if (confirm("¿Desea realmente borrar?") == 1) {
        $.ajax({
            type: "POST",
            url: "Docente/EliminarDocente",
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