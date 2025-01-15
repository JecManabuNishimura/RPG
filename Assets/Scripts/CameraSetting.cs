using Cinemachine;
using UnityEngine;

public class CameraSetting : MonoBehaviour
{
    void Start()
    {
        GameManager.Instance.cCam = GetComponent<CinemachineVirtualCamera>();
    }
}
