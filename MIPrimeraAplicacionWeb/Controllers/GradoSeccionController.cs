using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MIPrimeraAplicacionWeb.Controllers
{
    public class GradoSeccionController : Controller
    {
        // GET: GradoSeccion
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ListarGradoSeccion()
        {
            PruebaDataContext bd = new PruebaDataContext();
            var lista = (from gradoSec in bd.GradoSeccion 
                        join sec in bd.Seccion 
                        on gradoSec.IIDSECCION equals sec.IIDSECCION 
                        join grad in bd.Grado
                        on gradoSec.IIDGRADO equals grad.IIDGRADO
                        where gradoSec.BHABILITADO.Equals(1)
                        select new
                        {
                            gradoSec.IID,
                            NombreGrado = grad.NOMBRE,
                            NombreSeccion = sec.NOMBRE
                        }).ToList();
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecuperarDatos(int id)
        {
            PruebaDataContext bd = new PruebaDataContext();

            var consulta = (bd.GradoSeccion.Where(p => p.IID.Equals(id)))
                .Select(p => new {p.IID, p.IIDGRADO, p.IIDSECCION});

            return Json(consulta, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarSeccion()
        {
            PruebaDataContext bd = new PruebaDataContext();

            var lista = (bd.Seccion.Where(p => p.BHABILITADO.Equals(1))
                .Select(p => new {
                   IID = p.IIDSECCION,
                    p.NOMBRE
                })).ToList();

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarGrado()
        {
            PruebaDataContext bd = new PruebaDataContext();

            var lista = (bd.Grado.Where(p => p.BHABILITADO.Equals(1))
                .Select(p => new {
                    IID = p.IIDGRADO,
                    p.NOMBRE
                })).ToList();
             
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

         public int GuardarDatos(GradoSeccion gradiSeccionsita)
        {
            PruebaDataContext bd = new PruebaDataContext();
            int numRegisAfectados = 0;

            try
            {
                if (gradiSeccionsita.IID == 0)
                {
                    bd.GradoSeccion.InsertOnSubmit(gradiSeccionsita);
                    bd.SubmitChanges();
                    numRegisAfectados = 1;
                }
                else
                {
                    GradoSeccion gradoSeccionSel = bd.GradoSeccion.Where(p => p.IID.Equals(gradiSeccionsita.IID)).First();
                    gradoSeccionSel.IIDGRADO = gradiSeccionsita.IIDGRADO;
                    gradoSeccionSel.IIDSECCION = gradiSeccionsita.IIDSECCION;
                    bd.SubmitChanges();
                    numRegisAfectados = 1;
                }
            }
            catch(Exception ex) { numRegisAfectados = 0; throw ex; }

            return numRegisAfectados;
         }

        public int Eliminar(int id)
        {
            PruebaDataContext bd = new PruebaDataContext();
            int numRegisAfectados = 0;
            try
            {
                GradoSeccion seleccionado = (bd.GradoSeccion.Where(p => p.IID.Equals(id))).First();
                seleccionado.BHABILITADO = 0;
                bd.SubmitChanges();
                numRegisAfectados = 1;
            }
            catch (Exception e) { numRegisAfectados = 0; throw e; }

            return numRegisAfectados;
        }
    }
}