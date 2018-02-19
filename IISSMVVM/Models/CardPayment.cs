using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISSMVVM.Models
{
    public class CardPayment
    {
        public Guid id { get; set; }
        public Guid CPSaleNumber { get; set; }
        public string BankName { get; set; }
        public float CardNumber { get; set; }
        public string ExpirationDate { get; set; }
        public string CardHolderName { get; set; }
        public int IdC { get; set; }
    }
}
