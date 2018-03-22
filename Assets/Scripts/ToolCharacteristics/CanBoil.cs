using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBoil : ToolCharacteristic {

    public bool canBoil = false;
    public static int waterBoilingTime = 6;
    [SerializeField]
    bool hasFluid = false;

    public AudioClip boilingSound;
    AudioSource source;
    bool onlyOnce = true;

    public bool isWaterBoiled;

    private void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
        source.loop = true;
        source.clip = boilingSound;
    }

    private void Update()
    {
        if(hasFluid && canBoil)
        {
            StartWaterBoiling();
        }
        else
        {
            StopWaterBoiling();
        }
    }

    IEnumerator WaterBoil()
    {
        yield return new WaitForSeconds(waterBoilingTime);
        isWaterBoiled = true;
        // start sound
        if (onlyOnce)
        {
            source.loop = true;
            source.Play();
            onlyOnce = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!canBoil && collision.transform.parent)
        {
            canBoil = OnOven(collision.collider.transform.parent.gameObject, collision.collider.transform.parent.transform.parent);
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
        return isPlate;
    }

    public void SetHasFluid(bool flag)
    {
        hasFluid = flag;
    }

    private void StartWaterBoiling()
    {
        StartCoroutine("WaterBoil");
    }

    void StopWaterBoiling()
    {
        if (!isWaterBoiled)
        {
            StopCoroutine("WaterBoil");
        }
        source.loop = false;
        source.Stop();
        onlyOnce = true;
    }

    public bool GetCanBoil()
    {
        return canBoil;
    }
}
