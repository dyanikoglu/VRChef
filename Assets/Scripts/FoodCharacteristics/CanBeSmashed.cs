using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBeSmashed : FoodCharacteristic {
    public GameObject smashable;
    public GameObject smashed;
    public AudioClip smashSound;
    private int smashCount;
    GameObject _smashed;
    AudioSource source;
    bool isHalfSmashedAll = true;
    public bool iscollidedWithSmasher = false;
    public bool isCollidedWithBowl = false;
    public GameObject bowl;
    //public GameObject initialFood;

    void Start()
    {
        base.Start();
        smashCount = 0;
        source = gameObject.AddComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision col)
    {
        if (!iscollidedWithSmasher && col.gameObject.GetComponent<CanSmash>() != null)
        {
            iscollidedWithSmasher = true;
        }
        if (!isCollidedWithBowl && col.gameObject.GetComponent<CanMixedIn>() != null)
        {
            isCollidedWithBowl = true;
            bowl = col.gameObject;
        }
        
        if (col.gameObject.GetComponent<CanSmash>() != null && GetComponent<FoodStatus>().GetIsBoiled() && gameObject.GetComponent<FoodStatus>().GetIsPeeled())
        {
            AudioSource.PlayClipAtPoint(smashSound, transform.position);

            if (smashCount == 4)
            {
                if(gameObject.GetComponent<FoodStatus>().GetIsHalfSmashed()==false)
                { 
                    _smashed=Instantiate(smashed, smashable.transform.position,smashed.transform.rotation);
                    float size_y = _smashed.transform.GetChild(0).GetComponent<Renderer>().bounds.size.y;
                    float size_x = _smashed.transform.GetChild(0).GetComponent<Renderer>().bounds.size.x;
                    float size_z = _smashed.transform.GetChild(0).GetComponent<Renderer>().bounds.size.z;
                    Vector3 rescale = _smashed.transform.GetChild(0).transform.localScale;
                    rescale.y = smashable.GetComponent<Renderer>().bounds.size.y * rescale.y / (size_y*2.5f);
                    rescale.x = smashable.GetComponent<Renderer>().bounds.size.x * rescale.x / (size_x*2);
                    rescale.z = smashable.GetComponent<Renderer>().bounds.size.z * rescale.z / (size_z*2);
                    _smashed.transform.GetChild(0).transform.localScale = rescale;
                    _smashed.transform.GetChild(1).transform.localScale = rescale;
//                    initialFood = gameObject;
  //                  _smashed.GetComponent<CanBeSmashed>().initialFood = this.initialFood;
                    Destroy(gameObject);
                    _smashed.GetComponent<FoodStatus>().SetIsPeeled(true);
                    _smashed.GetComponent<FoodStatus>().SetIsBoiled(true);
                    _smashed.GetComponent<FoodStatus>().SetIsHalfSmashed(true);

                }
                else
                {
                    CanBeSmashed [] smashedObjects = FindObjectsOfType<CanBeSmashed>();
                    bool destroyed = false;

                    SimulationController sc = GameObject.Find("Simulation Controller").GetComponent<SimulationController>();
                    smashable.GetComponent<FoodStatus>().OperationDone += sc.OnOperationDone;

                    smashable.GetComponent<FoodStatus>().SetIsSmashed(true);

                    foreach (CanBeSmashed smashedObject in smashedObjects)
                    {
                        if (smashedObject.isCollidedWithBowl && smashedObject.iscollidedWithSmasher)
                        {
                            //Destroy(smashedObject.smashable);
                            smashedObject.smashable.gameObject.SetActive(false);
                            destroyed = true;
                        }
                    }
                    if (destroyed)
                    {
                        _smashed = Instantiate(smashed, bowl.transform.position, smashed.transform.rotation);
                    }
                    //_smashed.GetComponent<FoodStatus>().SetIsSmashed(true);
                }
            }

            else
            {
                smashCount++;
            }
            
        }
    }

}
