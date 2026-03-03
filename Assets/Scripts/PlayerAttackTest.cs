using UnityEngine;

public class PlayerAttackTest : MonoBehaviour
{
    public AttackHitbox hitbox;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            // 0.1 sn hitbox açık kalsın
            hitbox.EnableHitbox();
            Invoke(nameof(StopHit), 0.1f);
        }
    }

    void StopHit()
    {
        hitbox.DisableHitbox();
    }
}