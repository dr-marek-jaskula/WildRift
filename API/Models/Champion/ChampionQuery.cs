namespace WildRiftWebAPI;

public record ChampionQuery
(
    string SearchPhrase,
    int PageNumber,
    int PageSize,
    string SortBy,
    SortDirection SortDirection
);
