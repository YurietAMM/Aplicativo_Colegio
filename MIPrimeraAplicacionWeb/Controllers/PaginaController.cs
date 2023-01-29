using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace MIPrimeraAplicacionWeb.Controllers
{
    public class PaginaController : Controller
    {
        // GET: Pagina
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ListarPagina()
        {
            PruebaDataContext bd = new PruebaDataContext();

            var lista = bd.Pagina.Where(p => p.BHABILITADO.Equals(1))
                .Select(p => new
                {
                    p.IIDPAGINA,
                    p.MENSAJE,
                    p.CONTROLADOR,
                    p.ACCION
                }).ToList();

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecuperarDatos(int id)
        {
            PruebaDataContext bd = new PruebaDataContext();

            var obj = bd.Pagina.Where(p => p.IIDPAGINA.Equals(id))
                .Select(p => new
                {
                    p.IIDPAGINA,
                    p.MENSAJE,
                    p.CONTROLADOR,
                    p.ACCION
                }).First();

            return Json(obj, JsonRequestBehavior.AllowGet);
        }

        public int GuardarDatos(Pagina pag)
        {
            PruebaDataContext bd = new PruebaDataContext();
            int numRegisAfectados = 0;

            try
            {
                if(pag.IIDPAGINA == 0)
                {
                    int numVeces = bd.Pagina.Where(p => p.MENSAJE.Equals(pag.MENSAJE)).Count();

                    if (numVeces == 0)
                    {
                        bd.Pagina.InsertOnSubmit(pag);
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
                    int numVeces = bd.Pagina.Where(p => p.MENSAJE.Equals(pag.MENSAJE) && !p.IIDPAGINA.Equals(pag.IIDPAGINA)).Count();

                    if (numVeces == 0)
                    {
                        Pagina obj = bd.Pagina.Where(p => p.IIDPAGINA.Equals(pag.IIDPAGINA)).First();

                        obj.MENSAJE = pag.MENSAJE;
                        obj.CONTROLADOR = pag.CONTROLADOR;
                        obj.ACCION = pag.ACCION;
                        bd.SubmitChanges();
                        numRegisAfectados = 1;
                    }
                    else
                    {
                        numRegisAfectados = -1;
                    }
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
                Pagina obj = bd.Pagina.Where(p => p.IIDPAGINA.Equals(id)).First();

                obj.BHABILITADO = 0;

                bd.SubmitChanges();
                numRegisAfectados = 1;
            }
            catch(Exception ex) { numRegisAfectados=0; throw ex; }

            return numRegisAfectados;
        }
    }
}
