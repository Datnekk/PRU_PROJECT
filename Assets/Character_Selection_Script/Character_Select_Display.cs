using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Character_Select_Display : NetworkBehaviour
{
    [SerializeField] private Character_Database characterDatabase;

    [SerializeField] private Transform  characterHolder;

    [SerializeField] private Character_Select_Button selectButtonPrefab;

    [SerializeField] private Player_Card[] playerCards;

    [SerializeField] private TMP_Text characterNameText;

    [SerializeField] private SpriteRenderer characterSpriteRenderer;

    [SerializeField] private Button lockInbutton;

    private List<Character_Select_Button> characterButtons = new List<Character_Select_Button>();

    private NetworkList<Character_Selected_State> players;

    private void Awake()
    {
        players = new NetworkList<Character_Selected_State>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            Character[] allCharacters = characterDatabase.getAllCharacters();

            foreach (var character in allCharacters)
            {
                var selectButtonInstance = Instantiate(selectButtonPrefab, characterHolder);
                selectButtonInstance.SetCharacter(this, character);
                characterButtons.Add(selectButtonInstance); 
            }

            players.OnListChanged += HandlePlayerStateChange;
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
        }
        foreach(NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
        {
            HandleClientConnected(client.ClientId);
        }
    }

    public override void OnNetworkDespawn()
    {
        if (IsClient)
        {
            players.OnListChanged -= HandlePlayerStateChange;
        }

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnected;
        }

    }

    private void HandleClientConnected(ulong clientID)
    {
        players.Add(new Character_Selected_State(clientID));
    }

    private void HandleClientDisconnected(ulong clientID)
    {
        for (int i = 0; i < players.Count; i++) {
            if (players[i].ClientID == clientID) { 
                players.RemoveAt(i);
                break;
            }
        }  
    }

    public void Select(Character character)
    {
        for (int i = 0; i < players.Count; i++) {
            if (players[i].ClientID != NetworkManager.Singleton.LocalClientId)
            {
                continue;
            }

            if (players[i].IsLockedIn) { return; }

            if (players[i].CharacterId == character.Id) { return; }

            if (IsCharacterTaken(character.Id, false)) { return; }
        }

        characterNameText.text = character.DisplayName;
        characterSpriteRenderer.sprite = character.CharacterSprite;

        SelectServerRpc(character.Id);
    }


    [ServerRpc(RequireOwnership = false)]
    private void SelectServerRpc(int characterId, ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientID != serverRpcParams.Receive.SenderClientId) { continue; }

            if (!characterDatabase.IsValidCharacterId(characterId))
            {
                return;
            }

            if (IsCharacterTaken(characterId, true))
            {
                return;
            }

            players[i] = new Character_Selected_State(
                        players[i].ClientID,
                        characterId,
                        players[i].IsLockedIn
                );
        }
    }

    public void LockIn()
    {
        LockInServerRPC();
    }

    [ServerRpc(RequireOwnership = false)]
    private void LockInServerRPC(ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (players[i].ClientID != serverRpcParams.Receive.SenderClientId) { continue; }

            if (!characterDatabase.IsValidCharacterId(players[i].CharacterId))
            {
                return;
            }

            if (IsCharacterTaken(players[i].CharacterId, true))
            {
                return;
            }

            players[i] = new Character_Selected_State(
                        players[i].ClientID,
                        players[i].CharacterId,
                        true
             );
        }

        foreach (var player in players) {
            if (!player.IsLockedIn)
            {
                return;
            }
        }

        foreach (var player in players)
        {
            ServerManager.Instance.SetCharacter(player.ClientID, player.CharacterId);
        }

        ServerManager.Instance.StartGame();
    }

    private void HandlePlayerStateChange(NetworkListEvent<Character_Selected_State> changeEvent)
    {
        for (int i = 0; i < playerCards.Length; i++)
        {
            if (players.Count > i) {
                playerCards[i].UpdateDisplay(players[i]);
            } else
            {
                playerCards[i].DisableDisplay();
            }
        }

        foreach (var button in characterButtons) {
            if (button.IsDisabled) { continue; }

            if(IsCharacterTaken(button.Character.Id, false))
            {
                button.SetDisable();
            }
        }

        foreach (var player in players)
        {
            if (player.ClientID != NetworkManager.Singleton.LocalClientId) { continue; }

            if (player.IsLockedIn)
            {
                lockInbutton.interactable = false;
                break;
            }

            if (IsCharacterTaken(player.CharacterId, false))
            {
                lockInbutton.interactable = false;
                break;
            }

            lockInbutton.interactable = true;
            break;
        }
    }

    private bool IsCharacterTaken(int characterId, bool checkAll) {
        for (int i = 0; i < players.Count; i++)
        {
            if (!checkAll)
            {
                if (players[i].ClientID == NetworkManager.Singleton.LocalClientId) { continue; }
            }

            if (players[i].IsLockedIn && players[i].CharacterId == characterId)
            {
                return true;
            }
        }

        return false;   
    }

}
