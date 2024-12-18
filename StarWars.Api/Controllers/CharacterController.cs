using Microsoft.AspNetCore.Mvc;
using StarWars.Bl.Interface;
using StarWars.Model.Entities;

namespace StarWars.Api.Controllers;

public static class CharacterController
{
    public static void Map(WebApplication app)
    {
        // Get all characters
        app.MapGet("/characters", async (
            [FromServices] IService<Character> service,
            [FromQuery(Name = "ps")] int pageSize = 0,
            [FromQuery(Name = "p")] int pageNumber = 0) =>
        {
            if (pageSize < 0) 
                return Results.BadRequest("Page size must be greater or equal to 1");
            
            if (pageNumber < 0)
                return Results.BadRequest("Page number must be greater or equal to 1");

            if (pageSize > 0 && pageNumber == 0)
                return Results.BadRequest("Page number must be greater or equal to 1 when page size is greater than 0");
            
            var characters = pageSize == 0
                ? await service.GetAllAsync() 
                : await service.GetPagedAsync(pageNumber, pageSize);
            
            return Results.Ok(characters);
        }).WithName("GetAllCharacters");
    
        // Get character by id
        app.MapGet("/characters/{id:int}", async (
            [FromRoute] int id, 
            [FromServices] IService<Character> service) =>
        {
            var character = await service.GetByIdAsync(id);
            return character is not null ? Results.Ok(character) : Results.NotFound();
        }).WithName("GetCharacterById");

        // Create character
        app.MapPost("/characters", async (
            [FromBody] CharacterDto character, 
            [FromServices] IService<Character> service) =>
        {
            try
            {
                var convertedCharacter = new Character
                {
                    Name = character.Name,
                    Faction = character.Faction,
                    Species = character.Species,
                    Homeworld = character.Homeworld
                };
                var createdCharacter = await service.AddAsync(convertedCharacter);
                return Results.Created($"/characters/{createdCharacter!.Id}", createdCharacter);
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        }).WithName("CreateCharacter");

        // Create multiple characters
        app.MapPost("/characters/bulk", async (
            [FromBody] IEnumerable<CharacterDto> characters, 
            [FromServices] IService<Character> service) =>
        {
            try
            {
                var convertedCharacters = characters.Select(character => new Character
                {
                    Name = character.Name,
                    Faction = character.Faction,
                    Species = character.Species,
                    Homeworld = character.Homeworld
                });
                var createdCharacters = await service.CreateRangeAsync(convertedCharacters);
                return Results.Created($"/characters", createdCharacters);
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        }).WithName("CreateCharacters");
        
        // Update character
        app.MapPut("/characters", async (
            [FromBody] Character character, 
            [FromServices] IService<Character> service) =>
        {
            var updatedCharacter = await service.UpSertAsync(character);
            return Results.Ok(updatedCharacter);
        }).WithName("UpdateCharacter");

        // Update multiple characters
        app.MapPut("/characters/bulk/update", async (
            [FromBody] IEnumerable<Character> characters, 
            [FromServices] IService<Character> service) =>
        {
            var updatedCharacters = await service.UpSertRangeAsync(characters);
            return Results.Ok(updatedCharacters);
        }).WithName("UpdateCharacters");
        
        // Delete character by id
        app.MapDelete("/characters/{id:int}", async (
            [FromRoute] int id, 
            [FromServices] IService<Character> service) =>
        {
            var deletedCharacter = await service.DeleteAsyncById(id);
            return deletedCharacter == null ? Results.NotFound() : Results.Ok(deletedCharacter);
        }).WithName("DeleteCharacterById");
        
        // Delete character
        app.MapDelete("/characters", async (
            [FromBody] Character character, 
            [FromServices] IService<Character> service) =>
        {
            var deletedCharacter = await service.DeleteAsync(character);
            return deletedCharacter == null ? Results.NotFound() : Results.Ok(deletedCharacter);
        }).WithName("DeleteCharacter");

        // Delete multiple characters
        app.MapDelete("/characters/bulk/delete", async (
            [FromBody] IEnumerable<Character> ids, 
            [FromServices] IService<Character> service) =>
        {
            var deletedCharacters = await service.DeleteRangeAsync(ids);
            return deletedCharacters == null ? Results.NotFound() : Results.Ok(deletedCharacters);
        }).WithName("DeleteCharacters");
        
    }
}