using System;

namespace Model
{
    public class TaskUser : BaseEntity
    {
        public TaskUser()
        {
            DateCreated = DateTime.Now;
        }

        public string UserId { get; set; }
    }
}
