using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Dependency : BaseEntity
    { 
         
        public Guid PredecessorId { get; set; }
        public Guid SuccessorId { get; set; }

        public DependencyType Type { set; get; }

    }

    public enum DependencyType
    {
        // Summary:
        //     The task cannot end before its predecessor task ends, although it may end
        //     later.
        FinishFinish = 0,
        //
        // Summary:
        //     The task cannot start before its predecessor task ends, although it may start
        //     later.
        FinishStart = 1,
        //
        // Summary:
        //     The task cannot end before its predecessor task starts, although it may end
        //     later.
        StartFinish = 2,
        //
        // Summary:
        //     The task cannot start until the predecessor tasks starts, although it may
        //     start later.
        StartStart = 3,
    }
}
