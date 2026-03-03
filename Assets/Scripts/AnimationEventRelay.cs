using UnityEngine;

public class AnimationEventRelay : MonoBehaviour
{
    public AttackHitbox hitbox;

public void HitboxOn()
{
    Debug.Log("HITBOX ON (event)");
    if (hitbox != null) hitbox.EnableHitbox();
}

public void HitboxOff()
{
    Debug.Log("HITBOX OFF (event)");
    if (hitbox != null) hitbox.DisableHitbox();
}
    
}