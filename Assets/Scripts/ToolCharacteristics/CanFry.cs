using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanFry : MonoBehaviour {

    public bool canFry = false;
    GameObject firstCollidedObject;
    [SerializeField]
    bool hasFluid;

    private void OnCollisionEnter(Collision collision)
    {
        if (!canFry && collision.transform.parent)
        {
            firstCollidedObject = collision.collider.transform.parent.gameObject;
            canFry = OnOven(collision.collider.transform.parent);
        }
    }

    bool OnOven(Transform parent)
    {
        bool isPlate = false;
        while (parent != null)
        {
            if (parent.GetComponent<HotPlateOven>())
            {
                HotPlateOven hotPlateOven = parent.GetComponent<HotPlateOven>();
                for (int i = 0; i < hotPlateOven.plates.Length; i++)
                {
                    if (hotPlateOven.plates[i].name == firstCollidedObject.name && hotPlateOven.activePlateIndices[i])
                    {
                        isPlate = true;
                        return isPlate;
                    }
                }
                
            }
            parent = parent.transform.parent;
        }
        return isPlate;
    }

    public bool GetCanFry()
    {
        return canFry;
    }

    public void SetHasFluid(bool flag)
    {
        hasFluid = flag;
    }

    public bool GetHasFluid()
    {
        return hasFluid;
    }
}
