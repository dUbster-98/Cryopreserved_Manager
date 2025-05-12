using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryopreserved_Manager.Models
{
    public class CellInfo
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public string? Location { get; set; }
        public string? ReceiptDay { get; set; }
        public string? BarcodeText { get; set; }
        public string? State { get; set; }
    }
}
