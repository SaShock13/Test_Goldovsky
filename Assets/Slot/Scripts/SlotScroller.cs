using UnityEngine;
using System.Collections.Generic;
using AxGrid.Base;
using AxGrid.Model;

public class SlotScroller : MonoBehaviourExtBind
{
    [Header("Настройки слота")]
    public RectTransform contentRoot; // контейнер с элементами
    public float itemHeight = 100f;   // высота одного элемента
    public int itemCount = 5;          // сколько элементов в ряду
    public List<GameObject> items;     // список элементов (префабов)

    [Header("Скорость")]
    [SerializeField]
    private float currentSpeed = 0f;   // текущая скорость прокрутки
    private float targetSpeed = 0f;    // скорость, к которой разгоняемся

    private Vector2 startPositions;    // для расчета позиции
    private float totalHeight;         // высота всего контента

    [OnStart]
    void Init()
    {
        if (contentRoot == null)
        {
            Debug.LogError("SlotScroller: contentRoot не задан!");
            return;
        }

        // Инициализация totalHeight
        totalHeight = itemHeight * itemCount;

        // Расставляем элементы по вертикали
        for (int i = 0; i < items.Count; i++)
        {
            items[i].transform.SetParent(contentRoot, false);
            items[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -i * itemHeight);
        }
    }

    [OnUpdate]
    void UpdateScroll()
    {
        if (currentSpeed <= 0f) return;

        float delta = currentSpeed * Time.deltaTime;

        for (int i = 0; i < items.Count; i++)
        {
            RectTransform rt = items[i].GetComponent<RectTransform>();
            rt.anchoredPosition -= new Vector2(0, delta);

            // если элемент ушел вниз за нижнюю границу, переместить наверх
            if (rt.anchoredPosition.y < -totalHeight)
            {
                rt.anchoredPosition += new Vector2(0, totalHeight);
                // можно тут менять картинку/контент элемента
                Debug.Log("Item recycled: " + items[i].name);
            }
        }
    }

    // FSM вызывает эти методы для управления скоростью
    public void StartAccelerating(float speed)
    {
        targetSpeed = speed;
        Debug.Log("SlotScroller: StartAccelerating to " + speed);
    }

    public void StopSlot()
    {
        targetSpeed = 0f;
        Debug.Log("SlotScroller: StopSlot");
    }

    [OnUpdate]
    void ApplySpeed()
    {
        // Плавный разгон/замедление
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 3f);
    }
}