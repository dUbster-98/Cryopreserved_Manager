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
        public string? Quantity { get; set; }
        public string? Location { get; set; }
        public DateTime? ReceiptDate { get; set; }
        public string? State { get; set; }
    }
}
