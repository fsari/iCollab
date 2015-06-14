using System;
using System.Data.Entity;
using System.Linq;
using Core.Caching;
using Core.Repository;
using Core.Service.CrudService;
using Model;
using SharpRepository.Repository;

namespace Core.Service
{
    public interface IMeetingService : ICrudService<Meeting>
    {
        Meeting GetMeeting(Guid id, bool nocache = false);
        IQueryable<Meeting> GetMeetings();
        int MeetingsCount();
        IQueryable<Meeting> GetUserMeetings(string userId);
        IQueryable<Meeting> Search(string query);
    }

    public class MeetingService : BaseCrudService<Meeting>, IMeetingService
    {
        private readonly IRepository<Meeting> _repository; 

        public MeetingService(IRepository<Meeting> repository, Func<string, ICacheManager> cache)
            : base(repository, cache)
        {
            _repository = repository; 
        }

        private Meeting GetMeetingInstance(Guid id)
        {
            Meeting meeting = _repository.AsQueryable().Include(o=>o.Owner).Include(p => p.Project).Include(p=>p.Project.ProjectUsers).Include(a => a.Attachments).FirstOrDefault(i => i.Id == id);

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
            var meetings = _repository.AsQueryable().AsNoTracking().Where(m => m.IsDeleted == false).OrderByDescending(m => m.Id);

            return meetings;
        }

        public int MeetingsCount()
        {
            int count = _repository.AsQueryable().AsNoTracking().Count();

            return count;
        }

        public IQueryable<Meeting> GetUserMeetings(string userId)
        {
            var meetings = _repository.AsQueryable().Include(x=>x.Project.ProjectUsers).Include(o=>o.Owner).AsNoTracking()
                            .Where(m => m.IsDeleted == false).Where(x => x.OwnerId == userId || x.IsPublic || x.Project.ProjectUsers.Any(c => c.UserId == userId)).OrderByDescending(x => x.DateCreated);

            return meetings;
        }

        public IQueryable<Meeting> Search(string query)
        {
            var meetings = _repository.AsQueryable().AsNoTracking().Where(x => x.Title.Contains(query) || x.Description.Contains(query)).OrderByDescending(x => x.DateCreated);

            return meetings;
        }
    }
}