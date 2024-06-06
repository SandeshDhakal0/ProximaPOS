namespace TheHighInnovation.POS.WEB.Model;

public class PagerDto
{
    public int TotalItems { get; set; }
    
    public int CurrentPage { get; set; }
    
    public int PageSize { get; set; }
    
    public int TotalPages { get; set; }
    
    public int StartPage { get; set; }
    
    public int EndPage { get; set; }

    public PagerDto()
    {
            
    }

    public PagerDto(int totalItems, int page, int pageSize)
    {
        var totalPages = (int)Math.Ceiling((decimal)totalItems / (decimal)pageSize);

        var startPage = page - 5;
        var endPage = page + 4;

        if (startPage <= 0)
        {
            endPage = endPage - (startPage - 1);
            startPage = 1;
        }

        if (endPage > totalPages)
        {
            endPage = totalPages;
            if (endPage > 10)
            {
                startPage = endPage - 9;
            }
        }

        TotalItems = totalItems;
        CurrentPage = page;
        PageSize = pageSize;
        TotalPages = totalPages;
        StartPage = startPage;
        EndPage = endPage;
    }
}