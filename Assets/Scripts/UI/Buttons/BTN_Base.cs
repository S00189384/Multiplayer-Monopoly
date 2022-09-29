using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BTN_Base : MonoBehaviour
{
    protected Button button;
    protected TextMeshProUGUI tmp_buttonText;

    public virtual void Awake()
    {
        button = GetComponent<Button>();
        tmp_buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void AddOnClickListener(UnityEngine.Events.UnityAction call)
    {
        if (button == null)
            button = GetComponent<Button>();

        button.onClick.AddListener(call);
    }

    public void SetButtonInteractable(bool interactable)
    {
        if (button == null)
            button = GetComponent<Button>();

        button.interactable = interactable;
    }

    public void SetButtonText(string text)
    {
        if(tmp_buttonText == null)
            tmp_buttonText = GetComponentInChildren<TextMeshProUGUI>();

        tmp_buttonText.text = text;
    }
}
