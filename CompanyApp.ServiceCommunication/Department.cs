using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CompanyApp.ServiceCommunication.CompanyService
{
    public partial class Department : ICloneable
    {

        public Department() { }
        public Department(int id, string title, string location)
        {
            ID = id;
            Title = title;
            Location = location;
        }

        public object Clone() => MemberwiseClone();
    }
}
