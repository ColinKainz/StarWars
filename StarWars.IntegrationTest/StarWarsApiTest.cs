using System.Net;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using StarWars.Model.Entities;

namespace StarWars.IntegrationTest;

public class StarWarsApiTest
{
    private readonly HttpClient _client = new WebApplicationFactory<Program>().CreateClient();

    [Fact]
    public async Task SeedChars()
    {
        
        var testCharacters = new List<Character>();

        for (int i = 0; i < 50; i++)
        {
            testCharacters.Add(new Character
            {
                Name = $"CT-{RandomNumberGenerator.GetInt32(1000, 9999)}",
                Faction = "Republic",
                Species = "Clone",
                Homeworld = "Kamino"
            });
        }

        var jsonContent = JsonConvert.SerializeObject(testCharacters);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        
        // bulk insert
        var response = await _client.PostAsync("/characters/bulk", content);
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Update_Multiple_Chars()
    {
        await DeleteAll();
        var testCharacters = new List<Character>
        {
            new() { Name = "CT-7567 Rex", Faction = "Republic", Species = "Clone", Homeworld = "Kamino" },
            new() { Name = "CT-782 Hevy", Faction = "Republic", Species = "Clone", Homeworld = "Kamino" },
        };

        var jsonContent = JsonConvert.SerializeObject(testCharacters);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
        
        // bulk insert
        var response = await _client.PostAsync("/characters/bulk", content);
        
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        
        var getAll = await _client.GetAsync("/characters");
        var getAllContent = await getAll.Content.ReadAsStringAsync();
        var characters = JsonConvert.DeserializeObject<List<Character>>(getAllContent);
        
        Assert.NotNull(characters);
        Assert.NotEmpty(characters);
        
        foreach (var character in characters)
        {
            character.Faction = "Empire";
        }
        
        var updateContent = JsonConvert.SerializeObject(characters);
        var update = new StringContent(updateContent, Encoding.UTF8, "application/json");
        
        var updateResponse = await _client.PutAsync("/characters/bulk/update", update);
        
        Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
        
        var updatedContent = JsonConvert.DeserializeObject<List<Character>>(updateContent);
        
        Assert.NotNull(updatedContent);
        Assert.NotEmpty(updatedContent);
        
        Assert.All(updatedContent, c => Assert.Equal("Empire", c.Faction));
    }

    [Fact]
    public async Task DeleteAll()
    {
        await SeedChars();
        // Get all characters
        var getResponse = await _client.GetAsync("/characters");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

        var getContent = await getResponse.Content.ReadAsStringAsync();
        var characters = JsonConvert.DeserializeObject<List<Character>>(getContent);

        Assert.NotNull(characters);
        Assert.NotEmpty(characters);

        // Delete all characters
        var jsonContent = JsonConvert.SerializeObject(characters);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var deleteRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri("/characters/bulk/delete", UriKind.Relative),
            Content = content
        };

        var deleteResponse = await _client.SendAsync(deleteRequest);

        Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

        var deleteContent = await deleteResponse.Content.ReadAsStringAsync();
        var deletedCharacters = JsonConvert.DeserializeObject<List<Character>>(deleteContent);

        Assert.NotNull(deletedCharacters);
        Assert.Equal(characters.Count, deletedCharacters.Count);

        // Verify deletion
        var getAfterDeleteResponse = await _client.GetAsync("/characters");
        Assert.Equal(HttpStatusCode.OK, getAfterDeleteResponse.StatusCode);

        var getAfterDeleteContent = await getAfterDeleteResponse.Content.ReadAsStringAsync();
        var charactersAfterDelete = JsonConvert.DeserializeObject<List<Character>>(getAfterDeleteContent);

        Assert.NotNull(charactersAfterDelete);
        Assert.Empty(charactersAfterDelete);
    }

    [Fact]
    public async Task CreateCharacter_ShouldAddCharacter()
    {
        // Arrange
        var character = new Character
        {
            Name = "Yoda",
            Faction = "Jedi",
            Species = "Unknown",
            Homeworld = "Unknown"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/characters", character);

        // Assert
        response.EnsureSuccessStatusCode();
        var createdCharacter = await response.Content.ReadFromJsonAsync<Character>();
        Assert.NotNull(createdCharacter);
        Assert.Equal("Yoda", createdCharacter.Name);

        await DeleteAll();
    }

    [Fact]
    public async Task GetCharacterById_ShouldReturnCharacter()
    {
        // Arrange
        var character = new Character
        {
            Name = "Mace Windu",
            Faction = "Jedi",
            Species = "Human",
            Homeworld = "Haruun Kal"
        };
        var postResponse = await _client.PostAsJsonAsync("/characters", character);
        var createdCharacter = await postResponse.Content.ReadFromJsonAsync<Character>();

        // Act
        var response = await _client.GetAsync($"/characters/{createdCharacter!.Id}");

        // Assert
        response.EnsureSuccessStatusCode();
        var fetchedCharacter = await response.Content.ReadFromJsonAsync<Character>();
        Assert.Equal("Mace Windu", fetchedCharacter!.Name);
        Assert.Equal(createdCharacter.Id, fetchedCharacter.Id);
    }

    [Fact]
    public async Task UpdateCharacter_ShouldModifyCharacter()
    {
        // Arrange
        var character = new Character
        {
            Name = "Anakin Skywalker",
            Faction = "Jedi",
            Species = "Human",
            Homeworld = "Tatooine"
        };
        var postResponse = await _client.PostAsJsonAsync("/characters", character);
        var createdCharacter = await postResponse.Content.ReadFromJsonAsync<Character>();
        
        Assert.NotNull(createdCharacter);

        // Act
        createdCharacter.Faction = "Sith";
        var putResponse = await _client.PutAsJsonAsync("/characters/", createdCharacter);

        // Assert
        putResponse.EnsureSuccessStatusCode();
        var updatedCharacter = await putResponse.Content.ReadFromJsonAsync<Character>();
        Assert.NotNull(updatedCharacter);
        Assert.Equal("Sith", updatedCharacter.Faction);
    }

    [Fact]
    public async Task DeleteCharacterById_ShouldRemoveCharacter()
    {
        // Arrange
        var character = new Character
        {
            Name = "Padmé Amidala",
            Faction = "Republic",
            Species = "Human",
            Homeworld = "Naboo"
        };
        var postResponse = await _client.PostAsJsonAsync("/characters", character);
        var createdCharacter = await postResponse.Content.ReadFromJsonAsync<Character>();
        Assert.NotNull(createdCharacter);

        // Act
        var deleteResponse = await _client.DeleteAsync($"/characters/{createdCharacter.Id}");

        // Assert
        deleteResponse.EnsureSuccessStatusCode();
        var deletedCharacter = await deleteResponse.Content.ReadFromJsonAsync<Character>();
        Assert.NotNull(deletedCharacter);
        Assert.Equal("Padmé Amidala", deletedCharacter.Name);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/characters/{createdCharacter.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
    
    [Fact]
    public async Task DeleteCharacter_ShouldRemoveCharacter()
    {
        // Arrange
        var character = new Character
        {
            Name = "Obi-Wan Kenobi",
            Faction = "Jedi",
            Species = "Human",
            Homeworld = "Stewjon"
        };
        
        var postResponse = await _client.PostAsJsonAsync("/characters", character);
        var createdCharacter = await postResponse.Content.ReadFromJsonAsync<Character>();
        
        Assert.NotNull(createdCharacter);
        Assert.Equal("Obi-Wan Kenobi", createdCharacter.Name);

        // Act
        var jsonContent = JsonConvert.SerializeObject(createdCharacter);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        var deleteRequest = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            RequestUri = new Uri("/characters", UriKind.Relative),
            Content = content
        };
        
        var deleteResponse = await _client.SendAsync(deleteRequest);

        // Assert
        deleteResponse.EnsureSuccessStatusCode();
        var deletedCharacter = await deleteResponse.Content.ReadFromJsonAsync<Character>();
        Assert.NotNull(deletedCharacter);
        Assert.Equal("Obi-Wan Kenobi", deletedCharacter.Name);

        // Verify deletion
        var getResponse = await _client.GetAsync($"/characters/{createdCharacter.Id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
    
    [Fact]
    public async Task GetCharacters_WithPagination_ShouldReturnCorrectPage()
    {
        // Arrange
        await SeedChars(); // Seed more than 10 characters

        // Act
        var response = await _client.GetAsync("/characters?ps=5&p=2");

        // Assert
        response.EnsureSuccessStatusCode();
        var characters = await response.Content.ReadFromJsonAsync<List<Character>>();
        Assert.NotNull(characters);
        Assert.Equal(5, characters.Count);
        // Additional assertions can be made to ensure the correct characters are returned
    }
    
    [Fact]
    public async Task GetCharacterById_NonExistent_ShouldReturnNotFound()
    {
        // Act
        var response = await _client.GetAsync("/characters/9999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task CreateCharacter_InvalidData_ShouldReturnBadRequest()
    {
        // Arrange
        var character = new Character
        {
            // Name is missing
            Faction = "Jedi",
            Species = "Unknown",
            Homeworld = "Unknown"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/characters", character);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}