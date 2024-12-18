using HotChocolate.Subscriptions;
using StarWars.Domain.Interface;
using StarWars.Model.Entities;

namespace StarWars.Api.GraphQl;

public class Mutation
{
    public async Task<Character> AddCharacter(
        [Service] IRepository<Character> characterRepository,
        [Service] ITopicEventSender sender,
        Character character)
    {
        var insertedCharacter = await characterRepository
            .UpSertAsync(character);
        await sender.SendAsync(
            nameof(Subscription.OnCharacterAdded), insertedCharacter);
        return insertedCharacter;
    }

}