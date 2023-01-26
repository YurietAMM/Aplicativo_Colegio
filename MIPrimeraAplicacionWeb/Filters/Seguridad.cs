using MIPrimeraAplicacionWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MIPrimeraAplicacionWeb.Filters
{
    public class Seguridad : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var Usuario = HttpContext.Current.Session["IdUsuario"];
            List<string> CONTROLADORES = Variable.controladores.Select(p => p.ToUpper()).ToList();
            string nombreControlador = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;

            if(Usuario == null /*|| !CONTROLADORES.Contains(nombreControlador)*/)
            {
                filterContext.Result = new RedirectResult("~/Login/Index");
            }
            base.OnActionExecuting(filterContext);
        }
    }
}