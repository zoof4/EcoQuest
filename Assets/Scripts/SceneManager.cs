using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    /// <summary>
    /// 새로운 맵 씬을 로드하고 MapManager를 업데이트하는 함수
    /// 이 함수는 다른 스크립트(예: UI 버튼, 게임 매니저)에서 호출
    /// </summary>
    /// <param name="mapName">로드할 씬의 이름</param>
    public void LoadMap(string mapName)
    {
        // 1. SceneManager에 씬 이름이 등록되어 있는지 먼저 확인
        //씬 이름 오타로 인한 오류 방지
        if (!IsSceneInBuildSettings(mapName))
        {
            Debug.LogError($"씬 '{mapName}'이 빌드 설정에 등록되어 있지 않습니다. 빌드 설정(File > Build Settings)을 확인해주세요.");
            return;
        }

        // 2. 새로운 씬을 로드하기 전 MapManager에 현재 씬의 이름을 전달
        // 이 과정이 플레이어 컨트롤러가 맵 타입을 올바르게 인식하도록 보장
        if (MapManager.Instance != null)
        {
            MapManager.Instance.SetMap(mapName);
        }
        else
        {
            Debug.LogError("MapManager 인스턴스를 찾을 수 없습니다. MapManager 오브젝트가 씬에 있는지 확인해주세요.");
            return;
        }

        // 3. 비동기 방식으로 씬을 로드 (로딩 화면 등을 구현할 때 유용)
        SceneManager.LoadSceneAsync(mapName);
    }
    
    // UI 버튼 등에 연결하여 소코반 맵을 로드하는 예시 함수
    public void LoadSokobanMap()
    {
        // 하드코딩된 맵 이름 "M_PLU_02"를 사용
        LoadMap("M_PLU_02");
    }

    // 씬이 빌드 설정에 등록되어 있는지 확인하는 보조 함수
    private bool IsSceneInBuildSettings(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            if (name == sceneName)
            {
                return true;
            }
        }
        return false;
    }
}