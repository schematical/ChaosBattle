
using UnityEngine;
using UnityEngine.UI;

public class MainMenuPanel: ChaosBasePanel
{
    public Button TrainButton;

    void Init(){
 

    }
    void Start () {

        Init();
        
        HideCloseButton();
        TrainButton.onClick.AddListener(TrainButtonClickEvent);
        GameManager.instance.inputManager.IgnoreKeyboardInput();
        GameManager.instance.cameraManager.zoomingEnabled = false;

    }

    private void TrainButtonClickEvent()
    {
        GameManager.instance.SetGameMode(new TrainBasicGameMode());
        GameManager.instance.Resume();
        
        GameManager.instance.level.InitLevel();
        Hide();
    }
}
