using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
public class DebugPanel : MonoBehaviour {

    public class DebugPanelMode{
        public const string LOG = "LOG";
        public const string MEMORY = "MEMORY";
    }
    public Text infoText;

    public string mode = DebugPanelMode.LOG;
    public Button closeButton;
    public Button clearButton;
    public Button memoryButton;

	// Use this for initializsation
    void Start()
    {
        closeButton.onClick.AddListener(CloseButtonClickEvent);
        clearButton.onClick.AddListener(ClearButtonClickEvent);
        memoryButton.onClick.AddListener(MemoryButtonClickEvent);
    }
    public void Show(){
       
       

    }
    void MemoryButtonClickEvent(){
        mode = DebugPanelMode.MEMORY;
    }
    void CloseButtonClickEvent(){
        gameObject.SetActive(false);
    }
	
	// Update is called once per frame
	void Update () {
        switch (mode)
        {
            case(DebugPanelMode.MEMORY):
                infoText.text = "";
                /*
                infoText.text += "Bots: " + GameManager.instance.botManager.bots.Count + "\n";
                infoText.text += "Long Stored Bots: " + GameManager.instance.botManager.longStoredBots.Count + "\n";
                infoText.text += "----------------------------\n";
*/
                List<KeyValuePair<string, GarbageCollector.GCStats>> classesKeyValue = GameManager.instance.garbageCollector.classes.ToList<KeyValuePair<string, GarbageCollector.GCStats>>();
                for (int i = 0; i < classesKeyValue.Count; i++)
                {
                    KeyValuePair<string, GarbageCollector.GCStats> ele = classesKeyValue[i];
                    infoText.text += ele.Key + " - " + ele.Value.currCount + "/" + ele.Value.maxCount + "\n";
                }
                infoText.text += "----------------------------\n";
                /*
                IDictionary<string, int> classes = new Dictionary<string, int>(GameManager.instance.garbageCollector.classes);

                for (int i = 0; i < classes.Count; i++){
                    KeyValuePair<string, int> ele =  classes.ElementAt(i);
                    infoText.text += ele.Key + " - " + ele.Value.ToString() + "\n";
                }
                */
            break;
        }
	}
    void ClearButtonClickEvent(){
        infoText.text = "";
        mode = DebugPanelMode.LOG;
    }
    public void Log(string message){
        infoText.text = message + "\n" + infoText.text;
    }
}
