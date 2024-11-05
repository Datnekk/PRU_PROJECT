using System;
using Unity.Netcode;

public struct Character_Selected_State : INetworkSerializable, IEquatable<Character_Selected_State>
{
    public ulong ClientID;

    public int CharacterId;

    public bool IsLockedIn;

    public Character_Selected_State(ulong clientId, int characterId = -1, bool isLockedIn = false)
    {
        ClientID = clientId;
        CharacterId = characterId;
        IsLockedIn = isLockedIn;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ClientID);
        serializer.SerializeValue(ref CharacterId);
        serializer.SerializeValue(ref IsLockedIn);
    }

    public bool Equals(Character_Selected_State other)
    {
        return ClientID == other.ClientID && CharacterId == other.CharacterId && IsLockedIn == other.IsLockedIn;  
    }
}
