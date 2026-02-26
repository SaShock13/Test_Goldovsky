using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using AxGrid;
using AxGrid.Base;
using AxGrid.Model;

public class SimpleInfiniteScroll : MonoBehaviourExtBind
{
    [Header("Ссылки")]
    public RectTransform contentRoot;      
    public List<RectTransform> items;       // Список элементов 
    public VfxManager vfx;

    [Header("Настройки")]
    public float itemHeight = 200f;         // Высота одного элемента
    public float scrollSpeed = 100f;        // Скорость прокрутки
    public float acceleration = 400f;      // Скорость ускорения
    public float decelerationRate = 5f;     // Коэффициент замедления
    public float snapSpeed = 15f;           // Скорость доворота до сетки
    public float stopThreshold = 1f;        // Порог полной остановки в пикселях
    public float recycleBuffer = 0f;        // буфер для страховки

    [Header("Настройки остановки")]
    public float snapActivationSpeed = 5f;   // Скорость, при которой начинается снап 
    
    private bool isSnapping = false;
    private string spinMode;

    private float currentScrollY = 0f;
    private float targetScrollY = 0f;
    private float currentSpeed = 0f;
    private float speedWhenStopped = 0f;     // Запоминаем скорость в момент нажатия Stop
    private float currentSnapSpeed = 0f;     // Текущая скорость во время снапа
    private float snapDeceleration = 0f;     // Скорость замедление снапа
    private const float MinSnapSpeed = 30f;  // Минимальная скорость для старта снапа

    [OnStart]
    void Init()
    {
        
        // Выравниваем элементы в столбик при старте
        for (int i = 0; i < items.Count; i++)
        {
            items[i].anchoredPosition = new Vector2(0, -i * itemHeight);
            items[i].sizeDelta = new Vector2(items[i].sizeDelta.x, itemHeight);
        }

        currentScrollY = 0f;
        contentRoot.anchoredPosition = Vector2.zero;
        recycleBuffer = itemHeight; // буфер в 1 высоту элемента
        currentSpeed = 0;
    }

    [OnUpdate]
    void ScrollUpdate()
    {
        switch (spinMode)
        {
            case "Accelerating":
            case "Spinning":
                UpdateScrolling();
                break;
            case "Stopping":
                UpdateInertia();
                break;
            
        }

        CheckRecycle();
    }

    [Bind("OnSpinModeChanged")]
    private void OnSpinModeChanged(string value)
    {
        spinMode = value;
        // старт с нуля, чтобы плавно разгоняться
        if (value == "Accelerating" )
            currentSpeed = 0f;

        if (value == "Stopping")
            speedWhenStopped = currentSpeed;
        Debug.Log($"SpinMode changed: {value}");
    }

    // Скролл с плавным ускорением
    void UpdateScrolling()
    {
        currentSpeed = Mathf.MoveTowards(currentSpeed, scrollSpeed, acceleration * Time.deltaTime);
        currentScrollY -= currentSpeed * Time.deltaTime;
        contentRoot.anchoredPosition = new Vector2(0, currentScrollY);
    }

    // Фаза инерции
    void UpdateInertia()
    {
        if(isSnapping)
        {
            UpdateSnapping();
            return;
        }       
        currentSpeed = Mathf.Lerp(currentSpeed, 0f, decelerationRate * Time.deltaTime);
        currentScrollY -= currentSpeed * Time.deltaTime;
        contentRoot.anchoredPosition = new Vector2(0, currentScrollY);

        // пора ли переходить к снапу
        if (Mathf.Abs(currentSpeed) <= snapActivationSpeed)
        {
            // начинаем снап
            PrepareForSnap();
            //Debug.Log($"Speed threshold reached: {currentSpeed} <= {snapActivationSpeed}. Starting snap...");
        }
    }

    // Фаза снапа
    void UpdateSnapping()
    {
        float distance = targetScrollY - currentScrollY;
        float distanceAbs = Mathf.Abs(distance);

        if (distanceAbs < stopThreshold || currentSnapSpeed <= 0f)
        {
            CompleteStop();
            return;
        }

        // Уменьшаем скорость равномерно
        currentSnapSpeed = Mathf.MoveTowards(currentSnapSpeed, 0f, snapDeceleration * Time.deltaTime);

        float delta = Mathf.Sign(distance) * currentSnapSpeed * Time.deltaTime;

        if (Mathf.Abs(delta) >= distanceAbs)
        {
            currentScrollY = targetScrollY;
            contentRoot.anchoredPosition = new Vector2(0, currentScrollY);
            CompleteStop();
            return;
        }

        currentScrollY += delta;
        contentRoot.anchoredPosition = new Vector2(0, currentScrollY);
    }

    // Начальная скорость = последняя скорость инерции
    void PrepareForSnap()
    {
        isSnapping = true;
        CalculateSnapTarget();

        float distanceAbs = Mathf.Abs(targetScrollY - currentScrollY);

        if (distanceAbs < stopThreshold)
        {
            CompleteStop();
            return;
        }

        float startSpeedAbs = Mathf.Max(Mathf.Abs(currentSpeed), MinSnapSpeed);

        // Линейное затухание
        currentSnapSpeed = startSpeedAbs;
        snapDeceleration = (startSpeedAbs * startSpeedAbs) / (2f * Mathf.Max(distanceAbs, 0.0001f));
    }

    // Полная остановка
    void CompleteStop()
    {
        currentScrollY = targetScrollY;
        contentRoot.anchoredPosition = new Vector2(0, currentScrollY);
        currentSpeed = 0f;
        currentSnapSpeed = 0f;
        snapDeceleration = 0f;
        isSnapping = false;
        vfx.PlayFX();
        Settings.Invoke("OnSnaped");
        Debug.Log($"Snap complete at position: {currentScrollY}");
    }

    // Вычисление ближайшей позиции кратной itemHeight
    void CalculateSnapTarget()
    {        
        if (speedWhenStopped > 0) // Движение вниз в момент остановки
        {
            float steps = Mathf.Floor(currentScrollY / itemHeight);
            targetScrollY = steps * itemHeight;
            Debug.Log($"Moving down - snapping to floor: {targetScrollY}");
        }
    }

    // Проверка и переброс элементов
    void CheckRecycle()
    {
        if(spinMode == "Iddle") return;

        float viewportHeight = GetViewportHeight();
        float contentY = contentRoot.anchoredPosition.y;

        float viewportTop = 0f;
        float viewportBottom = -viewportHeight;

        foreach (var item in items)
        {
            float itemWorldY = contentY + item.anchoredPosition.y;

            // Ушёл вниз
            if (itemWorldY < viewportBottom - recycleBuffer)
            {
                MoveItemToTop(item);
            }
            // Ушёл вверх
            else if (itemWorldY > viewportTop + recycleBuffer)
            {
                MoveItemToBottom(item);
            }
        }
    }

    // переброс элемента в начало
    void MoveItemToTop(RectTransform item)
    {
        float highestY = float.MinValue;
        foreach (var i in items)
        {
            if (i.anchoredPosition.y > highestY)
                highestY = i.anchoredPosition.y;
        }
        item.anchoredPosition = new Vector2(0, highestY + itemHeight);
    }

    // переброс элемента в конец 
    void MoveItemToBottom(RectTransform item)
    {
        float lowestY = float.MaxValue;
        foreach (var i in items)
        {
            if (i.anchoredPosition.y < lowestY)
                lowestY = i.anchoredPosition.y;
        }
        item.anchoredPosition = new Vector2(0, lowestY - itemHeight);
    }

    // Получение высоты Viewport
    float GetViewportHeight()
    {
        if (contentRoot.parent is RectTransform rt)
            return rt.rect.height;
        return Screen.height;
    }

    
}