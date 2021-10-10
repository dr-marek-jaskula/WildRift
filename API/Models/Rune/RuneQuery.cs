namespace WildRiftWebAPI
{
    public record RuneQuery
    (
        string SearchPhrase,
        int PageNumber,
        int PageSize,
        string SortBy,
        SortDirection SortDirection
    );
}