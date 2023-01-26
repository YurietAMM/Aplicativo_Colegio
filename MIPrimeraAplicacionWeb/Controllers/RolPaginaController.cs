using MIPrimeraAplicacionWeb.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace MIPrimeraAplicacionWeb.Controllers
{
    [Seguridad]
    public class RolPaginaController : Controller
    {
        // GET: RolPagina
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ListarRol()
        {
            using (PruebaDataContext bd = new PruebaDataContext())
            {
                var lista = bd.Rol.Where(p => p.BHABILITADO.Equals(1)).Select(p => new
                {
                    p.IIDROL,
                    p.NOMBRE,
                    p.DESCRIPCION
                }).ToList();

                return Json(lista, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult ListarPaginas()
        {
            using (PruebaDataContext bd = new PruebaDataContext())
            {
                var lista = bd.Pagina.Where(p => p.BHABILITADO.Equals(1)).Select(p => new
                {
                    p.IIDPAGINA,
                    p.MENSAJE,
                    p.BHABILITADO
                }).ToList();

                return Json(lista, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult RecuperarDatos(int id)
        {
            using (PruebaDataContext bd = new PruebaDataContext())
            {
                var rol = bd.Rol.Where(p => p.IIDROL.Equals(id))
                    .Select(p => new
                    {
                        p.IIDROL,
                        p.NOMBRE,
                        p.DESCRIPCION
                    }).First();

                return Json(rol, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult RecuperarDatosCheckBox(int id)
        {
            using (PruebaDataContext bd = new PruebaDataContext())
            {
                var lista = bd.RolPagina.Where(p => p.IIDROL.Equals(id))
                    .Select(p => new
                    {
                        p.IIDROL,
                        p.IIDPAGINA,
                        p.BHABILITADO
                    }).ToList();

                return Json(lista, JsonRequestBehavior.AllowGet);
            }
        }

        public int GuardarDatos(Rol rolsito, string valorEnviar)
        {
            int numRegisAfectados = 0;

            try
            {
                using (PruebaDataContext bd = new PruebaDataContext())
                {
                    using(var transacion = new TransactionScope())
                    {
                        if (rolsito.IIDROL.Equals(0))
                        {
                            //nuevo
                            bd.Rol.InsertOnSubmit(rolsito);
                            bd.SubmitChanges();

                            string[] codigos = valorEnviar.Split('$');
                            for(int i = 0; i  < codigos.Length; i++)
                            {
                                RolPagina obj = new RolPagina();
                                obj.IIDROL = rolsito.IIDROL;
                                obj.IIDPAGINA = int.Parse(codigos[i]);
                                obj.BHABILITADO = 1;
                                bd.RolPagina.InsertOnSubmit(obj);
                            }
                            bd.SubmitChanges();
                            transacion.Complete();
                            numRegisAfectados = 1;
                        }
                        else
                        {
                            //editar
                            Rol objRol = bd.Rol.Where(p => p.IIDROL.Equals(rolsito.IIDROL)).First();
                            objRol.NOMBRE = rolsito.NOMBRE;
                            objRol.DESCRIPCION = rolsito.DESCRIPCION;

                            var lista = bd.RolPagina.Where(p => p.IIDROL.Equals(rolsito.IIDROL)).ToList();
                            foreach(RolPagina rP in lista)
                            {
                                rP.BHABILITADO = 0;
                            }

                            string[] codigos = valorEnviar.Split('$');
                            for (int i = 0; i < codigos.Length; i++)
                            {
                                int cantidad = bd.RolPagina.Where(p => p.IIDROL.Equals(rolsito.IIDROL)
                                && p.IIDPAGINA.Equals(int.Parse(codigos[i]))).Count();
                                if(cantidad == 0)
                                {
                                    RolPagina obj = new RolPagina();
                                    obj.IIDROL = rolsito.IIDROL;
                                    obj.IIDPAGINA = int.Parse(codigos[i]);
                                    obj.BHABILITADO = 1;
                                    bd.RolPagina.InsertOnSubmit(obj);
                                }else
                                {
                                    RolPagina orp = bd.RolPagina.Where(p => p.IIDROL.Equals(rolsito.IIDROL)
                                    && p.IIDPAGINA.Equals(int.Parse(codigos[i]))).First();

                                    orp.BHABILITADO = 1;
                                }
                            }
                            bd.SubmitChanges();
                            transacion.Complete();
                            numRegisAfectados = 1;
                        }
                    }
                }
            }
            catch(Exception ex) { numRegisAfectados = 0; throw ex; }
            
            return numRegisAfectados;
        }
    }
}