
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class MapSpawnManager : MonoBehaviour
{
    [SerializeField] private Transform player;

    private void Awake()
    {
        string id = SceneTransition.NextSpawnPointId;

        if (string.IsNullOrEmpty(id))
            return;

        var points = FindObjectsByType<MapSpawnPoint>(
            FindObjectsSortMode.None);

        foreach (var point in points)
        {
            if (point.SpawnPointId != id)
                continue;

            // プレイヤー位置を変更
            player.position = point.transform.position;

            // Repositoryにも反映
            PlayerDataRepository.Instance.playerPos =
                point.transform.position;

            SceneTransition.NextSpawnPointId = null;
            return;
        }

        Debug.LogWarning(
            $"出現地点 {id} が見つかりません");
    }
}
