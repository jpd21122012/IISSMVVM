using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IISSMVVM.Models
{
    public class Bills
    {
        public Guid id { get; set; }
        public DateTimeOffset Date { get; set; }
        public string RFC { get; set; }
        public string Name { get; set; }
        public string PaymentType { get; set; }
        public float Quantity { get; set; }
        public string ProductName { get; set; }
        public string SubTotal { get; set; }
        public float Tax { get; set; }
        public float Total { get; set; }
        public int IdC { get; set; }
        public string State { get; set; }
        public string Type { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
