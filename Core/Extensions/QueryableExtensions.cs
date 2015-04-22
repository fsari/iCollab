using System.Collections.Generic;
using System.Linq;
using Model;

namespace Core.Extensions
{
    public static  class QueryableExtensions
    { 
        public static T GetNext<T>(this IQueryable<T> list, T current) where T :BaseEntity
        {  
            return list.FirstOrDefault(elem => elem.DateCreated > current.DateCreated);
        }

        public static T GetPrevious<T>(this IQueryable<T> list, T current) where T : BaseEntity
        {  
            return list.FirstOrDefault(elem => elem.DateCreated < current.DateCreated);
        }
    } 
}
