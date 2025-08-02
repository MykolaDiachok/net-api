namespace net_api.Types;

public class Category
{
    public string? Id { get; set; } = Guid.NewGuid().ToString();
    public required string Name { get; set; } 
}