using Model;
using Model.Activity;

namespace Core.Activities
{
    public interface IActivityService
    {
        void HandleActivity(ApplicationUser user, Verb verb, object obj);
    }
}
