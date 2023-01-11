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
        contenido += "<button class='btn btn-info' data-bs-toggle='modal' onclick='Editar(" + data[i].IID + ")' data-bs-target='#agregarEditarModal'><i class='bi bi-pencil-square'></i></button>";
        contenido += "<button  class='btn btn-danger' data-bs-toggle='modal' onclick='AbrirEliminar(" + data[i].IID + ")' data-bs-target='#borrarModal'><i class='bi bi-trash3-fill'></i></button>";
        contenido += "</td>";
        contenido += "</tr>";
    }

    contenido += "</tbody>";
    contenido += "</table>";

    document.getElementById("divTabla").innerHTML = contenido;
}

//function Listar() {
//    $.get("Matricula/ListarMatricula", function (data) {
//        CrearListado(["#", "Nombre Periodo", "Nombre Curso", "Nombre Docente", "Nombre Grado", "Acciones"], data);
//    });
//}

//Listar();

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

function LlenarCombos() {

    $.get("Matricula/ListarPeriodo", function (data) {
        LlenarComboBox(data, document.getElementById("cboModalPeriodo"), true);
    });

    $.get("Matricula/ListarGradoSeccion", function (data) {
        LlenarComboBox(data, document.getElementById("cboModalGradoSeccion"), true);
    });

    $.get("Matricula/ListarAlumnos", function (data) {
        LlenarComboBox(data, document.getElementById("cboModalAlumno"), true);
    });

}

LlenarCombos();

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
        var periodo = document.getElementById("cboModalPeriodo").value;
        var gradoSeccion = document.getElementById("cboModalGradoSeccion").value;
        var alumno = document.getElementById("cboModalAlumno").value

        frm.append("IIDMATRICULA", id);
        frm.append("IIDPERIODO", periodo);
        frm.append("IIDGRADOSECCION", gradoSeccion);
        frm.append("IIDALUMNO", alumno);
        frm.append("BHABILITADO", 1);

        if (confirm("¿Desea realmente guardar?") == 1) {
            $.ajax({
                type: "POST",
                url: "Matricula/GuardarDatos",
                data: frm,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data != 0) {
                        console.log("todo bien");
                        //Listar();
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

    $.get("Matricula/RecuperarDatos/?id=" + Id, function (data) {
        document.getElementById("txtModalId").value = data[0].IID;
        document.getElementById("cboModalPeriodo").value = data[0].IIDPERIODO;
        document.getElementById("cboModalGradoSeccion").value = data[0].IIDGRADOSECCION;
        document.getElementById("cboModalAula").value = data[0].IIDAULA;
        document.getElementById("cboModalDocente").value = data[0].IIDDOCENTE;
        if (document.getElementById("cboModalPeriodo").value != "" && document.getElementById("cboModalGradoSeccion").value != "") {
            $.get("GradoSeccionAula/ListarCurso/?IIDPERIODO=" + document.getElementById("cboModalPeriodo").value + "&IIDGRADOSECCION=" + document.getElementById("cboModalGradoSeccion").value, function (rpta) {
                LlenarComboBox(rpta, document.getElementById("cboModalCurso"), true);
                document.getElementById("cboModalCurso").value = data[0].IIDCURSO;
            });
        }
    });
    var btn = document.getElementById("btnAgregarEditar");
    btn.classList.remove("btn-success");
    btn.classList.add("btn-info");
    btn.value = "Editar";
}

function Eliminar() {
    var id = document.getElementById("spanModalId").value;
    $.get("Matricula/Eliminar/?id=" + id, function (data) {
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
