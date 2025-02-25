using UnityEngine;

public class BallController : MonoBehaviour
{
    [SerializeField] private GameObject smallBallPrefab;
    [SerializeField] private GameObject mediumBallPrefab;
    [SerializeField] private GameObject largeBallPrefab;
    [SerializeField] private BallSize currentSize;
    private enum BallSize { Small, Medium, Large }

    void Update()
    {
        // Dividir una bola grande en dos medianas (tecla izquierda)
        if (Input.GetKeyDown(KeyCode.LeftArrow) && currentSize == BallSize.Large)
        {
            DivideBall(BallSize.Medium);
        }

        // Dividir una bola mediana en dos pequeñas (tecla derecha)
        if (Input.GetKeyDown(KeyCode.RightArrow) && currentSize == BallSize.Medium)
        {
            DivideBall(BallSize.Small);
        }

        // Unir dos bolas pequeñas en una mediana (tecla arriba)
        if (Input.GetKeyDown(KeyCode.UpArrow) && currentSize == BallSize.Small)
        {
            CombineBalls(BallSize.Medium);
        }

        // Unir dos bolas medianas en una grande (tecla abajo)
        if (Input.GetKeyDown(KeyCode.DownArrow) && currentSize == BallSize.Medium)
        {
            CombineBalls(BallSize.Large);
        }
    }

    void DivideBall(BallSize newSize)
    {
        // Destruir la bola actual
        Destroy(gameObject);

        // Crear dos nuevas bolas del tamaño especificado
        CreateBall(newSize);
        CreateBall(newSize);
    }

    void CombineBalls(BallSize newSize)
    {
        // Aquí necesitas lógica para encontrar otra bola del mismo tamaño
        // y luego combinarlas en una nueva bola del tamaño especificado.
        // Esto puede requerir un sistema de gestión de bolas en la escena.

        // Ejemplo simplificado:
        GameObject[] balls = GameObject.FindGameObjectsWithTag("Ball");
        foreach (GameObject ball in balls)
        {
            if (ball != gameObject && ball.GetComponent<BallController>().currentSize == currentSize)
            {
                // Destruir ambas bolas
                Destroy(ball);
                Destroy(gameObject);

                // Crear una nueva bola del tamaño especificado
                CreateBall(newSize);
                break;
            }
        }
    }

    void CreateBall(BallSize size)
    {
        GameObject go;

        switch (size)
        {
            case BallSize.Small:
                go = Instantiate(smallBallPrefab);
                break;
            case BallSize.Medium:
                go = Instantiate(mediumBallPrefab);
                break;
            case BallSize.Large:
                go = Instantiate(largeBallPrefab);
                break;
            default:
                break;
        }
    }


}