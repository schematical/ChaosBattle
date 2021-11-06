
using UnityEngine;
using UnityEngine.UI;

public class ChaosBasePanel: MonoBehaviour
{
    public Button closeButton;

    public void Start()
    {
        closeButton.onClick.AddListener(CloseButtonClickEvent);
    }

   protected void HideCloseButton()
    {
        closeButton.gameObject.SetActive(false);
    }

    private void CloseButtonClickEvent()
    {
        Hide();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}