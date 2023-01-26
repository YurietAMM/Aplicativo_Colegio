using MIPrimeraAplicacionWeb.Filters;
using MIPrimeraAplicacionWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.Mvc;

namespace MIPrimeraAplicacionWeb.Controllers
{
    [Seguridad]
    public class UsuarioController : Controller
    {
        // GET: Usuario
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ListarRoles()
        {
            PruebaDataContext bd = new PruebaDataContext();

            var lista = bd.Rol.Where(p => p.BHABILITADO.Equals(1)).Select(p => new
            {
                IID = p.IIDROL,
                p.NOMBRE
            }).ToList();

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarPersonas()
        {
            List<Persona> lista = new List<Persona>();
            using (PruebaDataContext bd = new PruebaDataContext())
            {
                var listaAlumnos = (from item in bd.Alumno
                                   where item.bTieneUsuario.Equals(0)
                                   select new Persona
                                   {
                                       IID = item.IIDALUMNO,
                                       NOMBRE = item.NOMBRE + " " + item.APPATERNO + " " + item.APMATERNO + " (A)"
                                   }).ToList();
                lista.AddRange(listaAlumnos);
                var listaDocentes = (from item in bd.Docente
                                    where item.bTieneUsuario.Equals(0)
                                    select new Persona
                                    {
                                        IID = item.IIDDOCENTE,
                                        NOMBRE = item.NOMBRE + " " + item.APPATERNO + " " + item.APMATERNO + " (D)"
                                    }).ToList();
                lista.AddRange(listaDocentes);
                lista = lista.OrderBy(p => p.NOMBRE).ToList();

                return Json(lista, JsonRequestBehavior.AllowGet);
            }
        }

        public int GuardarDatos(Usuario usuarito, string NombrePersona)
        {
            int numRegisAfectados = 0;
            try
            {
                using (PruebaDataContext bd = new PruebaDataContext())
                {
                    using (var transaccion = new TransactionScope())
                    {
                        if (usuarito.IIDUSUARIO.Equals(0))
                        {
                            //Nuevo
                            //Ciframos la contraseña antes de guardarla en la bd
                            string clave = usuarito.CONTRA;
                            SHA256Managed sha = new SHA256Managed();
                            byte[] dataNC = Encoding.Default.GetBytes(clave);
                            byte[] dataC = sha.ComputeHash(dataNC);
                            usuarito.CONTRA = BitConverter.ToString(dataC).Replace("-", "");
                            usuarito.TIPOUSUARIO = char.Parse(NombrePersona.Substring(NombrePersona.Length - 2, 1));

                            bd.Usuario.InsertOnSubmit(usuarito);

                            if (usuarito.TIPOUSUARIO.Equals('D'))
                            {
                                //Registra Docente
                                Docente obj = bd.Docente.Where(p => p.IIDDOCENTE.Equals(usuarito.IID)).First();
                                obj.bTieneUsuario = 1;
                            }
                            else
                            {
                                //Registra Alumno
                                Alumno obj = bd.Alumno.Where(p => p.IIDALUMNO.Equals(usuarito.IID)).First();
                                obj.bTieneUsuario = 1;
                            }
                            bd.SubmitChanges();
                            transaccion.Complete();
                            numRegisAfectados = 1;
                        }
                        else
                        {
                            //Editar
                            Usuario us = bd.Usuario.Where(p => p.IIDUSUARIO.Equals(usuarito.IIDUSUARIO)).First();

                            us.IIDROL = usuarito.IIDROL;
                            us.CONTRA = usuarito.CONTRA;
                            bd.SubmitChanges();
                            transaccion.Complete();
                            numRegisAfectados = 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                numRegisAfectados = 0;
                throw ex;
            }
            return numRegisAfectados;
        }

        public JsonResult ListarUsuarios()
        {
            List<UsuarioP> lista = new List<UsuarioP>();
            using (PruebaDataContext bd = new PruebaDataContext())
            {
                List<UsuarioP> listaAlumnos = (from item in bd.Usuario
                                             join alumno in bd.Alumno
                                             on item.IID equals alumno.IIDALUMNO
                                             join rol in bd.Rol
                                             on item.IIDROL equals rol.IIDROL
                                             where item.BHABILITADO==1 && item.TIPOUSUARIO=='A'
                                             select new UsuarioP
                                             {
                                                 IID = item.IIDUSUARIO,
                                                 NombreCompleto = alumno.NOMBRE + " " + alumno.APPATERNO + " " + alumno.APMATERNO,
                                                 NombreUsuario = item.NOMBREUSUARIO,
                                                 Rol = rol.NOMBRE,
                                                 TipoEmpleado = "ALUMNO"
                                             }).ToList();

                lista.AddRange(listaAlumnos);

                List<UsuarioP> listaDocentes = (from item in bd.Usuario
                                               join docente in bd.Docente
                                               on item.IID equals docente.IIDDOCENTE
                                               join rol in bd.Rol
                                               on item.IIDROL equals rol.IIDROL
                                               where item.BHABILITADO == 1 && item.TIPOUSUARIO == 'D'
                                               select new UsuarioP
                                               {
                                                   IID = item.IIDUSUARIO,
                                                   NombreCompleto = docente.NOMBRE + " " + docente.APPATERNO + " " + docente.APMATERNO,
                                                   NombreUsuario = item.NOMBREUSUARIO,
                                                   Rol = rol.NOMBRE,
                                                   TipoEmpleado = "DOCENTE"
                                               }).ToList();

                lista.AddRange(listaDocentes);

                lista = lista.OrderBy(p => p.IID).ToList();
            }
            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult RecuperarDatos(int id)
        {
            using (PruebaDataContext bd = new PruebaDataContext())
            {
                var obj = bd.Usuario.Where(p => p.IIDUSUARIO.Equals(id))
                    .Select(p => new
                    {
                        p.IIDUSUARIO,
                        p.NOMBREUSUARIO,
                        p.CONTRA,
                        p.IID,
                        p.IIDROL
                    }).First();

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
        }
    }
}