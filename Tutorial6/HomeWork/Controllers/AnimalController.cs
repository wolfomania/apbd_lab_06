using HomeWork.Models;
using HomeWork.Models.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace HomeWork.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnimalsController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public AnimalsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult GetAnimals(string orderBy = "Name")
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        string sqlCommand = $"SELECT * FROM Animal ORDER BY {orderBy} ASC";
        command.CommandText = sqlCommand;

        var reader = command.ExecuteReader();

        var animals = new List<Animal>();

        int idAnimalOrdinal = reader.GetOrdinal("IdAnimal");
        int nameOrdinal = reader.GetOrdinal("Name");
        int descriptionOrdinal = reader.GetOrdinal("Description");
        int categoryOrdinal = reader.GetOrdinal("Category");
        int areaOrdinal = reader.GetOrdinal("Area");

        while (reader.Read())
        {
            animals.Add(new Animal()
            {
                IdAnimal = reader.GetInt32(idAnimalOrdinal),
                Name = reader.GetString(nameOrdinal),
                Description = reader.GetString(descriptionOrdinal),
                Category = reader.GetString(categoryOrdinal),
                Area = reader.GetString(areaOrdinal)
            });
        }

        return Ok(animals);
    }

    [HttpPost]
    public IActionResult AddAnimal(AddAnimalRequest animal)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "INSERT INTO Animal VALUES (@animalName, @animalDescription, @animalCategory, @animalArea)";
        command.Parameters.AddWithValue("animalName", animal.Name);
        command.Parameters.AddWithValue("animalDescription", animal.Description);
        command.Parameters.AddWithValue("animalCategory", animal.Category);
        command.Parameters.AddWithValue("animalArea", animal.Area);

        command.ExecuteNonQuery();

        return Created("", animal);
    }
    
    // Same json object of animal for update and add
    [HttpPut("{idAnimal:int}")]
    public IActionResult UpdateAnimal(int idAnimal, AddAnimalRequest updatedAnimal)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "UPDATE Animal " +
                              "SET name = @animalName, " +
                              "Description = @animalDescription, " +
                              "Category = @animalCategory, " +
                              "Area = @animalArea " +
                              "WHERE IdAnimal = @idAnimal";
        command.Parameters.AddWithValue("idAnimal", idAnimal);
        command.Parameters.AddWithValue("animalName", updatedAnimal.Name);
        command.Parameters.AddWithValue("animalDescription", updatedAnimal.Description);
        command.Parameters.AddWithValue("animalCategory", updatedAnimal.Category);
        command.Parameters.AddWithValue("animalArea", updatedAnimal.Area);

        var numOfAffectedRows = command.ExecuteNonQuery();
        if (numOfAffectedRows == 0)
            return NotFound($"Animal with IdAnimal {idAnimal} not found.");
        
        return Ok(new Animal()
        {
            IdAnimal = idAnimal,
            Name = updatedAnimal.Name,
            Description = updatedAnimal.Description,
            Category = updatedAnimal.Category,
            Area = updatedAnimal.Area
        });
    }
    
    [HttpDelete("{idAnimal}")]
    public IActionResult DeleteAnimal(int idAnimal)
    {
        using SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();

        using SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "DELETE FROM Animal WHERE IdAnimal = @idAnimal";
        command.Parameters.AddWithValue("idAnimal", idAnimal);

        var rowAffected = command.ExecuteNonQuery();
        if (rowAffected == 0)
            return NotFound($"Animal with IdAnimal {idAnimal} not found.");

        return NoContent();
    }
}