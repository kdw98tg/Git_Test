using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField]
    private GameObject itemPrefab = null;

    // 아이템이 생성될 발판들 (여러 개면 그중 하나를 무작위로 고름)
    [SerializeField]
    private Collider[] platforms = null;

    [SerializeField]
    private int maxItemCount = 3;
    [SerializeField]
    private float spawnInterval = 5f;
    // 발판 표면에서 얼마나 띄워서 생성할지
    [SerializeField]
    private float heightOffset = 0.5f;

    private List<Item> itemList = null;

    private void Awake()
    {
        itemList = new List<Item>();
    }

    public void Init()
    {
        StartCoroutine(CoSpawnItems());
    }

    private IEnumerator CoSpawnItems()
    {
        while (true)
        {
            if (itemList.Count < maxItemCount)
            {
                SpawnItem();
            }
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnItem()
    {
        if (TryGetRandomPointOnPlatform(out Vector3 spawnPosition) == false)
        {
            return;
        }

        GameObject go
            = Instantiate(
                itemPrefab,
                spawnPosition,
                Quaternion.identity,
                this.transform);

        Item itemComponent
            = go.GetComponent<Item>();
        if (itemComponent != null)
        {
            itemComponent.Init(OnItemPicked);
            itemList.Add(itemComponent);
        }
    }

    private bool TryGetRandomPointOnPlatform(out Vector3 point)
    {
        point = Vector3.zero;
        if (platforms == null || platforms.Length == 0)
        {
            return false;
        }

        Collider platform = platforms[Random.Range(0, platforms.Length)];
        Bounds bounds = platform.bounds;

        // 발판 범위 안의 무작위 XZ 위치에서 아래로 레이를 쏴 실제 표면을 찾음
        float x = Random.Range(bounds.min.x, bounds.max.x);
        float z = Random.Range(bounds.min.z, bounds.max.z);
        Vector3 origin = new Vector3(x, bounds.max.y + 10f, z);
        Ray ray = new Ray(origin, Vector3.down);

        if (platform.Raycast(ray, out RaycastHit hit, bounds.size.y + 20f))
        {
            point = hit.point + Vector3.up * heightOffset;
            return true;
        }
        return false;
    }

    private void OnItemPicked(Item item)
    {
        itemList.Remove(item);
    }
}
