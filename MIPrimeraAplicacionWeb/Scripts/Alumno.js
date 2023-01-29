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
        contenido += "<button class='btn btn-info' data-bs-toggle='modal' onclick='Editar(" + data[i].IIDALUMNO + ")' data-bs-target='#agregarEditarModal'><i class='bi bi-pencil-square'></i></button>";
        contenido += "<button  class='btn btn-danger' data-bs-toggle='modal' onclick='Eliminar(" + data[i].IIDALUMNO + ")'><i class='bi bi-trash3-fill'></i></button>";
        contenido += "</td>";
        contenido += "</tr>";
    }

    contenido += "</tbody>";
    contenido += "</table>";

    document.getElementById("divTabla").innerHTML = contenido;
}

Listar();

function LlenarComboBox(data , control, primerElemento) {
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

$.get("Alumno/ListarSexo", function (data) {
    LlenarComboBox(data, document.getElementById("cboSexo"), true);
    /*LlenarComboBox(data, document.getElementById("cboModalSexo"), true);*/
});

$("#txtModalFechaNaci").datepicker(
    {
        dateFormat: "dd/mm/yy",
        changeMonth: true,
        changeYear: true
    }
);

function Listar() {
    $.get("Alumno/ListarAlumnos", function (data) {
        CrearListado(["#", "Nombres", "Apellido Paterno", "Apellido Materno", "Telefono Padre", "Acciones"], data);
    });
}

var btnBuscar = document.getElementById("btnBuscar");

btnBuscar.onclick = function () {
    var cboSexo = document.getElementById("cboSexo").value;

    if (cboSexo == 0) {
        Listar();
    } else {

        $.get("Alumno/BuscarSexo/?sexo=" + cboSexo, function (data) {
            CrearListado(["#", "Nombres", "Apellido Paterno", "Apellido Materno", "Telefono Padre", "Acciones"], data);
        });
    }
}

var btnLimpiar = document.getElementById("btnLimpiar");

btnLimpiar.onclick = function () {
    Listar();
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

    var radios = document.getElementsByClassName("form-check-input");
    for (var i = 0; i < radios.length; i++) {
        radios[i].checked = false;
    }
}

function Agregar() {
    datosObliga();
    if (datosObliga() == true) {
        var frm = new FormData();
        var id = document.getElementById("txtModalId").value;
        var nombre = document.getElementById("txtModalNombre").value;
        var apePater = document.getElementById("txtModalApePate").value;
        var apeMater = document.getElementById("txtModalApeMate").value;
        var fechaNaci = document.getElementById("txtModalFechaNaci").value;
        /*var sexo = document.getElementById("cboModalSexo").value;*/
        var sexo;
        if (document.getElementById("radioModalMaculino").checked == true) {
            sexo = 1;
        }
        else {
            sexo = 2;
        }
        var telPater = document.getElementById("txtModalTelPadre").value;
        var telMater = document.getElementById("txtModalTelMadre").value;
        var numHerma = document.getElementById("txtModalNumHerma").value;
        frm.append("IIDALUMNO", id);
        frm.append("NOMBRE", nombre);
        frm.append("APPATERNO", apePater);
        frm.append("APMATERNO", apeMater);
        frm.append("FECHANACIMIENTO", fechaNaci);
        frm.append("IIDSEXO", sexo);
        frm.append("TELEFONOPADRE", telPater);
        frm.append("TELEFONOMADRE", telMater);
        frm.append("NUMEROHERMANOS", numHerma);
        frm.append("BHABILITADO", 1);

        if (confirm("¿Desea realmente guardar?") == 1) {
            $.ajax({
                type: "POST",
                url: "Alumno/GuardarDatos",
                data: frm,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data == 1) {
                        Listar();
                    } else {
                        if (data == 0) {
                            alert("ocurrio un error");
                        }
                        else {
                            alert("Ya esta registrado ese nombre");
                        }
                    }
                }
            });
        }


    }
    document.getElementById("btn-cerrarModal").onclick;
    quitarError();
}

function Editar(id) {
    $.get("Alumno/RecuperarDatos/?id=" + id, function (data) {
        document.getElementById("txtModalId").value = data.IIDALUMNO;
        document.getElementById("txtModalNombre").value = data.NOMBRE;
        document.getElementById("txtModalApePate").value = data.APPATERNO;
        document.getElementById("txtModalApeMate").value = data.APMATERNO;
        document.getElementById("txtModalFechaNaci").value = data.FECHANACIMIENTO;
        /*document.getElementById("cboModalSexo").value = data.IIDSEXO;*/
        if (data.IIDSEXO == 1) {
            document.getElementById("radioModalMaculino").checked = true;
        } else {
            document.getElementById("radioModalFemenino").checked = true;
        }
        document.getElementById("txtModalTelPadre").value = data.TELEFONOPADRE;
        document.getElementById("txtModalTelMadre").value = data.TELEFONOMADRE;
        document.getElementById("txtModalNumHerma").value = data.NUMEROHERMANOS;
        
    });
    var btn = document.getElementById("btnAgregarEditar");
    btn.classList.remove("btn-success");
    btn.classList.add("btn-info");
    btn.value = "Editar";
}

function Eliminar(id) {
    if (confirm("Desea eliminar?") == 1) {
        $.get("Alumno/EliminarAlumno/?idAlumno=" + id, function (data) {
            if (data == 0) {
                alert("Ocurrio un error");
            }
            else {
                Listar();
            }
        });
    }
}