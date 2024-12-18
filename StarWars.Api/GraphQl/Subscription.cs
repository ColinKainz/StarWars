using StarWars.Model.Entities;

namespace StarWars.Api.GraphQl;

public class Subscription
{
    [Subscribe]
    public Character OnCharacterAdded(
        [EventMessage] Character character) => character;
}