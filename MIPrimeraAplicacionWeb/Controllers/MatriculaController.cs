using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using System.Security.Cryptography;

namespace MIPrimeraAplicacionWeb.Controllers
{
    public class MatriculaController : Controller
    {
        // GET: Matricula
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult ListarPeriodo()
        {
            PruebaDataContext bd = new PruebaDataContext();

            var lista = (bd.Periodo.Where(p => p.BHABILITADO.Equals(1))
                .Select(p => new {
                    IID = p.IIDPERIODO,
                    p.NOMBRE
                }).ToList());

            return Json(lista, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListarGradoSeccion()
        {
            PruebaDataContext bd = new PruebaDataContext();

            var lista = from gs in bd.GradoSeccion
                        join grado in bd.Grado
                        on gs.IIDGRADO equals grado.IIDGRADO
                        join seccion in bd.Seccion
                        on gs.IIDSECCION equals seccion.IIDSECCION
                        select new
                        {
                            gs.IID,
                            NOMBRE = grado.NOMBRE + " - " + seccion.NOMBRE
                        };

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListarAlumnos()
        {
            PruebaDataContext bd = new PruebaDataContext();

            var lista = (bd.Alumno.Where(p => p.BHABILITADO.Equals(1))
                .Select(p => new {
                    IID = p.IIDALUMNO,
                    NOMBRE = p.NOMBRE + " " + p.APPATERNO + " " + p.APMATERNO
                }).ToList());

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public int GuardarDatos(Matricula gsa, int IIDGRADOSECCION, string valorEnviar)
        {
            PruebaDataContext bd = new PruebaDataContext();
            int numRegisAfectados = 0;

            GradoSeccion obj = bd.GradoSeccion.Where(p => p.IID.Equals(IIDGRADOSECCION)).First();

            int IIDGrado = (int) obj.IIDGRADO;
            int IIDSeccion = (int) obj.IIDSECCION;

            gsa.IIDGRADO = IIDGrado;
            gsa.IIDSECCION = IIDSeccion;
            gsa.FECHA = DateTime.Now;

            try
            {
                
                using(var transaccion = new TransactionScope())
                {
                    if (gsa.IIDMATRICULA.Equals(0))
                    {
                        int numVeces = bd.Matricula.Where(p => p.IIDALUMNO.Equals(gsa.IIDALUMNO) 
                        && p.IIDPERIODO.Equals(gsa.IIDPERIODO) && p.IIDGRADO.Equals(gsa.IIDGRADO)).Count();

                        if(numVeces == 0)
                        {
                            bd.Matricula.InsertOnSubmit(gsa);
                            bd.SubmitChanges();

                            int idMG = gsa.IIDMATRICULA;

                            var listaCursos = bd.PeriodoGradoCurso.Where(p => p.IIDPERIODO.Equals(gsa.IIDPERIODO)
                            && p.IIDGRADO.Equals(IIDGrado)).Select(p => p.IIDCURSO);

                            foreach (var curso in listaCursos)
                            {
                                DetalleMatricula dm = new DetalleMatricula();
                                dm.IIDMATRICULA = idMG;
                                dm.IIDCURSO = (int)curso;
                                dm.NOTA1 = 0;
                                dm.NOTA2 = 0;
                                dm.NOTA3 = 0;
                                dm.NOTA4 = 0;
                                dm.PROMEDIO = 0;
                                dm.bhabilitado = 1;
                                bd.DetalleMatricula.InsertOnSubmit(dm);
                            }
                            bd.SubmitChanges();
                            transaccion.Complete();
                            numRegisAfectados = 1;
                        }
                        else
                        {
                            numRegisAfectados = -1;
                        }
                    }
                    else
                    {
                        int numVeces = bd.Matricula.Where(p => p.IIDALUMNO.Equals(gsa.IIDALUMNO)
                        && p.IIDPERIODO.Equals(gsa.IIDPERIODO) && p.IIDGRADO.Equals(gsa.IIDGRADO)
                        && !p.IIDMATRICULA.Equals(gsa.IIDMATRICULA)).Count();

                        if(numVeces == 0)
                        {
                            var matricula = bd.Matricula.Where(p => p.IIDMATRICULA.Equals(gsa.IIDMATRICULA)).First();
                            matricula.IIDPERIODO = gsa.IIDPERIODO;
                            matricula.IIDGRADO = gsa.IIDGRADO;
                            matricula.IIDSECCION = gsa.IIDSECCION;
                            matricula.IIDALUMNO = gsa.IIDALUMNO;

                            var dm = bd.DetalleMatricula.Where(p => p.IIDMATRICULA.Equals(gsa.IIDMATRICULA));
                            foreach (DetalleMatricula dtmt in dm)
                            {
                                dtmt.bhabilitado = 0;
                            }
                            // 1$5$7 => [1, 5, 7]
                            string[] valores = valorEnviar.Split('$');
                            for (int i = 0; i < valores.Length; i++)
                            {
                                DetalleMatricula detalleMatri = bd.DetalleMatricula
                                    .Where(p => p.IIDMATRICULA.Equals(gsa.IIDMATRICULA)
                                    && p.IIDCURSO == int.Parse(valores[i])).First();

                                detalleMatri.bhabilitado = 1;
                            }

                            bd.SubmitChanges();
                            transaccion.Complete();
                            numRegisAfectados = 1;
                        }
                        else
                        {
                            numRegisAfectados = -1;
                        }
                    }
                }

            }
            catch (Exception ex) { numRegisAfectados = 0; throw ex; }

            return numRegisAfectados;
        }

        public JsonResult ListarMatricula()
        {
            PruebaDataContext bd = new PruebaDataContext();

            var lista = from matricula in bd.Matricula
                        join periodo in bd.Periodo
                        on matricula.IIDPERIODO equals periodo.IIDPERIODO
                        join grado in bd.Grado
                        on matricula.IIDGRADO equals grado.IIDGRADO
                        join seccion in bd.Seccion
                        on matricula.IIDSECCION equals seccion.IIDSECCION
                        join alumno in bd.Alumno
                        on matricula.IIDALUMNO equals alumno.IIDALUMNO
                        where matricula.BHABILITADO == 1
                        select new
                        {
                            IID = matricula.IIDMATRICULA,
                            nombrePeriodo = periodo.NOMBRE,
                            nombreGrado = grado.NOMBRE,
                            nombreSeccion = seccion.NOMBRE,
                            nombreAlumno = alumno.NOMBRE + " " + alumno.APPATERNO + " " + alumno.APMATERNO
                        };

            return Json(lista, JsonRequestBehavior.AllowGet);
        }

        public int Eliminar(int id)
        {
            int numRegisAfectados = 0;
            PruebaDataContext bd = new PruebaDataContext();

            try
            {
                using(var transaccion = new TransactionScope())
                {
                    Matricula objMtricula = bd.Matricula.Where(p => p.IIDMATRICULA.Equals(id)).First();
                    objMtricula.BHABILITADO = 0;
                    bd.SubmitChanges();

                    var listaDetalleMatricula = bd.DetalleMatricula.Where(p => p.IIDMATRICULA.Equals(id));
                    
                    foreach(DetalleMatricula objDM in listaDetalleMatricula)
                    {
                        objDM.bhabilitado = 0;
                    }

                    bd.SubmitChanges();
                    transaccion.Complete();
                    numRegisAfectados = 1;
                }
            }
            catch(Exception e)
            {
                numRegisAfectados = 0;
                throw e;
            }
            return numRegisAfectados;
        }

        public JsonResult RecuperarDatos(int id)
        {
            using (PruebaDataContext bd = new PruebaDataContext())
            {
                var obj = bd.Matricula.Where(p => p.IIDMATRICULA.Equals(id)).Select(p => new
                {
                    IIDMATRICULA = (int) p.IIDMATRICULA,
                    IIDPERIODO = (int) p.IIDPERIODO,
                    IIDSECCION = (int) p.IIDSECCION,
                    IIDALUMNO = (int) p.IIDALUMNO
                }).First();

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult RecuperarCursos(int id)
        {
            using (PruebaDataContext bd = new PruebaDataContext())
            {
                var obj = (from detalleMatricula in bd.DetalleMatricula
                          join curso in bd.Curso
                          on detalleMatricula.IIDCURSO equals curso.IIDCURSO
                          where detalleMatricula.IIDMATRICULA == id
                          select new
                          {
                              detalleMatricula.IIDMATRICULA,
                              curso.IIDCURSO,
                              curso.NOMBRE,
                              detalleMatricula.bhabilitado
                          }).ToList();

                return Json(obj, JsonRequestBehavior.AllowGet);
            }
        }
    }
}