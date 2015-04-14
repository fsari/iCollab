﻿using System; 
using Kendo.Mvc.UI;

namespace iCollab.ViewModels.Gantt
{
    public class GanttTaskViewModel : IGanttTask
    { 
        public Guid TaskID { get; set; }
        public Guid? ParentID { get; set; }

        public string Title { get; set; }

        private DateTime start;
        public DateTime Start
        {
            get { return start; }
            set { start = value.ToUniversalTime(); }
        }

        private DateTime end;
        public DateTime End
        {
            get { return end; }
            set { end = value.ToUniversalTime(); }
        }

        public bool Summary { get; set; }
        public bool Expanded { get; set; }
        public decimal PercentComplete { get; set; }
        public int OrderId { get; set; }
    }

    public class DependencyViewModel : IGanttDependency
    {
        public Guid DependencyID { get; set; }

        public Guid PredecessorID { get; set; }
        public Guid SuccessorID { get; set; }
        public DependencyType Type { get; set; }
    }

}