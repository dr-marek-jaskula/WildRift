namespace WildRiftWebAPI
{
    public record CreateChampionPassiveDto
    (
        string Id,
        string Name,
        string Description,
        string Image_full,
        string Image_sprite,
        string Image_group,
        int Image_x,
        int Image_y,
        int Image_w,
        int Image_h
    );
}