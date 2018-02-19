using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISSMVVM.Models
{
    public class Payment
    {
        public Guid id { get; set; }
        public Guid CPSaleNumber { get; set; }
        public string BankName { get; set; }
        public float CardNumber { get; set; }
        public string ExpirationDate { get; set; }
        public string CardHolderName { get; set; }
        public Guid ASaleNumber { get; set; }
        public string StreetName { get; set; }
        public float HouseNumber { get; set; }
        public string Country { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public string Image { get; set; }
    }
}
