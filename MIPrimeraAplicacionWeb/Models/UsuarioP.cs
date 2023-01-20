using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MIPrimeraAplicacionWeb.Models
{
    public class UsuarioP
    {
        public int IID { get; set; }
        public string NombreCompleto { get; set; }
        public string NombreUsuario { get; set; }
        public string Rol { get; set; }
        public string TipoEmpleado { get; set; }

    }
}