using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectsPool : MonoBehaviour
{
    internal GameObject prefab;
    internal Queue<GameObject> objects = new Queue<GameObject>(); 
 
   public GameObject Get()
   {
       if (objects.Count == 0)
           AddObjects(1);
       return objects.Dequeue();
   }
 
   public void ReturnToPool(GameObject objectToReturn)
   {
       objectToReturn.SetActive(false);
       objects.Enqueue(objectToReturn);
   }
   private void AddObjects(int count)
   {
       var newObject = GameObject.Instantiate(prefab);
       newObject.transform.parent = prefab.transform.parent;
       newObject.SetActive(false);
       objects.Enqueue(newObject);
   }
}
