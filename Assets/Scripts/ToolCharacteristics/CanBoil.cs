using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBoil : FoodCharacteristic {

    public bool canBoil = false;

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {
        /*if (canBoil)
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<MeshCollider>().convex = false;
        }*/
        /*meshCollider = gameObject.AddComponent<MeshCollider>() as MeshCollider;
        meshCollider.cookingOptions = MeshColliderCookingOptions.InflateConvexMesh;
        meshCollider.skinWidth = 0.01f;
        meshCollider.convex = true;*/
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!canBoil && collision.transform.parent)
        {
            //Debug.Log("enter " + collision.collider.name);
            canBoil = OnOven(collision.collider.transform.parent.gameObject, collision.collider.transform.parent.transform.parent);
        }
        if (canBoil)
        {
            Debug.Log("enter " + collision.collider.name);
        }
    }

    bool OnOven(GameObject plate, Transform parent)
    {
        bool isPlate = false;
        if (parent != null)
        {
            if (parent.GetComponent<HotPlateOven>())
            {
                HotPlateOven hotPlateOven = parent.GetComponent<HotPlateOven>();
                for (int i = 0; i < hotPlateOven.plates.Length; i++)
                {
                    if (hotPlateOven.plates[i].name == plate.name && hotPlateOven.activePlateIndices[i])
                    {
                        isPlate = true;
                        return isPlate;
                    }
                }

            }
        }
        Debug.Log("onOven: " + isPlate);
        return isPlate;
    }

}
