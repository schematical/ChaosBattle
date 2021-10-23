using UnityEngine;
using System.Collections;

public class EditTrainingRoomBasicGameMode : GameModeBase
{
    
	public override string type
	{
        get{
            return GameManager.GameModeType.EDIT_TRAINING_ROOM_BASIC;
        }

    }
    public override void Setup(){
        GameManager.instance.DestroyEverything();
       
        GameManager.instance.boardManager.SetupScene(trainingRoomData);
        GameManager.instance.menuManager.ShowTrainingRoomEditUI();
    }
    public override void Tick(){
      
    }
    public override void Suspend(){
        
    }
    public override void Shutdown(){
        
    }
}
