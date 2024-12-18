using StarWars.Domain.Interface;
using StarWars.Model.Entities;

namespace StarWars.Bl.Services;

public class CharacterService: AService<Character>
{
    public CharacterService(IRepository<Character> repository) : base(repository)
    {
    }
}