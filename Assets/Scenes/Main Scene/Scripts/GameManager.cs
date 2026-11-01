using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public DayManager DayManager {  get; private set; }
    public NightManager NightManager { get; private set; }


    public TimeOfDay CurrentTimeOfDay { get; set; } = TimeOfDay.Day;
    public UnityEvent<TimeOfDay> OnTimeOfDayChange = new UnityEvent<TimeOfDay>();

    [SerializeField]
    [Required]
    public GameObject campfire;

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
