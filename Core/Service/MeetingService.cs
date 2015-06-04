using System;
using System.Data.Entity;
using System.Linq;
using Core.Caching;
using Core.Repository;
using Core.Service.CrudService;
using Model;

namespace Core.Service
{
    public interface IMeetingService : ICrudService<Meeting>
    {
        Meeting GetMeeting(Guid id, bool nocache = false);
        IQueryable<Meeting> GetMeetings();
        int MeetingsCount();
        IQueryable<Meeting> GetUserMeetings(string username);
        IQueryable<Meeting> Search(string query);
    }

    public class MeetingService : BaseCrudService<Meeting>, IMeetingService
    {
        private readonly IRepository<Meeting> _repository; 

        public MeetingService(IRepository<Meeting> repository, Func<string, ICacheManager> cache, UoW uow)
            : base(repository, cache, uow)
        {
            _repository = repository; 
        }

        private Meeting GetMeetingInstance(Guid id)
        {
            Meeting meeting = _repository.Collection.Include(p => p.Project).Include(a => a.Attachments).FirstOrDefault(i => i.Id == id);

            return meeting;
        }

        public Meeting GetMeeting(Guid id, bool nocachce = false)
        {
            Meeting meeting;

            if (nocachce)
            {
                meeting = GetMeetingInstance(id);

                return meeting;
            }

            meeting = Cache.Get(id.ToString(), () => GetMeetingInstance(id));
             
            return meeting;
        }

        public IQueryable<Meeting> GetMeetings()
        {
            var meetings = _repository.CollectionUntracked.Where(m => m.IsDeleted == false).OrderByDescending(m => m.Id);

            return meetings;
        }

        public int MeetingsCount()
        {
            int count = _repository.CollectionUntracked.Count();

            return count;
        }

        public IQueryable<Meeting> GetUserMeetings(string username)
        {
            var meetings = _repository.CollectionUntracked.Where(x => x.CreatedBy == username).OrderByDescending(x=>x.DateCreated);

            return meetings;
        }

        public IQueryable<Meeting> Search(string query)
        {
            var meetings = _repository.CollectionUntracked.Where(x => x.Title.Contains(query) || x.Description.Contains(query)).OrderByDescending(x => x.DateCreated);

            return meetings;
        }
    }
}