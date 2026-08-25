using UnityEngine;

public class StageController : MonoBehaviour
{
    // 한 씬 안에 들어 있는 각 스테이지 루트 오브젝트입니다.
    // 씬을 여러 개로 나누지 않고 Stage01Root, Stage02Root, Stage03Root를 켜고 끄는 방식입니다.
    [SerializeField] private GameObject _homeStageRoot;
    [SerializeField] private GameObject _alleyStageRoot;
    [SerializeField] private GameObject _schoolGateStageRoot;

    // 현재 켜져 있는 스테이지 번호입니다.
    private int _currentStageIndex = 1;

    // 다른 스크립트가 현재 스테이지 번호를 읽을 때 사용합니다.
    public int CurrentStageIndex
    {
        get
        {
            return _currentStageIndex;
        }
    }

    // 현재 스테이지가 마지막 스테이지인지 알려줍니다.
    public bool IsLastStage
    {
        get
        {
            return _currentStageIndex >= 3;
        }
    }

    // 씬이 시작되면 기본으로 1스테이지를 켭니다.
    private void Awake()
    {
        ShowStage(1);
    }

    // 요청한 스테이지 하나만 켜고 나머지 스테이지는 끕니다.
    // Mathf.Clamp로 1보다 작거나 3보다 큰 값이 들어와도 1~3 범위 안에 묶습니다.
    public void ShowStage(int stageIndex)
    {
        _currentStageIndex = Mathf.Clamp(stageIndex, 1, 3);

        SetStageActive(_homeStageRoot, _currentStageIndex == 1);
        SetStageActive(_alleyStageRoot, _currentStageIndex == 2);
        SetStageActive(_schoolGateStageRoot, _currentStageIndex == 3);
    }

    // 현재 스테이지 번호에서 다음 스테이지로 넘어갑니다.
    public void ShowNextStage()
    {
        ShowStage(_currentStageIndex + 1);
    }

    // 스테이지 루트의 활성화 상태를 바꾸고, 켤 때는 안의 오브젝트를 다시 켭니다.
    private void SetStageActive(GameObject stageRoot, bool isActive)
    {
        if (stageRoot == null)
        {
            return;
        }

        if (isActive)
        {
            ResetObjects(stageRoot.transform);
        }

        stageRoot.SetActive(isActive);
    }

    // 이전 플레이에서 꺼졌던 아이템과 장애물 등을 다시 켜기 위해 자식들을 재귀적(함수가 자기 자신을 다시 부르는 것)으로 켭니다.
    // 아이템을 먹거나 실드로 장애물을 막으면 오브젝트가 꺼지므로, 재시작 때 다시 보이게 만드는 역할입니다.
    private void ResetObjects(Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            Transform child = parent.GetChild(i);
            child.gameObject.SetActive(true);
            ResetObjects(child);
        }
    }
}
