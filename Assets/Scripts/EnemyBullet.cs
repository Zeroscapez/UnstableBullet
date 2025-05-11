using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : BulletBase
{
    public bool hasHitPlayer;
    public bool hasBeenGrazed;

    new public void ReturnToPool()
    {
        
        base.ReturnToPool();
    }
    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("PlayerHitbox"))
        {
            // Handle enemy bullet hitting the player
     
            hasHitPlayer = true;
            PlayerScoreManager.Instance.AddScore(-100);
            ReturnToPool();
        }
    }
}
