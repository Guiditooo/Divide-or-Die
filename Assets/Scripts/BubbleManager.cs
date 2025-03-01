using Unity.VisualScripting;
using UnityEngine;

public class BubbleManager : MonoBehaviour
{
    [Header("Bubble Data")]
    [SerializeField] private BubbleData smallBubbleData;
    [SerializeField] private BubbleData mediumBubbleData;
    [SerializeField] private BubbleData largeBubbleData;

    [Header("Prefabs")]
    [SerializeField] private GameObject bubblePrefab;
    [SerializeField] private GameObject bulletPrefab;

    [Header("References")]
    [SerializeField] private Transform bubbleSpawnArea;
    [SerializeField] private Transform bubbleParent;
    [SerializeField] private Transform bulletSpawnArea;
    [SerializeField] private Transform bulletParent;

    [Header("Variables")]
    private ObjectPool<GameObject> bubblePool;
    private ObjectPool<GameObject> bulletPool;

    private int level;
    private int bulletDamage;

    private void Start()
    {
        level = 1;
        bulletDamage = 1;

        bubblePool = new ObjectPool<GameObject>
            (
                createFunc: () => Instantiate(bubblePrefab, Vector3.zero, Quaternion.identity, bubbleParent),
                initialSize: 1
            );

        bulletPool = new ObjectPool<GameObject>
            (
                createFunc: () => Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity, bubbleParent),
                initialSize: 1
            );
    }

    #region Bubbles
    private GameObject CreateBubble(BubbleData data, Vector3 pos)
    {
        GameObject bubble = bubblePool.Get();
        
        bubble.transform.SetPositionAndRotation(pos, Quaternion.identity);
        bubble.transform.localScale = new Vector3(data.scaleFactor,data.scaleFactor, data.scaleFactor);
        bubble.name = data.bubbleName;
        bubble.layer = (int)data.thisType;

        bubble.SetActive(true);

        Rigidbody2D rb = bubble.GetComponent<Rigidbody2D>();

        Bubble bubbleComponent = bubble.GetOrAddComponent<Bubble>();
        bubbleComponent.Initialize(data, rb, level);

        bubbleComponent.OnCollisionWithPair += MergeBubbles;
        bubbleComponent.OnCollisionWithBullet += DamageBubble;

        return bubble;
    }

    private void DamageBubble(Bubble bubble)
    {
        if(bubble.HP-bulletDamage<=0)
        {
            PopBubble(bubble);
        }
        else
        {
            bubble.GetDamage(bulletDamage);
        }
    }

    private void PopBubble(Bubble bubble)
    {
        bubble.gameObject.SetActive(false);
        bubblePool.Release(bubble.gameObject);

        if (bubble.Data.divideIn == null)
            return;

        CreateBubble(bubble.Data.divideIn, bubble.gameObject.transform.position);
        CreateBubble(bubble.Data.divideIn, bubble.gameObject.transform.position);
    }

    private void MergeBubbles(BubbleCollision col)
    {
        col.To.gameObject.SetActive(false);
        col.From.gameObject.SetActive(false);

        bubblePool.Release(col.To.gameObject);
        bubblePool.Release(col.From.gameObject);


        CreateBubble(col.To.Data.mergeTo, (col.To.transform.position + col.From.transform.position) / 2);
    }

    #endregion

    #region Bullets
    void CreateBullet(Vector3 pos)
    {
        GameObject bullet = bulletPool.Get();

        bullet.transform.SetPositionAndRotation(pos, Quaternion.identity);

        bullet.SetActive(true);
    }
    void RecycleBullet(Bullet bullet)
    {
        bulletPool.Release(bullet.gameObject);
    }
    #endregion

    private void Update()
    {
        Testing();
    }

    //Agregarle Bubble
    //Agregarle Bubble Bounce Phs

    #region Test
    private void Testing()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            CreateBubble(largeBubbleData, bubbleSpawnArea.position);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            CreateBubble(mediumBubbleData, bubbleSpawnArea.position);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            CreateBubble(smallBubbleData, bubbleSpawnArea.position);
        }
    }
    #endregion

}