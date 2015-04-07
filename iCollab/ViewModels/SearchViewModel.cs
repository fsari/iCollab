using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Model;
using PagedList;

namespace iCollab.ViewModels
{
    public class SearchViewModel<T> where T: BaseEntity
    {
        public string Query { set; get; }
        public int Page { set; get; }
        public IPagedList<T> Results { set; get; }

        public int TotalItemCount { set; get; }
    }
}