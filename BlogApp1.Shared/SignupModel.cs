using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogApp1.Shared
{
    public class SignupModel
    {

        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public List<string> Role { get; set; }
    }
}
