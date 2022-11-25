namespace DoliteTemplate.Domain.DTOs;

public class DeviceReadDto
{
    public Guid Id { get; set; }
    public string Type { get; set; } = null!;
    public string Name { get; set; } = null!;
}