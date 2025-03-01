using UnityEngine;

public enum BubbleType
{
    LargeBubble = 6,
    MediumBubble,
    SmallBubble
}

[CreateAssetMenu(fileName = "BubbleData", menuName = "ScriptableObjects/BubbleData", order = 1)]
public class BubbleData : ScriptableObject
{
    [Header("Bubble Type")]
    public BubbleType thisType;
    [Header("Bubble Attributes")]
    public string bubbleName;
    public float sizeFactor;
    public float scaleFactor;
    public float hpFactor;
    public int baseHP;
    public float initialSpeed;
    public float minRotation;
    public float maxRotation;
    public float wakeUpTime;
    [Header("Bubble Results")]
    public BubbleData mergeTo;
    public BubbleData divideIn;
}