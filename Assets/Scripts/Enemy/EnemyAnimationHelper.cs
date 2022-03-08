using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimationHelper : MonoBehaviour
{
    EnemyController enemyParent;
    void Start()
    {
        var enemyP = transform.parent.gameObject;
        if(enemyP.TryGetComponent<EnemyController>(out EnemyController enemy)){
            enemyParent = enemy;
        }
    }

    public void StartShot(){
        enemyParent?.Shot();
    }
}
