function CrearListado(arrayColumna, data) {
    var contenido = "";

    contenido += "<table id='tablas' class='table'>";
    contenido += "<thead>";
    contenido += "    <tr>";
    for (var i = 0; i < arrayColumna.length; i++) {
        contenido += "<th scope='col'>";
        contenido += arrayColumna[i];
        contenido += "</th>";
    }

    contenido += "    </tr>";
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
        contenido += "<button  class='btn btn-danger' data-bs-toggle='modal' data-bs-target='#borrarModal'><i class='bi bi-trash3-fill'></i></button>";
        contenido += "</td>";
        contenido += "    </tr>";
    }
    
    contenido += "</tbody>";
    contenido += "</table>";

    document.getElementById("divTabla").innerHTML = contenido;
    $("#tablas").dataTable({ searching: false });
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