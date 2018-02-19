using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISSMVVM.Models
{
    public class Empresas
    {
        public Guid Id { get; set; }
        public int IdC { get; set; }
        public string Nombre { get; set; }
        public string FechaInicio { get; set; }
        public string FechaFin { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Workspace { get; set; }
        public string FaceListId { get; set; }
        public string Estado { get; set; }
        //Add in Android
        public string Image { get; set; }
    }
}
