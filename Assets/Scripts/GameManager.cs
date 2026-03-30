using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI targetNameText;

    [Header("Names")]
    public string[] names = new string[]
    {
        "Алиса", "Борис", "Вера", "Глеб", "Дарья",
        "Егор", "Жанна", "Иван", "Катя", "Лёша",
        "Маша", "Никита", "Оля", "Петя", "Света"
    };

    [Header("Colors")]
    public Color[] palette = new Color[]
    {
        new Color(1f, 0.2f, 0.2f),
        new Color(0.2f, 0.8f, 0.2f),
        new Color(0.2f, 0.6f, 1f),
        new Color(1f, 0.8f, 0.1f),
        new Color(0.9f, 0.3f, 1f),
        new Color(0.1f, 0.9f, 0.9f),
        new Color(1f, 0.5f, 0.1f),
    };

    public string CurrentTargetName { get; private set; }
    public Color CurrentTargetColor { get; private set; }

    // Дом, который является текущей целью
    public HouseMover TargetHouse { get; private set; }

    private int colorIndex = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        PickNewTarget();
    }

    public void PickNewTarget()
    {
        CurrentTargetName = names[Random.Range(0, names.Length)];
        CurrentTargetColor = palette[colorIndex % palette.Length];
        colorIndex++;
        TargetHouse = null;

        if (targetNameText != null)
        {
            targetNameText.text = "Для: " + CurrentTargetName;
            targetNameText.color = CurrentTargetColor;
        }
    }

    public void RegisterHouse(HouseMover house)
    {
        // Назначаем новый целевой дом только если его ещё нет
        if (TargetHouse == null || !TargetHouse.IsTarget)
        {
            TargetHouse = house;
            house.SetAsTarget(CurrentTargetColor);
        }
    }

    public void UnregisterHouse(HouseMover house)
    {
        if (TargetHouse == house)
            TargetHouse = null;
    }

    public void OnGiftDelivered()
    {
        PickNewTarget();
    }
}
