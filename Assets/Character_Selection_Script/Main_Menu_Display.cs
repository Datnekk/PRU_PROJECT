using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main_Menu_Display : NetworkBehaviour
{
    
    public void StartHost()
    {
        ServerManager.Instance.StartHost();      
    }

    public void StartServer()
    {
        ServerManager.Instance.StartServer();
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();

    }
}
