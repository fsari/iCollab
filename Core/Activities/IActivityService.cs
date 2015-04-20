using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Model.Activity;

namespace Core.Activities
{
    public interface IActivityService
    {
        void HandleActivity(ApplicationUser user, Verb verb, object obj);
    }
}
