using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "New Character Database", menuName = "Characters/Database")]
public class Character_Database : ScriptableObject
{
    [SerializeField] private Character[] characters = new Character[0];

    public Character[] getAllCharacters() => characters;

    public Character getCharacterById(int id)
    {
        foreach (Character character in characters) {
            if (character.Id == id)
            {
                return character;
            }
        }
        return null;
    }

    public bool IsValidCharacterId(int id)
    {
        return characters.Any(x => x.Id == id);
    }
}
