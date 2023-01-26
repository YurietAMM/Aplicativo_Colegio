using MIPrimeraAplicacionWeb.Filters;
using MIPrimeraAplicacionWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MIPrimeraAplicacionWeb.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        public int Ingresar(string username, string password)
        {
            int respuesta = 0;
            try
            {
                using (PruebaDataContext bd = new PruebaDataContext()) 
                {
                    SHA256Managed sha = new SHA256Managed();
                    byte[] dataNC = Encoding.Default.GetBytes(password);
                    byte[] dataC = sha.ComputeHash(dataNC);
                    string contraC = BitConverter.ToString(dataC).Replace("-", "");

                    respuesta = bd.Usuario.Where(p => p.NOMBREUSUARIO.Equals(username) && p.CONTRA.Equals(contraC)).Count();
                    
                    if(respuesta == 1)
                    {
                        int idUsuario = bd.Usuario.Where(p => p.NOMBREUSUARIO.Equals(username) && p.CONTRA.Equals(contraC)).First().IIDUSUARIO;
                        Session["IdUsuario"] = idUsuario;

                        var roles = from usuario in bd.Usuario
                                    join rol in bd.Rol
                                    on usuario.IIDROL equals rol.IIDROL
                                    join rolpagina in bd.RolPagina
                                    on rol.IIDROL equals rolpagina.IIDROL
                                    join pagina in bd.Pagina
                                    on rolpagina.IIDPAGINA equals pagina.IIDPAGINA
                                    where usuario.BHABILITADO == 1 && rolpagina.BHABILITADO == 1
                                    && usuario.IIDUSUARIO == idUsuario
                                    select new
                                    {
                                        acciones = pagina.ACCION,
                                        controladores = pagina.CONTROLADOR,
                                        mensaje = pagina.MENSAJE
                                    };
                        Variable.acciones = new List<string>();
                        Variable.controladores = new List<string>();
                        Variable.mensaje = new List<string>();

                        foreach(var item in roles)
                        {
                            Variable.acciones.Add(item.acciones);
                            Variable.controladores.Add(item.controladores);
                            Variable.mensaje.Add(item.mensaje);
                        }
                    }
                }

            }
            catch(Exception ex) { respuesta = 0; throw ex; }

            return respuesta;
        }

        public ActionResult Cerrar()
        {
            return RedirectToAction("Index");
        }
    }
}