using StarWars.Model.Configuration;
using StarWars.Model.Entities;

namespace StarWars.Domain.Repository;

public class CharacterRepository: ARepository<Character>
{
    public CharacterRepository(StarWarsContext context) : base(context)
    {
    }
}