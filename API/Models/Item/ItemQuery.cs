namespace WildRiftWebAPI
{
    public record ItemQuery
    (
        string SearchPhrase,
        int PageNumber,
        int PageSize,
        string SortBy,
        SortDirection SortDirection
    );
}