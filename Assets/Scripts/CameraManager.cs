using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraManager : MonoBehaviour {
    public class FollowMode{
        public const string MAIN = "MAIN";
        public const string MINI_MAP = "MINI_MAP";
        public const string NNET = "NNET";
    }
    public string followMode = FollowMode.MAIN;
    public bool paningEnabled = true;
    public bool zoomingEnabled = true;
    protected ChaosEntity _chaosEntity;
    public Camera camera;
    public float zoomSize = 5;
    public Vector2 vector;
    public UnityEvent onZoomChange = new UnityEvent();
    // public MiniMapCamera miniMapCamera;
    public Camera nNetCamera;
    public Vector3 lastMousePos;
    public Dictionary<int, bool> previousButtonState = new Dictionary<int, bool>();
	// Use this for initialization
	void Start () {
        // GameManager.instance.cameraManager = this;
        camera = GetComponent<Camera>();
	}
    public void FollowEntity(ChaosEntity chaosEntity){
        FollowEntity(chaosEntity, FollowMode.MAIN);
    }
    public void FollowEntity(ChaosEntity chaosEntity, string _followMode){
        _chaosEntity = chaosEntity;
        followMode = _followMode;
        if(_chaosEntity != null){
            // chaosEntity.isBeingFollowed = true;
        }
      
       

        // nNetCamera.gameObject.SetActive(false);
        // miniMapCamera.gameObject.SetActive(false);

        switch(followMode){
            case (FollowMode.MAIN):
               
            break;
            case (FollowMode.MINI_MAP):
                // miniMapCamera.gameObject.SetActive(true);
            break;
            case(FollowMode.NNET):
                nNetCamera.gameObject.SetActive(true);
            break;
        }
    }
    public void OnZoomChange(UnityAction fun)
    {
        onZoomChange.AddListener(fun);
    }
    void OnGUI()
    {
        
    }
    public float UILineBaseWidth(){
        return .005f * Camera.main.orthographicSize;
    }
	// Update is called once per frame
	void Update () {
        bool cameraHasChanged = false;
        if (zoomingEnabled)
        {
            if (
                Input.GetAxis("Mouse ScrollWheel") > 0 ||
                Input.GetKey(KeyCode.LeftBracket)
            )
            {
                if (camera.orthographicSize > 1)
                {
                    camera.orthographicSize = GetComponent<Camera>().orthographicSize - 1;
                    cameraHasChanged = true;
                }

            }
            if (
                Input.GetAxis("Mouse ScrollWheel") < 0 ||
                Input.GetKey(KeyCode.RightBracket)

            )
            {
                if (camera.orthographicSize < 30)
                {
                    camera.orthographicSize = GetComponent<Camera>().orthographicSize + 1;
                    cameraHasChanged = true;
                }

            }

            CheckButtonState(2);
            if (Input.GetMouseButton(2))
            {
                Vector3 deltaVec = lastMousePos - Input.mousePosition;
                
                transform.position += deltaVec * .3f;
            }
        }
       
 
        lastMousePos = Input.mousePosition;

        /*if(GameManager.instance.gameMode != null){
            GameManager.instance.gameMode.CheckInputs();
        }*/



        if(cameraHasChanged){
            onZoomChange.Invoke();
        }
        /*

        if(Input.GetAxis("Mouse "))
        */
        if(!_chaosEntity) { // || !player.isBeingFollowed){
            return;
        }
        Vector3 vector3  = new Vector3(_chaosEntity.transform.position.x, _chaosEntity.transform.position.y, transform.position.z);
        switch(followMode){
            case(FollowMode.MAIN):
                transform.position = vector3;
            break;
            case (FollowMode.MINI_MAP):
                // miniMapCamera.transform.position = vector3;
            break;
            case (FollowMode.NNET):
                nNetCamera.transform.SetPositionAndRotation(vector3, _chaosEntity.transform.localRotation);
            break;
        }
       
	}

    private void CheckButtonState(int buttonIndex)
    {
        if (
            !previousButtonState.ContainsKey(buttonIndex) 
        )
        {
            previousButtonState.Add(buttonIndex, Input.GetMouseButtonDown(buttonIndex));
        }
        else
        {
            if (
                previousButtonState[buttonIndex] != Input.GetMouseButtonDown(buttonIndex)
            )
            {
                if (Input.GetMouseButtonDown(buttonIndex))
                {
                    // StartDrag(2);
                }
                else
                {
                    // EndDrag(2);
                }
            }
        }
        previousButtonState[buttonIndex] = Input.GetMouseButtonDown(buttonIndex);
    }
}
