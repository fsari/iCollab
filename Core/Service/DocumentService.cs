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

    public interface IDocumentService : ICrudService<Document>
    {
        Document GetDocument(Guid id, bool nocache = false);
        IQueryable<Document> GetDocuments();
        IQueryable<Document> UserDocuments(string username); 
        IQueryable<Document> Search(string query); 
    }


    public class DocumentService : BaseCrudService<Document>,IDocumentService
    {
        private readonly IRepository<Document> _repository; 
        public DocumentService
            (
                IRepository<Document> repository,
                Func<string, ICacheManager> cache 
            ): base(repository, cache)
            {
                _repository = repository; 
            }

        private Document GetDocumentInstance(Guid id)
        {
            Document document = _repository.AsQueryable()
                                .Include(o=>o.Owner)
                                .Include(p=>p.Project)
                                .Include(u=>u.Project.ProjectUsers)
                                .Include(a => a.Attachments)
                                .Include(c => c.ContentPages)
                                .FirstOrDefault(x => x.Id == id); 
            return document;
        }

        public Document GetDocument(Guid id, bool nocache = false)
        {
            Document document;

            if (nocache)
            {
                document = GetDocumentInstance(id);
                return document;
            }

            document = Cache.Get(id.ToString(), () => GetDocumentInstance(id));

            return document;
        }

        public IQueryable<Document> GetDocuments()
        {
            var documents = _repository.AsQueryable()
                                       .Where(m => m.IsDeleted == false)
                                       .OrderByDescending(t => t.DateCreated);

            return documents;
        }

        public IQueryable<Document> UserDocuments(string userId)
        {
            var documents = _repository.AsQueryable()
                                        .Include(o=>o.Owner)
                                        .Include(p=>p.Project)
                                        .Include(u=>u.Project.ProjectUsers)
                                        .Where(m => m.IsDeleted == false)
                                        .Where(x => x.Owner.Id == userId || x.IsPublic || x.Project.ProjectUsers.Any(c => c.UserId == userId))
                                        .Where(x=> (x.Project == null  || x.Project.IsDeleted == false))
                                        .OrderByDescending(x => x.DateCreated);

            return documents;
        }

        public IQueryable<Document> Search(string query)
        {
            var documents = _repository.AsQueryable().Where(m => m.IsDeleted == false).Where(x => x.Title.Contains(query) || x.Description.Contains(query)).OrderByDescending(x => x.DateCreated);

            return documents;
        }
         
    }
}