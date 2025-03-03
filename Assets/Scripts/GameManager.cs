using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
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
    private int smallBubbleCount;

    private Coroutine autoShootCoroutine;

    public Action OnLevelWin;
    public Action OnLevelLose;

    private bool isPaused = false;
    public bool IsPaused { get { return isPaused; } }
    private void Start()
    {
        level = 1;
        bulletDamage = 1;
        smallBubbleCount = 4;

        isPaused = false;

        bubblePool = new ObjectPool<GameObject>
            (
                createFunc: () => Instantiate(bubblePrefab, Vector3.zero, Quaternion.identity, bubbleParent),
                initialSize: 1
            );

        bulletPool = new ObjectPool<GameObject>
            (
                createFunc: () => Instantiate(bulletPrefab, Vector3.zero, bulletPrefab.transform.rotation, bulletParent),
                initialSize: 4
            );

        autoShootCoroutine = StartCoroutine(AutoShoot(0.33f));
    }

    #region Bubbles
    private GameObject CreateBubble(BubbleData data, Vector3 pos, bool isMerged, int hp = 0, bool isNewBubble = false)
    {
        GameObject bubble = bubblePool.Get();
        //GameObject bubble = Instantiate(bubblePrefab, Vector3.zero, Quaternion.identity, bubbleParent);

        bubble.transform.SetPositionAndRotation(pos, Quaternion.identity);

        Bubble bubbleComponent = bubble.GetComponent<Bubble>();
        if (bubbleComponent == null)
            bubbleComponent = bubble.AddComponent<Bubble>();

        bubbleComponent.Initialize(data, isMerged ? hp : (int)(data.hpFactor * (level + data.baseHP)), isNewBubble);

        bubbleComponent.OnCollisionWithPair += MergeBubbles;
        bubbleComponent.OnCollisionWithBullet += DamageBubble;
        bubbleComponent.OnCollisionWithPlayer += ShowGameOver;

        bubble.SetActive(true);

        return bubble;
    }

    private void DamageBubble(BubbleBulletCollision col)
    {
        if (col.bubble.HP - bulletDamage <= 0)
        {
            PopBubble(col.bubble);
        }
        else
        {
            col.bubble.GetDamage(bulletDamage);
        }

        RecycleBullet(col.bullet);
    }

    private void PopBubble(Bubble bubble)
    {
        if (bubble == null)
            return;

        BubbleData divideInData = bubble.Data.divideIn;

        Bounds bounds = bubble.gameObject.GetComponent<SpriteRenderer>().bounds;

        Vector3 pos1 = new Vector3(bounds.min.x, bounds.max.y, 0);
        Vector3 pos2 = new Vector3(bounds.max.x, bounds.max.y, 0);

        if (divideInData == null)
        {
            RecycleBubble(bubble.gameObject);
            smallBubbleCount--;
            if (smallBubbleCount <= 0)
                OnLevelWin?.Invoke();
            return;
        }

        CreateBubble(divideInData, pos1, false);
        CreateBubble(divideInData, pos2, false);

        RecycleBubble(bubble.gameObject);
    }

    private void MergeBubbles(BubbleBubbleCollision col)
    {
        Bubble bub1 = col.From;
        Bubble bub2 = col.To;

        if (bub1.Data.mergeTo == null || bub2.Data.mergeTo == null || bub1.Data.mergeTo.thisType != bub2.Data.mergeTo.thisType)
        {
            return;
        }

        CreateBubble(bub1.Data.mergeTo, (bub2.transform.position + bub1.transform.position) / 2, true, bub1.HP + bub2.HP);

        RecycleBubble(bub2.gameObject);
        RecycleBubble(bub1.gameObject);
    }

    private void RecycleBubble(GameObject bubble)
    {
        bubble.name = "Recicled Bubble";
        Bubble bubbleComponent = bubble.GetComponent<Bubble>();

        if (bubbleComponent != null)
        {
            // Limpiar eventos suscritos
            bubbleComponent.OnCollisionWithPair = null;
            bubbleComponent.OnCollisionWithBullet = null;
            bubbleComponent.OnCollisionWithPlayer = null;

            // Reiniciar propiedades
            bubbleComponent.gameObject.SetActive(false);
            bubbleComponent.transform.position = Vector3.zero;
            bubbleComponent.transform.localScale = Vector3.one;
            bubbleComponent.transform.rotation = Quaternion.identity;
            bubbleComponent.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            bubbleComponent.GetComponent<CircleCollider2D>().enabled = false;
            bubbleComponent.ResetData();
        }

        bubblePool.Release(bubble);
        //Destroy(bub1.gameObject);
    }

    #endregion

    #region Bullets
    void CreateBullet()
    {
        GameObject bullet = bulletPool.Get();

        bullet.transform.position = bulletSpawnArea.position;

        Bullet bul = bullet.GetComponent<Bullet>();
        if (bul == null)
            bul = bullet.AddComponent<Bullet>();

        bul.Initialize(bulletData);
        bul.OnRecycle += RecycleBullet;

        bullet.name = "Bullet <->";

        bullet.SetActive(true);
    }
    void RecycleBullet(Bullet bullet)
    {
        bullet.name = "Recicled Bullet";

        bullet.gameObject.GetComponent<BoxCollider2D>().enabled = false;
        bullet.OnRecycle = null;
        bullet.gameObject.SetActive(false);

        bulletPool.Release(bullet.gameObject);
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }

    }

    IEnumerator AutoShoot(float spareTime)
    {
        yield return new WaitForSeconds(spareTime);
        CreateBullet();
        StartCoroutine(AutoShoot(spareTime));
    }

    public void PauseGame()
    {
        isPaused = !isPaused;

        Time.timeScale = !isPaused ? 1.0f : 0.0f;

        if (autoShootCoroutine != null)
        {
            StopCoroutine(autoShootCoroutine);
        }
    }
    #endregion

}