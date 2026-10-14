namespace DevFlow.Application.Common;

public record PagedResult<T>(IEnumerable<T> Items, int Page, int PageSize, int TotalItems)
{
    public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
}
