using System.ComponentModel.DataAnnotations;

namespace Tutorial6.Models.DTOs;

public class AddAnimalRequest
{
    [MinLength(3)]
    [MaxLength(200)]
    public string Name { get; set; }
    [MaxLength(200)]
    public string? Description { get; set; }
}