using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanPutIn : MonoBehaviour
{
    public GameObject parentObj;
    Collider gameObjectCol, parentObjCol;

    void Start()
    {
        if (gameObject != null)
            gameObjectCol = gameObject.GetComponent<Collider>();
        if (parentObj != null)
            parentObjCol = parentObj.GetComponent<Collider>();
       
    }

    void Update()
    {
        if (parentObjCol.bounds.Intersects(gameObjectCol.bounds))
        {
            gameObject.transform.parent = parentObj.transform;
        }
        else
        {
            gameObject.transform.parent = null;
        }
    }
}