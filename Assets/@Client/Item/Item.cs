using System;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField]
    private float healAmount = 30f;

    private Action<Item> onItemPicked = null;

    private void Awake()
    {
        // 플레이어와 부딪히지 않고 통과하면서 감지되도록 트리거로 설정
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    public void Init(Action<Item> onItemPicked)
    {
        this.onItemPicked = onItemPicked;
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player == null)
        {
            return;
        }

        player.Heal(healAmount);
        onItemPicked?.Invoke(this);
        Destroy(gameObject);
    }
}
