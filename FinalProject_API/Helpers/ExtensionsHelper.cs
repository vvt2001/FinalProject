namespace FinalProject_API.Helpers
{
    public static class ExtensionsHelper
    {
        public static IQueryable<T> Paged<T>(this IQueryable<T> query, int pagenumber, int pagesize)
        {
            if (pagenumber < 1) pagenumber = 1;
            return query.Skip((pagenumber - 1) * pagesize).Take(pagesize);
        }
    }
}
