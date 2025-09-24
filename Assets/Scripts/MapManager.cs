using UnityEngine;
using UnityEngine.SceneManagement;

public enum MapType
{
    Normal,
    Sokoban,
    CardBattle,
    Puzzle
}

public class MapManager : MonoBehaviour
{
    public static MapManager Instance
    {
        get;
        private set;
    }
    // 현재 맵의 타입과 이름을 관리
    public MapType currentMapType;
    public string currentMapName;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SetMap(scene.name);
    }
    // <summary>
    // 맵 이름을 기반으로 맵 타입을 설정하는 함수.
    // </summary>
    // <param name="mapName">현재 로드된 맵 이름</param>   
    // public void SetMap(string mapName)
    // {
    //     currentMapName = mapName;

    //     // 맵 이름에 따라 맵 타입을 결정하는 로직
    //     if (mapName == "M_PLU_02")
    //     {
    //         currentMapType = MapType.Sokoban;
    //     }
    //     else
    //     {
    //         currentMapType = MapType.Normal;
    //     }

    //     Debug.Log($"맵 이름: {currentMapName}, 타입: {currentMapType}으로 설정되었습니다.");
    // }

    // 이전 코드와 달리 씬 이름을 기반으로 맵 타입을 결정
    public void SetMap(string mapName)
    {
        currentMapName = mapName;

        // 씬 내의 MapData 오브젝트를 찾아 맵 타입 설정
       MapData mapData = FindFirstObjectByType<MapData>();
        if (mapData != null)
        {
            currentMapType = mapData.mapType;
        }
        else
        {
            // MapData가 없는 경우, 기본값(Normal)으로 설정
            currentMapType = MapType.Normal;
        }
        Debug.Log($"맵 이름 : {currentMapName}, 타입 : {currentMapType}으로 설정되었습니다.");
    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}