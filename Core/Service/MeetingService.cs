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
        private readonly ICacheManager<Guid, Meeting> _cache;

        public MeetingService(IRepository<Meeting> repository, ICacheManager<Guid, Meeting> cache, UoW uow)
            : base(repository, cache, uow)
        {
            _repository = repository;
            _cache = cache;
        }

        private Meeting GetMeetingInstance(Guid id)
        {
            Meeting meeting = _repository.Collection.Include(a => a.Attachments).FirstOrDefault(i => i.Id == id);

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

            meeting = _cache.Get(id);

            if (meeting == null)
            {
                meeting = GetMeetingInstance(id);

                if (meeting == null)
                {
                    return null;
                }

                _cache.Set(id, meeting);
            }

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