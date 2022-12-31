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
        contenido += "<button class='btn btn-info' data-bs-toggle='modal' onclick='Editar(" + data[i].IIDDOCENTE + ")' data-bs-target='#agregarEditarModal'><i class='bi bi-pencil-square'></i></button>";
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

function Agregar() {
    if (datosObliga() == true) {
        var frm = new FormData();
        var id = document.getElementById("txtModalId").value;
        var nombre = document.getElementById("txtModalNombre").value;
        var apePate = document.getElementById("txtModalApePate").value;
        var apeMate = document.getElementById("txtModalApeMate").value;
        var direccion = document.getElementById("txtModalDirecci").value;
        var telCel = document.getElementById("txtModalTelCel").value;
        var telFijo = document.getElementById("txtModalTelFijo").value;
        var email = document.getElementById("txtModalEmail").value;
        var sexo = document.getElementById("cboModalSexo").value;
        var fechaContrato = document.getElementById("txtModalFechaContrato").value;
        var modalidadContrato = document.getElementById("cboModalContra").value;
        var foto = (document.getElementById("imgModalFoto").src).replace("data:image/png;base64,", "");

        frm.append("IIDDOCENTE", id);
        frm.append("NOMBRE", nombre);
        frm.append("APPATERNO", apePate);
        frm.append("APMATERNO", apeMate);
        frm.append("DIRECCION", direccion);
        frm.append("TELEFONOCELULAR", telCel);
        frm.append("TELEFONOFIJO", telFijo);
        frm.append("EMAIl", email);
        frm.append("IIDSEXO", sexo);
        frm.append("FECHACONTRATO", fechaContrato);
        frm.append("IIDMODALIDADCONTRATO", modalidadContrato);
        frm.append("CADENAFOTO", foto);
        frm.append("BHABILITADO", 1);
        frm.append("IIDTIPOUSUARIO", "D");
        frm.append("bTieneUsuario", 0);

        if (confirm("¿Desea realmente guardar?") == 1) {
            $.ajax({
                type: "POST",
                url: "Docente/GuardarDatos",
                data: frm,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data != 0) {
                        console.log("todo bien");
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

    $.get("Docente/RecuperarDatos/?id=" + Id, function (data) {
        document.getElementById("txtModalId").value = data[0].IIDDOCENTE;
        document.getElementById("txtModalNombre").value = data[0].NOMBRE;
        document.getElementById("txtModalApePate").value = data[0].APPATERNO;
        document.getElementById("txtModalApeMate").value = data[0].APMATERNO;
        document.getElementById("txtModalDirecci").value = data[0].DIRECCION;
        document.getElementById("txtModalTelCel").value = data[0].TELEFONOCELULAR;
        document.getElementById("txtModalTelFijo").value = data[0].TELEFONOFIJO;
        document.getElementById("txtModalEmail").value = data[0].EMAIL;
        document.getElementById("cboModalSexo").value = data[0].IIDSEXO;
        document.getElementById("txtModalFechaContrato").value = data[0].FECHACONTRATO;
        document.getElementById("cboModalContra").value = data[0].IIDMODALIDADCONTRATO;
        document.getElementById("imgModalFoto").src = "data:image/png;base64,"+data[0].FOTOMOSTRAR;
    });
    var btn = document.getElementById("btnAgregarEditar");
    btn.classList.remove("btn-success");
    btn.classList.add("btn-info");
    btn.value = "Editar";
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

var btnModalFoto = document.getElementById("btnModalFoto");

btnModalFoto.onchange = function (e) {
    var file = document.getElementById("btnModalFoto").files[0];
    var reader = new FileReader();
    if (reader != null) {
        reader.onloadend = function () {
            var img = document.getElementById("imgModalFoto");
            img.src = reader.result;
        }
    }
    reader.readAsDataURL(file);
}