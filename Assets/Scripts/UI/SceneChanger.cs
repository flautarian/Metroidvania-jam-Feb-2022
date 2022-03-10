using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    [SerializeField]
    private string sceneName;

    [SerializeField]
    private Vector2 nextPlayerPosition;

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.tag.Equals(Constants.TAG_PLAYER))
            GameManager.Instance.ChangeScene(sceneName, nextPlayerPosition);
    }
}
