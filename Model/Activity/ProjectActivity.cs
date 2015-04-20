using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Activity
{
    public class ProjectActivity : Activity
    { 


        public Project Project { set; get; }
        public Guid ProjectId { set; get; }

    }
}
