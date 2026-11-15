using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] public MonsterManager MonsterManager;

    public TimeOfDay CurrentTimeOfDay { get; set; } = TimeOfDay.Day;
    public UnityEvent<TimeOfDay> OnTimeOfDayChange = new UnityEvent<TimeOfDay>();

    [SerializeField]
    [Required]
    public GameObject tower;
    public float playAreaRadius = 27;

    private void Awake()
    {
        instance = this;
    }

    public void ChangeTimeToDay()
    {
        CurrentTimeOfDay = TimeOfDay.Day;
        OnTimeOfDayChange.Invoke(CurrentTimeOfDay);
    }

    public void ChangeTimeToNight()
    {
        CurrentTimeOfDay = TimeOfDay.Night;
        OnTimeOfDayChange.Invoke(CurrentTimeOfDay);
    }
}
