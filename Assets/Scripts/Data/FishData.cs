using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;


[CreateAssetMenu(fileName = "FishData", menuName = "FishData", order = 0)]
public class FishData : ScriptableObject {
    public string fishName = "";
    public Sprite fishSprite;
    public float fishWorth = 0;
    public int fishWallBounces = 3;
    public float fishSpeed = 0;
    public bool fishClamp = true;

}
