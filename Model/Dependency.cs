using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Dependency : BaseEntity
    { 
        public Guid DependencyId { get; set; } 
        public Guid PredecessorId { get; set; }
        public Guid SuccessorId { get; set; }

    }
}
