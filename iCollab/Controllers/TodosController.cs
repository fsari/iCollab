using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Web.Http;
using iCollab.Infra;
using iCollab.ViewModels;
using Microsoft.AspNet.Identity;
using Model;
using Model.TodoList;

namespace iCollab.Controllers
{
    [Authorize]
    public class TodosController : ApiController
    {
        private readonly DataContext _db = new DataContext(null);

        public IEnumerable<TodoDto> Get()
        {
            string username = User.Identity.GetUserName();

            ApplicationUser user = _db.Users.Include(t=>t.Todos).FirstOrDefault(x => x.UserName == username);

            if (user == null)
            {
                return null;
            }

            IEnumerable<TodoDto> todos = user.Todos.Select(x => new TodoDto
            {
                Completed = x.Completed,
                Title = x.Title
            });
             
            return todos;
        }

        public IHttpActionResult Post(TodoDto[] todoDto)
        {
            if (ModelState.IsValid)
            {  
                string username = User.Identity.GetUserName();

                ApplicationUser user = _db.Users.Include(t=>t.Todos).FirstOrDefault(u=>u.UserName == username);

                if (user == null)
                {
                    return BadRequest("kullanici yok.");
                }

                if (user.Todos == null)
                {
                    user.Todos = new Collection<Todo>();
                }

                user.Todos.Clear();

                foreach (TodoDto dto in todoDto)
                {
                    var todo = new Todo {Completed = dto.Completed, Title = dto.Title};

                    user.Todos.Add(todo);
                }

                _db.SaveChanges();

                return Ok(todoDto); 
            }

            return BadRequest("hata.");
        }
    }
}