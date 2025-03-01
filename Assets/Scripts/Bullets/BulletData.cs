using UnityEngine;

//public enum BulletType
//{
//    LargeBubble = 6,
//    MediumBubble,
//    SmallBubble
//}

[CreateAssetMenu(fileName = "BulletData", menuName = "ScriptableObjects/BulletData", order = 2)]
public class BulletData : ScriptableObject
{
    //[Header("Bullet Type")]
    //public BulletType thisType;
    [Header("Bullet Attributes")]
    public string bulletName;
    public float speed;
}