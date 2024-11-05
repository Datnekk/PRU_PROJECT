using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class CharacterSpawner : NetworkBehaviour
{
    [SerializeField] private Character_Database CharacterDatabase;

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        foreach (var client in ServerManager.Instance.ClientData)
        {
            var character = CharacterDatabase.getCharacterById(client.Value.characterId);

            if(character != null)
            {
                var spawnPos = new Vector3(Random.Range(-3f, 3f), 0f, Random.Range(-3f, 3f));
                var characterInstance = Instantiate(character.GameplayPrefab, spawnPos, Quaternion.identity);
                characterInstance.SpawnAsPlayerObject(client.Value.clientId);
            }
        }
    }
}