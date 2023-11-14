namespace BookManage.Models
{
    public class Pager
    {
        public int TotalItem { get; private set; }
        public int CurrentPage { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPage { get; private set; }
        public int StartPage { get; private set; }
        public int EndPage { get; private set;}


        public Pager() { }

        //public Pager(int totalItem, int page, int pageSize)
        //{
        //    int TotalPage = (int)Math.Ceiling((decimal)totalItem / (decimal)pageSize);
        //    int CurrentPage = CurrentPage 
        //}
    }
}
