using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MIPrimeraAplicacionWeb.Controllers
{
    public class SeccionController : Controller
    {
        // GET: Seccion
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ListarSeccion()
        {
            PruebaDataContext bd = new PruebaDataContext();
            var lista = (bd.Seccion.Where(p => p.BHABILITADO.Equals(1))
                .Select(p => new { p.IIDSECCION, p.NOMBRE })).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public int GuardarDatos(Seccion secioncita)
        {
            PruebaDataContext bd = new PruebaDataContext();
            int numRegisAfectados = 0;

            try
            {
                if (secioncita.IIDSECCION == 0)
                {
                    int numVeces = bd.Alumno.Where(p => p.NOMBRE.Equals(secioncita.NOMBRE)).Count();
                    if (numVeces == 0)
                    {
                        bd.Seccion.InsertOnSubmit(secioncita);
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
                    int numVeces = bd.Seccion.Where(p => p.NOMBRE.Equals(secioncita.NOMBRE)
                    && !p.IIDSECCION.Equals(secioncita.IIDSECCION)).Count();
                    if (numVeces == 0)
                    {
                        Seccion seccionSel = (bd.Seccion.Where(p => p.IIDSECCION.Equals(secioncita.IIDSECCION))).First();
                        seccionSel.NOMBRE = secioncita.NOMBRE;
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

        public JsonResult RecuperarDatos(int id)
        {
            PruebaDataContext bd = new PruebaDataContext();
            var lista = (bd.Seccion.Where(p => p.BHABILITADO.Equals(1) && p.IIDSECCION.Equals(id))
                .Select(
                p => new {
                    p.IIDSECCION,
                    p.NOMBRE
                })).First();

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public int EliminarSeccion(int idSeccion)
        {
            int numRegisAfectados = 0;
            PruebaDataContext bd = new PruebaDataContext();
            try
            {
                Seccion seccionSel = bd.Seccion.Where(p => p.IIDSECCION.Equals(idSeccion)).First();
                seccionSel.BHABILITADO = 0;
                bd.SubmitChanges();
                numRegisAfectados = 1;
            }
            catch (Exception ex) { numRegisAfectados = 0; throw ex; }
            return numRegisAfectados;
        }
    }
}