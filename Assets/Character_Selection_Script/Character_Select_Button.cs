using UnityEngine;
using UnityEngine.UI;

public class Character_Select_Button : MonoBehaviour
{
    [SerializeField] private Image iconImage;

    [SerializeField] private Button button;

    private Character_Select_Display characterSelect;

    public Character Character { get; private set; }
    public bool IsDisabled { get; private set; }

    public void SetCharacter(Character_Select_Display characterSelect, Character character)
    {
        iconImage.sprite = character.Icon;

        this.characterSelect = characterSelect;
        Character = character;
    }

    public void SelectCharacter()
    {
        characterSelect.Select(Character);
    }

    public void SetDisable()
    {
        IsDisabled = true;
        button.interactable = false;
    }
}
