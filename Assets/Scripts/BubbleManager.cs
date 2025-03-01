using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class BubbleManager : MonoBehaviour
{
    [Header("Bubble Data")]
    [SerializeField] private BubbleData smallBubbleData;
    [SerializeField] private BubbleData mediumBubbleData;
    [SerializeField] private BubbleData largeBubbleData;

    [Header("Bullet Data")]
    [SerializeField] private BulletData bulletData;

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
                createFunc: () => Instantiate(bulletPrefab, Vector3.zero, bulletPrefab.transform.rotation, bulletParent),
                initialSize: 1
            );

        StartCoroutine(AutoShoot(0.25f));
    }

    #region Bubbles
    private GameObject CreateBubble(BubbleData data, Vector3 pos, bool isMerged, int hp = 0, bool isNewBubble = false)
    {
        //GameObject bubble = bubblePool.Get();
        GameObject bubble = Instantiate(bubblePrefab, Vector3.zero, Quaternion.identity, bubbleParent);

        bubble.transform.SetPositionAndRotation(pos, Quaternion.identity);

        Bubble bubbleComponent = bubble.GetOrAddComponent<Bubble>();

        bubbleComponent.Initialize(data, isMerged ? hp : (int)(data.hpFactor * (level + data.baseHP)), isNewBubble);

        bubbleComponent.OnCollisionWithPair += MergeBubbles;
        bubbleComponent.OnCollisionWithBullet += DamageBubble;
        bubbleComponent.OnCollisionWithPlayer += ShowGameOver;

        bubble.SetActive(true);

        return bubble;
    }

    private void DamageBubble(BubbleBulletCollision col)
    {
        RecycleBullet(col.bullet);

        if (col.bubble.HP - bulletDamage <= 0)
        {
            PopBubble(col.bubble);
        }
        else
        {
            col.bubble.GetDamage(bulletDamage);
        }

    }

    private void PopBubble(Bubble bubble)
    {

        BubbleData data = bubble.Data;

        Bounds bounds = bubble.gameObject.GetComponent<SpriteRenderer>().bounds;

        Vector3 pos1 = new Vector3(bounds.min.x, bounds.max.y, 0);
        Vector3 pos2 = new Vector3(bounds.max.x, bounds.max.y, 0);

        bubble.OnCollisionWithPair -= MergeBubbles;
        bubble.OnCollisionWithBullet -= DamageBubble;
        bubble.OnCollisionWithPlayer -= ShowGameOver;

        bubble.gameObject.SetActive(false);

        Destroy(bubble.gameObject);
        //bubblePool.Release(bubble.gameObject);

        //Debug.Log("Exploto " + bubble.name);

        if (data.divideIn == null)
            return;

        //Debug.Log("La burbuja esta creando 2 " + data.divideIn.thisType.ToString());

        CreateBubble(data.divideIn, pos1, false);
        CreateBubble(data.divideIn, pos2, false);
    }

    private void MergeBubbles(BubbleBubbleCollision col)
    {
        Bubble bub1 = col.From;
        Bubble bub2 = col.To;

        if (bub1.Data.mergeTo.thisType != bub2.Data.mergeTo.thisType)
        {
            return;
        }

        bub1.gameObject.GetComponent<CircleCollider2D>().enabled = false;
        bub2.gameObject.GetComponent<CircleCollider2D>().enabled = false;

        CreateBubble(bub1.Data.mergeTo, (bub2.transform.position + bub1.transform.position) / 2, true, bub1.HP + bub2.HP);

        bub1.gameObject.SetActive(false);
        bub2.gameObject.SetActive(false);

        Destroy(bub1.gameObject);
        Destroy(bub2.gameObject);

        //bubblePool.Release(bub1.gameObject);
        //bubblePool.Release(bub2.gameObject);
  
        //Debug.Log("Creando una " + bub1.Data.mergeTo.thisType.ToString());
        //Debug.Log("Creando una " + bub2.Data.mergeTo.thisType.ToString());
    }

    #endregion

    #region Bullets
    void CreateBullet()
    {
        GameObject bullet = bulletPool.Get();

        bullet.transform.position = bulletSpawnArea.position;

        Bullet bul = bullet.GetComponent<Bullet>();
        bul.Initialize(bulletData);
        bul.OnRecycle += RecycleBullet;

        bullet.SetActive(true);
    }
    void RecycleBullet(Bullet bullet)
    {
        bullet.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        bullet.OnRecycle -= RecycleBullet;
        bullet.gameObject.SetActive(false);

        bulletPool.Release(bullet.gameObject);

        //Debug.Log("Bala Reseteada");
    }
    #endregion

    #region UI
    void ShowGameOver()
    {
        //Debug.Log("PERDISTE MANITO");
    }
    void StartGame()
    {
        StartCoroutine(AutoShoot(0.5f));
    }
    void PauseGame()
    {

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
            CreateBubble(largeBubbleData, bubbleSpawnArea.position, false, isNewBubble: true);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            CreateBubble(mediumBubbleData, bubbleSpawnArea.position, false, isNewBubble: true);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            CreateBubble(smallBubbleData, bubbleSpawnArea.position, false, isNewBubble: true);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CreateBullet();
        }


    }

    IEnumerator AutoShoot(float spareTime)
    {
        yield return new WaitForSeconds(spareTime);
        CreateBullet();
        StartCoroutine(AutoShoot(spareTime));
    }

    #endregion

}