using StarWars.Model.Configuration;
using StarWars.Model.Entities;

namespace StarWars.Api.GraphQl;

public class Query
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Character> GetCharacters([Service] StarWarsContext context) =>
        context.Characters;
    
    [UseFirstOrDefault]
    [UseProjection]
    public IQueryable<Character> GetCharacters(
        [Service] StarWarsContext context,
        int id) =>
        context.Characters.Where(r => r.Id == id);

}