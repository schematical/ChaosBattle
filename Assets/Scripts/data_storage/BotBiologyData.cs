using UnityEngine;
using System.Collections;
using System.Collections.Generic;
[System.Serializable]
public class BotBiologyData: BaseData//ICTSerializedData
{


    [System.Serializable]
    public class EyeData
    {
        public string id;
        public float visionDistance = 3;
        public float startDistance = 0;
        public float angle;
        public float angularSpread = 15;
      
    }

    public float r;
    public float g;
    public float b;
    public float speed;
    public float size;
    public float turnSpeed;
    public List<EyeData> eyes = new List<EyeData>();


    public override string _class_name
    {
        get {
            return "BotBiologyData";
        }
    }
    public Color color{
        get
        {
            return new Color(
                1-r,
                1-g,
                1-b
            );
        }
    }

    public void PopulateRandom(){
        r = Random.Range(0f, 255f) / 255f;
        g = Random.Range(0f, 255f) / 255f;
        b = Random.Range(0f, 255f) / 255f;
        speed = 4;//Random.Range(2, 5);
        turnSpeed = 90;
    }
    public BotBiologyData MutateRandom(){
        BotBiologyData newBotBiology = new BotBiologyData();


        newBotBiology.r = MutateColorField(r);
        newBotBiology.g = MutateColorField(g);
        newBotBiology.b = MutateColorField(b);
        newBotBiology.speed = speed;
        newBotBiology.turnSpeed = turnSpeed;
        newBotBiology.eyes = new List<EyeData>();
        int chance = -1;
        foreach(EyeData parentEyeData in eyes){
            string json = JsonUtility.ToJson(parentEyeData);
            EyeData eyeData = JsonUtility.FromJson<EyeData>(json);
            newBotBiology.eyes.Add(eyeData);
            chance = (int)Random.Range(0, 100);
            if(chance < 1){
                //Remove it

            }else if (chance < 25){
                //Mutate Start/End
                eyeData.startDistance += Random.Range(-1, 1) / 10;
                if(eyeData.startDistance < 0){
                    eyeData.startDistance = 0;
                }
                eyeData.visionDistance += Random.Range(-1, 1) / 10;
                if (eyeData.visionDistance < 0)
                {
                    eyeData.visionDistance = 0;
                }
            }else if(chance < 50){
                //Mutate Degrees
                eyeData.angle += Random.Range(-5, 5);
            }else if( chance < 75){
                //Mutate widths
                eyeData.angularSpread += Random.Range(-5, 5);
            }

        }
        /*
        chance = (int)Random.Range(0, 100);
        if (chance >  98){
        //Add an eye
            EyeData eyeData = new EyeData();
            //Mutate Start/End
            eyeData.startDistance += Random.Range(0, 4);
            eyeData.visionDistance += Random.Range(0, 7);
       
            //Mutate Degrees
            eyeData.angle += Random.Range(-90, 90);
      
            //Mutate widths
            eyeData.angularSpread += Random.Range(-45, 45);
            eyeData.id = newBotBiology.eyes.Count.ToString();
            newBotBiology.eyes.Add(eyeData);
        }*/
        //speed += Random.Range(-20, 20)/10;
        return newBotBiology;
    }
    protected float MutateColorField(float cF){
        float RANGE = 2;
        float newCf = cF + (Random.Range(RANGE * -1f, RANGE) / 255);
        if (newCf < 0)
        {
            newCf = 0;
        }
        if (newCf > 1)
        {
            newCf = 1;
        }
        return newCf;
    }
}
