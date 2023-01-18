using MIPrimeraAplicacionWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MIPrimeraAplicacionWeb.Controllers
{
    public class AlumnoController : Controller
    {
        // GET: Alumno
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ListarAlumnos()
        {
            PruebaDataContext bd = new PruebaDataContext();
            var lista = (bd.Alumno.Where(p => p.BHABILITADO.Equals(1))
            .Select(p => new { p.IIDALUMNO, p.NOMBRE, p.APPATERNO, p.APMATERNO, p.TELEFONOPADRE })).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListarSexo()
        {
            PruebaDataContext bd = new PruebaDataContext();
            var lista = (bd.Sexo.Where(p => p.BHABILITADO.Equals(1)).Select(p => new { IID = p.IIDSEXO, p.NOMBRE })).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult BuscarSexo(int sexo)
        {
            PruebaDataContext bd = new PruebaDataContext();
            var lista = (bd.Alumno.Where(p => p.BHABILITADO.Equals(1) && p.IIDSEXO.Equals(sexo))
                .Select(p => new { p.IIDALUMNO, p.NOMBRE, p.APPATERNO, p.APMATERNO, p.TELEFONOPADRE })).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public int EliminarAlumno(int idAlumno)
        {
            int numRegisAfectados = 0;
            PruebaDataContext bd = new PruebaDataContext();
            try
            {
                Alumno alumnoSel = bd.Alumno.Where(p => p.IIDALUMNO.Equals(idAlumno)).First();
                alumnoSel.BHABILITADO = 0;
                bd.SubmitChanges();
                numRegisAfectados = 1;
            }
            catch(Exception ex) { numRegisAfectados = 0; throw ex; }
            return numRegisAfectados;
        }

        public JsonResult RecuperarDatos(int id)
        {
            PruebaDataContext bd = new PruebaDataContext();
            var lista = (bd.Alumno.Where(p => p.BHABILITADO.Equals(1) && p.IIDALUMNO.Equals(id))
                .Select(
                p => new { 
                    p.IIDALUMNO, p.NOMBRE,  p.APPATERNO, p.APMATERNO, 
                    FECHANACIMIENTO = ((DateTime)p.FECHANACIMIENTO).ToShortDateString(), 
                    p.IIDSEXO, p.TELEFONOPADRE, p.TELEFONOMADRE, p.NUMEROHERMANOS })).First();
            
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public int GuardarDatos(Alumno alumnito)
        {
            PruebaDataContext bd = new PruebaDataContext();
            int numRegisAfectados = 0;

            try
            {
                if (alumnito.IIDALUMNO == 0)
                {
                    int numVeces = bd.Alumno.Where(p => p.NOMBRE.Equals(alumnito.NOMBRE)).Count();
                    if (numVeces == 0)
                    {
                        alumnito.IIDTIPOUSUARIO = 'A';
                        bd.Alumno.InsertOnSubmit(alumnito);
                        bd.SubmitChanges();
                        numRegisAfectados = 1;
                    }
                    else
                    {
                        numRegisAfectados = -1;
                    }
                }
                else
                {
                    int numVeces = bd.Alumno.Where(p => p.NOMBRE.Equals(alumnito.NOMBRE) 
                    && p.APPATERNO.Equals(alumnito.APPATERNO) && p.APMATERNO.Equals(alumnito.APMATERNO)).Count();
                    if(numVeces == 0)
                    {
                        Alumno alumnoSel = (bd.Alumno.Where(p => p.IIDALUMNO.Equals(alumnito.IIDALUMNO))).First();
                        alumnoSel.NOMBRE = alumnito.NOMBRE;
                        alumnoSel.APPATERNO = alumnito.APPATERNO;
                        alumnoSel.APMATERNO = alumnito.APMATERNO;
                        alumnoSel.FECHANACIMIENTO = alumnito.FECHANACIMIENTO;
                        alumnoSel.IIDSEXO = alumnito.IIDSEXO;
                        alumnoSel.TELEFONOPADRE = alumnito.TELEFONOPADRE;
                        alumnoSel.TELEFONOMADRE = alumnito.TELEFONOMADRE;
                        alumnoSel.NUMEROHERMANOS = alumnito.NUMEROHERMANOS;
                        bd.SubmitChanges();
                        numRegisAfectados = 1;
                    } 
                    else
                    {
                        numRegisAfectados = -1;
                    }
                }
            }
            catch (Exception e)
            {
                numRegisAfectados = 0;
                throw e;
            }

            return numRegisAfectados;
        }
    }
}