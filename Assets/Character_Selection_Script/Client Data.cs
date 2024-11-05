using System;
using Unity.VisualScripting;

[Serializable]
public class ClientData 
{
    public ulong clientId;
    public int characterId = -1;
    public bool isAlive = true;

    public ClientData(ulong clientId)
    {
        this.clientId = clientId;
    }
}
