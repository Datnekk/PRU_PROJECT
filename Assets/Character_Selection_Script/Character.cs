using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Characters/Character")]
public class Character : ScriptableObject
{
    [SerializeField]
    private int id = -1;

    [SerializeField]
    private string displayName;

    [SerializeField]
    private Sprite icon;

    [SerializeField]
    private Sprite characterSprite;

    [SerializeField]
    private GameObject characterPrefab;

    [SerializeField]
    private NetworkObject gamePlayPrefab;

    public int Id => id;

    public string DisplayName => displayName;

    public Sprite Icon => icon;

    public Sprite CharacterSprite => characterSprite;

    public GameObject CharacterPrefab => characterPrefab;

    public NetworkObject GameplayPrefab => gamePlayPrefab;
}
