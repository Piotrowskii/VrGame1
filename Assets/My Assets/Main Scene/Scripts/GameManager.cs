using NaughtyAttributes;
using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] public MonsterManager MonsterManager;

    public TimeOfDay CurrentTimeOfDay { get; set; } = TimeOfDay.Day;
    public UnityEvent<TimeOfDay> OnTimeOfDayChange = new UnityEvent<TimeOfDay>();

    [Header("Tower")]
    [SerializeField] public GameObject Tower;
    public float PlayAreaRadius = 27;


    [Header("Terrain")]
    [SerializeField] private bool enableTestTerrain = false;
    [SerializeField] private Terrain mainTerrain;
    [SerializeField] private GameObject testTerrain;

    public Vector3 startPlayerPosition = new Vector3(40f,3.49f,40f);

    private void Awake()
    {
        Instance = this;

        if (enableTestTerrain)
        {
            mainTerrain.enabled = false;
            testTerrain.SetActive(true);
        }
        else
        {
            mainTerrain.enabled = true;
            testTerrain.SetActive(false);
        }
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
