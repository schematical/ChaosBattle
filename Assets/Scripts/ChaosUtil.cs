﻿
using System;
using System.Collections.Generic;
using UnityEngine;

public class ChaosUtil
{
   
    public const String UP = "up";
    public const String DOWN = "down";
    public const String LEFT = "left";
    public const String RIGHT = "right";
    public const String PASS = "pass";
    public const String FIRE = "fires";
    
    public static List<String> KEY_DIRECTIONS = new List<String>()
    {
        LEFT,
        RIGHT,
        UP,
        DOWN
    };
    public static List<String> KEYS = new List<String>()
    {
        LEFT,
        RIGHT,
        UP,
        DOWN,
        PASS,//"pass",
        FIRE,//"fire"
    };
    
    public static String GetKeyFromVector(Vector2 vector2)
    {
        if (vector2.Equals(Vector2.down))
        {
            return DOWN;
        }else if(vector2.Equals(Vector2.up)){
            return UP;
        }else if(vector2.Equals(Vector2.left))
        {
            return LEFT;
        }else if(vector2.Equals(Vector2.right))
        {
            return RIGHT;
        }else
        {
            bool useX = Math.Abs(vector2.x) > Math.Abs(vector2.y);
            if (useX)
            {
                if (vector2.x > 0)
                {
                    return RIGHT;
                }
                else
                {
                    return LEFT;
                }

            }
            else
            {
                if (vector2.y > 0)
                {
                    //Looking up

                    return UP;
                }
                else
                {
                    //Looking down
                    return DOWN;
                }
            }
        }
    }
    public static Vector2 GetVectorFromKey(String key)
    {
        if (key.Equals(DOWN))
        {
            return Vector2.down;
        }else if (key.Equals(UP))
        {
            return Vector2.up;
        }else if (key.Equals(LEFT))
        {
            return Vector2.left;
        }else if (key.Equals(RIGHT))
        {
            return Vector2.right;
        }else
        {
            return Vector2.negativeInfinity;
        }
    }
    public static float AngleBetweenVector2(Vector2 vec1, Vector2 vec2)
    {
        Vector2 diference = vec2 - vec1;
        float sign = (vec2.y < vec1.y)? -1.0f : 1.0f;
        return Vector2.Angle(Vector2.right, diference) * sign;
    }
    public static Vector2 Rotate( Vector2 v, float degrees) {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);
         
        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
    protected static int FlattenAxis(float axisVal){
        if (axisVal > .5f)
        {
            return 1;
        }
        if (axisVal < -.5f)
        {
            return -1;
        }

        return 0;
    }
    public static Vector2 FlattenVector(Vector2 vector2)
    {
        vector2 = vector2.normalized;
        return new Vector2(
            FlattenAxis(vector2.x),
            FlattenAxis(vector2.y)
        );
    }
    

}
