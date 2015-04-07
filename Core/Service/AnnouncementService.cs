using System;
using System.Data.Entity;
using System.Linq;
using Core.Caching;
using Core.Repository;
using Core.Service.CrudService;
using Model;

namespace Core.Service
{
    public interface IAnnouncementService : ICrudService<Announcement>
    {
        Announcement GetAnnouncement(Guid id, bool nocache = false);
        IQueryable<Announcement> GetAnnouncements();
        int AnnouncementsCount();
        IQueryable<Announcement> GetPublishedAnnouncements();

        IQueryable<Announcement> Search(string query);
    }
     
    public class AnnouncementService : BaseCrudService<Announcement>,IAnnouncementService
    {
        private readonly IRepository<Announcement> _repository;
        private readonly ICacheManager<Guid, Announcement> _cache; 
        public AnnouncementService(
            IRepository<Announcement> repository, 
            ICacheManager<Guid, Announcement> cache,
            UoW uow
            ) : base(repository, cache, uow)
        {
            _repository = repository;
            _cache = cache;
        }

        private Announcement GetAnnouncementInstance(Guid id)
        {
            Announcement announcement = _repository.Collection.Include(a => a.Attachments).FirstOrDefault(u => u.Id == id);

            return announcement;
        }

        public Announcement GetAnnouncement(Guid id, bool nocache= false)
        {
            Announcement announcement;

            if (nocache)
            {
                announcement = GetAnnouncementInstance(id);

                return announcement;
            }

            announcement = _cache.Get(id);

            if (announcement == null)
            {
                announcement = GetAnnouncementInstance(id);

                if (announcement == null)
                {
                    return null;
                }

                _cache.Set(id, announcement);
            }
             
            return announcement;
        }

        public IQueryable<Announcement> GetAnnouncements()
        {
            var announcements = _repository.CollectionUntracked.OrderByDescending(x => x.PublishDate);

            return announcements;
        }

        public int AnnouncementsCount()
        {
            int count = _repository.CollectionUntracked.Count();

            return count;
        }

        public IQueryable<Announcement> GetPublishedAnnouncements()
        {
            var announcements = _repository.CollectionUntracked.Where(x=> DateTime.Now > x.PublishDate & DateTime.Now < x.EndDate).OrderByDescending(x => x.PublishDate);

            return announcements;
        }

        public IQueryable<Announcement> Search(string query)
        {
            var announcements = _repository.CollectionUntracked.Where(x => x.Title.Contains(query) || x.Description.Contains(query)).OrderByDescending(x => x.Id);

            return announcements;
        }
    }
}
