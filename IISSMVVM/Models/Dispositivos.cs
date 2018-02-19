using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISSMVVM.Models
{
    public class Dispositivos
    {
        public Guid Id { get; set; }
        public int IdC { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }
        public string isActive { get; set; }
        public string IP { get; set; }
        public string Guardia { get; set; }
        public string Sector { get; set; }
        public int IdDisp { get; set; }
        public int Checksum { get; set; }
    }
}
