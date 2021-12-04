namespace WildRiftWebAPI;

public class User
{
    public string Username { get; set; }
    public string Email { get; set; }
    public DateTime? Create_time { get; set; }
    public string Password_hash { get; set; }
    public int Role_id { get; set; }
    public virtual Role Role { get; set; }
}
