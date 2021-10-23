using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class InputManager{
    public UnityEvent onTimeScaleChange = new UnityEvent();
    protected bool listenToKeyboardInput = false;
    public void ListenToKeyboardInput(){
        listenToKeyboardInput = true;
        GameManager.instance.cameraManager.paningEnabled = true;
        GameManager.instance.cameraManager.zoomingEnabled = true;
    }
    public void IgnoreKeyboardInput()
    {
        listenToKeyboardInput = false;
        GameManager.instance.cameraManager.zoomingEnabled = false;
        GameManager.instance.cameraManager.paningEnabled = false;
    }
    public void Tick(){
        if (
            Input.GetKeyDown(KeyCode.Escape)
        )
        {
            GameManager.instance.menuManager.HideOverlays();
            ListenToKeyboardInput();
            // GameManager.instance.menuManager.menuBarPanel.Show();
            // GameManager.instance.menuManager.footerPanel.Show();
            // GameManager.instance.SetDiagnosticsMode(false);
        }

        if(!listenToKeyboardInput){
            return;
        }


        /*if (
            Input.GetKeyDown(KeyCode.Tab)
        )
        {
            BotController botController = GameManager.instance.level.RandomAliveBot();
            if (botController != null)
            {
                GameManager.instance.cameraManager.FollowEntity(botController.entity);
            }

        }*/





        if (
           Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.Equals)
        ){
            SpeedUp();
        }



        if (
            Input.GetKeyDown(KeyCode.Minus)
        ){
            SlowDown();
        }

        if (
           /*
           (
               Input.GetKeyDown(KeyCode.LeftControl) ||
               Input.GetKeyDown(KeyCode.LeftApple) ||
               Input.GetKeyDown(KeyCode.LeftAlt) ||
               Input.GetKeyDown(KeyCode.RightControl)
           ) && */
           Input.GetKeyDown(KeyCode.D)
       )
        {
            ToggleDebug();

        }

        if (
           Input.GetKeyDown(KeyCode.E)
       )
        {
            IGameModeWithSpawnables gameMode = (IGameModeWithSpawnables)GameManager.instance.gameMode;
            //Increase count
            if (gameMode.maxBotCount > 80)
            {
                gameMode.maxBotCount = 160;
                return;
            }
            gameMode.maxBotCount *= 2;
        }

        if (
            Input.GetKeyDown(KeyCode.Q)
        )
        {
            IGameModeWithSpawnables gameMode = (IGameModeWithSpawnables)GameManager.instance.gameMode;
            //Decrease count
            if (gameMode.maxBotCount < 10)
            {
                gameMode.maxBotCount = 5;
                return;
            }
            gameMode.maxBotCount = (int)Mathf.Round(gameMode.maxBotCount * .5f);
        }
        if (Input.GetKey(KeyCode.C))
        {
            GameManager.instance.menuManager.HideAllMenues();
            // GameManager.instance.menuManager.cinimaticOverlayPanel.Show();

        }
        /*if(Input.GetKey(KeyCode.M)){
            GameManager.instance.menuManager.menuBarPanel.Show();
            GameManager.instance.menuManager.footerPanel.Show();
            GameManager.instance.menuManager.cinimaticOverlayPanel.Stop();
        }*/

    }
    public void ToggleDebug(){
        if (GameManager.instance.menuManager.debugPanel.isActiveAndEnabled)
        {
            GameManager.instance.menuManager.debugPanel.gameObject.SetActive(false);
        }
        else
        {
            GameManager.instance.menuManager.debugPanel.gameObject.SetActive(true);
            GameManager.instance.menuManager.debugPanel.Show();
        }
    }
    public void SpeedUp(){
        //Double game speed up to 8
        if (Time.timeScale > 32)
        {
            Time.timeScale = 64;
            return;
        }
        Time.timeScale = Time.timeScale * 2;
        if (Time.timeScale.Equals(0))
        {
            Time.timeScale = .125f;
        }
        onTimeScaleChange.Invoke();
    }
    public void SlowDown(){
        //Divide game speed down to 1
      
        if (Time.timeScale < .125f)
        {
            Time.timeScale = 0;
            return;
        }
        Time.timeScale = Time.timeScale * .5f;
        onTimeScaleChange.Invoke();
    }

}
