using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WizmeTest.Helpers
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public bool HasPrevious => (CurrentPage > 1);
        public bool HasNext => (CurrentPage < TotalPages);

        public List<T> Items;

        public PagedList(IQueryable<T> source, int pageNumber, int pageSize)
        {
            TotalCount = source.Count();
            Items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            PageSize = pageSize;
            CurrentPage = pageNumber;
            TotalPages = (int)Math.Ceiling(TotalCount / (double)pageSize);
        }

    }
}
