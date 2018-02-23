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
    bool isBoiled = true;
    bool isHalfSmashedAll = true;
    public bool iscollidedWithSmasher = false;
    public bool isCollidedWithBowl = false;
    void Start()
    {
        smashCount = 0;
        source = gameObject.AddComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.GetComponent<CanSmash>() != null)
        {
            iscollidedWithSmasher = true;
        }
        if (col.gameObject.GetComponent<CanMixedIn>() != null)
        {
            isCollidedWithBowl = true;
        }
        if (col.gameObject.GetComponent<CanSmash>() != null && isBoiled && gameObject.GetComponent<FoodCharacteristic>().GetIsPeeled())
        {
            AudioSource.PlayClipAtPoint(smashSound, transform.position);

            if (smashCount == 4)
            {
                if(gameObject.GetComponent<FoodCharacteristic>().GetIsHalfSmashed()==false)
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
                    Destroy(gameObject);
                    _smashed.GetComponent<FoodCharacteristic>().SetIsPeeled(true);
                    _smashed.GetComponent<FoodCharacteristic>().SetIsHalfSmashed(true);
                }
                else
                {
                    CanBeSmashed [] smashedObjects = FindObjectsOfType<CanBeSmashed>();
                    foreach (CanBeSmashed smashedObject in smashedObjects)
                    {
                        if (smashedObject.isCollidedWithBowl && smashedObject.iscollidedWithSmasher)
                        {
                            Destroy(smashedObject.smashable);
                        }
                    }
                    _smashed = Instantiate(smashed, smashable.transform.position, smashed.transform.rotation);
        
                }
            }

            else
            {
                smashCount++;
            }
            
        }
    }

}
