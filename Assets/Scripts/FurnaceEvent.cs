using UnityEngine;
using UnityEngine.UI;

public class FurnaceEvent : MonoBehaviour
{

    [Header("UI Elements")]
    public RectTransform indicator;
    public RectTransform bar;
    public RectTransform target;

    [Header("Settings")]
    bool movingRight = true;
    public float speed = 100f;
    public float barWidth;

    private float minX;
    private float maxX;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CalculateBounds();   
    }

    void CalculateBounds()
    {
        float halfBar = bar.rect.width / 2f;
        float halfIndicator = indicator.rect.width / 2f;

        // Ustalamy sztywne granice, których wskaźnik nie może przekroczyć
        maxX = halfBar - halfIndicator;
        minX = -maxX;
    }

    // Update is called once per frame
    void Update()
    {
        MoveIndicator();

        if (Input.GetKeyDown(KeyCode.E))
        {
            CheckSuccess();
        }
    }

    void MoveIndicator()
    {
        float currentX = indicator.anchoredPosition.x;

        if (movingRight)
        {
            currentX += speed * Time.deltaTime;
            if (currentX >= maxX)
            {
                currentX = maxX; // "Dobijamy" do krawędzi
                movingRight = false;
            }
        }
        else
        {
            currentX -= speed * Time.deltaTime;
            if (currentX <= minX)
            {
                currentX = minX; // "Dobijamy" do krawędzi
                movingRight = true;
            }
        }

        indicator.anchoredPosition = new Vector2(currentX, indicator.anchoredPosition.y);
    }

    void CheckSuccess()
    {
        // Pobieramy granice Target
        float targetPos = target.anchoredPosition.x;
        float targetHalfWidth = target.rect.width / 2f;

        float minX = targetPos - targetHalfWidth;
        float maxX = targetPos + targetHalfWidth;

        float indicatorX = indicator.anchoredPosition.x;

        if (indicatorX >= minX && indicatorX <= maxX)
        {
            Debug.Log("Success!");
            // TODO: sukces logika
        }
        else
        {
            // TODO: logika porażki
        }
    }

}
