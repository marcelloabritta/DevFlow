namespace DevFlow.Application.Common;

public static class Pagination
{
    private const int MaxPageSize = 50;
    private const int FirstPage = 1;
    private const int DefaultPageSize = 10;
    public static (int Page, int PageSize) Normalize(int page, int pageSize)
    {
        if (page < FirstPage)
        {
            page = FirstPage;
        }

        if (pageSize < 1)
        {
            pageSize = DefaultPageSize;
        }

        if (pageSize > MaxPageSize)
        {
            pageSize = MaxPageSize;
        }

        return (page, pageSize);
    }
}
