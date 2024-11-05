using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Player_Card : MonoBehaviour
{
    [SerializeField] private Character_Database characterDatabase;

    [SerializeField] private GameObject visuals;

    [SerializeField] private Image characterIconImage;

    [SerializeField] private TMP_Text playerNameText;

    [SerializeField] private TMP_Text characterNameText;

    public void UpdateDisplay(Character_Selected_State state)
    {
        if (state.CharacterId != -1)
        {
            var character = characterDatabase.getCharacterById(state.CharacterId);
            characterIconImage.sprite = character.Icon;
            characterNameText.text = character.DisplayName;

        }
        else
        {
            characterIconImage.enabled = false;
        }

        playerNameText.text = state.IsLockedIn ? $"Player {state.ClientID}" : $"Player {state.ClientID} (Picking ...)";

        visuals.SetActive(true);
    }

    public void DisableDisplay()
    {
        visuals.SetActive(false);
    }
}
