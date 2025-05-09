using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cryopreserved_Manager.Models
{
    public class UserInfo
    {
        public string Key { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserID { get; set; }
        public string Status { get; set; }
        public string Role { get; set; }
        public string Department { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool NeedResetPW { get; set; }
        public string Password_Changed_Date { get; set; }
        public string Password_retry { get; set; }
        public bool AdminAlarm { get; set; }
    }
}
