using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Inspector에서 따라갈 대상과 카메라 위치 보정값을 설정합니다.
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset = new Vector3(3f, 3.3f, -10f);
    [SerializeField] private float _minX = 3.7f;
    [SerializeField] private float _maxX = 47f;

    // 플레이어 이동이 끝난 뒤 카메라 위치를 따라가도록 LateUpdate에서 처리합니다.
    // x 위치는 플레이어를 따라가지만, Mathf.Clamp로 카메라가 정해진 범위 밖으로 나가지 않게 막습니다.
    private void LateUpdate()
    {
        if (_target == null)
        {
            return;
        }

        Vector3 nextPosition = _target.position + _offset;
        nextPosition.x = Mathf.Clamp(nextPosition.x, _minX, _maxX);
        nextPosition.y = transform.position.y;
        nextPosition.z = -10f;

        transform.position = nextPosition;
    }
}


