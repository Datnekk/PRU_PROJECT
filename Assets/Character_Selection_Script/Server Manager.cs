using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerManager : MonoBehaviour
{
    [Header("Settings")]

    [SerializeField] private string charSelectScene = "Character Selection";

    [SerializeField] private string gamePlaySecene = "Dark Dungeon";

    [SerializeField] private string endGameScene = "End Game";

    public static ServerManager Instance { get; private set; }

    public Dictionary<ulong, ClientData> ClientData { get; private set; }

    private bool gameHasStarted;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this; 
            DontDestroyOnLoad(gameObject);
        }
    }

    public void StartServer()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;

        NetworkManager.Singleton.OnServerStarted += OnNetworkReady;

        ClientData = new Dictionary<ulong, ClientData>();

        NetworkManager.Singleton.StartServer();
    }

    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;

        NetworkManager.Singleton.OnServerStarted += OnNetworkReady;

        ClientData = new Dictionary<ulong, ClientData>();   

        NetworkManager.Singleton.StartHost();   
    }

    private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {
        if(ClientData.Count > 2)
        {
            response.Approved = false;
            return;
        }

        response.Approved = true;
        response.CreatePlayerObject = false;
        response.Pending = false;

        ClientData[request.ClientNetworkId] = new ClientData(request.ClientNetworkId);

        Debug.Log($"Added client {request.ClientNetworkId}");
    }

    private void OnNetworkReady()
    {
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnect;

        NetworkManager.Singleton.SceneManager.LoadScene(charSelectScene, LoadSceneMode.Single);
    }

    private void OnClientDisconnect(ulong clientId)
    {
        if (ClientData.ContainsKey(clientId))
        {
            if (ClientData.Remove(clientId))
            {
                Debug.Log($"Remove client {clientId}");
            }

            if (gameHasStarted)
            {
                EndGame();
            }
        }
    }

    public void SetCharacter(ulong clientId, int characterId)
    {
        if (ClientData.TryGetValue(clientId, out ClientData data)) { 
            data.characterId = characterId;
        }
    }

    public void StartGame()
    {
        gameHasStarted = true;

        NetworkManager.Singleton.SceneManager.LoadScene(gamePlaySecene, LoadSceneMode.Single);
    }

    public void PlayerDied(ulong clientId)
    {
        if (ClientData.ContainsKey(clientId))
        {
            Debug.Log($"Player {clientId} has died.");

            ClientData[clientId].isAlive = false;

            EndGame();
        }
    }

    private void EndGame()
    {
        gameHasStarted = false;

        NetworkManager.Singleton.SceneManager.LoadScene(endGameScene, LoadSceneMode.Single);
    }
}
