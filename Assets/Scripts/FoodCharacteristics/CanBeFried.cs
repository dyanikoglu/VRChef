using System.Collections;
using UnityEngine;
using System;

public class CanBeFried : FoodCharacteristic
{
    public Texture objectTexture;

    public Material[] myMaterials;
    public Material myMaterial;

    public Shader shaderWithBlending;

    public Texture friedTexture;

    public float fryingTimeInSeconds;

    public bool fryingStarted;
    public bool fryingStopped;
    public bool onCanFry;
    public GameObject canFryObject;

    float blend = 0;

    private MaterialPropertyBlock propertyBlock;

    public Material friedMaterial;
    public Color burnedColor;

    public AudioClip fryingSound;
    AudioSource source;
    bool onlyOnce = true;

    bool isFriedLocal;

    private void Awake()
    {
        GetComponent<Renderer>().material = new Material(GetComponent<Renderer>().material);
        myMaterial = GetComponent<Renderer>().material;
        propertyBlock = new MaterialPropertyBlock();

        fryingStarted = false;
        onCanFry = false;
        fryingStopped = false;

        friedMaterial.SetFloat("_WetWeight", 0);

        isFriedLocal = false;
    }

    // Use this for initialization
    void Start()
    {
        base.Start();

        objectTexture = myMaterial.mainTexture;
        if (!objectTexture)
        {
            Texture2D texture = new Texture2D(2, 2, TextureFormat.ARGB32, false);

            // set the pixel values
            Color color = myMaterial.color;
            texture.SetPixel(0, 0, color);
            texture.SetPixel(1, 0, color);
            texture.SetPixel(0, 1, color);
            texture.SetPixel(1, 1, color);

            // Apply all SetPixel calls
            texture.Apply();
            objectTexture = texture;
        }

        source = gameObject.AddComponent<AudioSource>();
        source.loop = true;
        source.clip = fryingSound;
        source.playOnAwake = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CanFry>())
        {
            onCanFry = true;
            canFryObject = collision.gameObject;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<CanFry>())
        {
            onCanFry = false;
        }
    }

    private void Update()
    {
        if (onCanFry)
        {
            if(!GetComponent<FoodStatus>().GetIsFried() && !fryingStarted && canFryObject.GetComponent<CanFry>().GetCanFry() && canFryObject.GetComponent<CanFry>().GetHasFluid())
            {
                StartFrying();
            }
            else if (!canFryObject.GetComponent<CanFry>().GetCanFry())
            {
                StopFrying();
            }
        }
        else
        {
            if(fryingStarted && !fryingStopped)
            {
                StopFrying();
            }
        }
    }

    public void StartFrying()
    {
        int i = 0;
        myMaterials = new Material[GetComponent<Renderer>().materials.Length];

        foreach (Material m in GetComponent<Renderer>().materials)
        {
            myMaterials[i] = m;

            myMaterials[i].shader = shaderWithBlending;

            myMaterials[i].SetTexture("_MainTex", objectTexture);
            myMaterials[i].SetTexture("_Texture2", friedTexture);
            myMaterials[i].SetFloat("_Blend", 0f);
            i++;
        }

        GetComponent<Renderer>().sharedMaterials = myMaterials;

        fryingStopped = false;

        // to animate object's frying process
        StartCoroutine("Fry");

        if (onlyOnce)
        {
            source.loop = true;
            source.Play();
            onlyOnce = false;
        }
    }

    public void StopFrying()
    {
        // to stop object's frying process
        StopCoroutine("Fry");
        fryingStopped = true;
        fryingStarted = false;

        source.loop = false;
        source.Stop();
        onlyOnce = true;

        if(isFriedLocal)
        {
            GetComponent<FoodStatus>().SetIsFried(true);
        }
    }

    IEnumerator Fry()
    {
        /* <Method Summary>
         * Method animates frying process by changing texture blending relative to 
         * frying time. Once frying time passed, blend value will be 1 implying
         * object's texture include only it's fried texture
         * <Method Summary>
         */
        fryingStarted = true;
        float fadeValue = 0f;

        while (blend < 1f)
        {
            fadeValue = Time.deltaTime / fryingTimeInSeconds;
            blend += fadeValue;

            for (int i = 0; i < myMaterials.Length; i++)
            {
                myMaterials[i].SetFloat("_Blend", blend);

                if (GetComponent<CanBeChopped>())
                {
                    GetComponent<CanBeChopped>().capMaterial = myMaterials[i];
                }
            }
            yield return new WaitForSeconds(fadeValue);

        }

        isFriedLocal = true;
        //GetComponent<FoodStatus>().SetIsFried(true);

        // not only need to change myMaterial but also change GetComponent...
        for (int i = 0; i < myMaterials.Length; i++)
        {
            myMaterials[i] = friedMaterial;
        }
        GetComponent<Renderer>().sharedMaterials = myMaterials;

        // if this food has been fried properly, then make it's inside fried too.
        // this block makes cutting fried objects works.
        if (GetComponent<CanBeChopped>())
        {
            GetComponent<CanBeChopped>().capMaterial = friedMaterial;
        }

         yield return new WaitForSeconds(3f);


         for (int i = 0; i < GetComponent<Renderer>().sharedMaterials.Length; i++)
         {
             GetComponent<Renderer>().sharedMaterials[i].SetColor("_WetTint", burnedColor);
         }

        isFriedLocal = false;
        /*
        float burnedLevel = 0f;

        Material[] copyMaterials = new Material[GetComponent<Renderer>().sharedMaterials.Length];

        for (int i = 0; i < GetComponent<Renderer>().sharedMaterials.Length; i++)
        {
            //GetComponent<Renderer>().sharedMaterials[i].SetFloat("_WetWeight", burnedLevel);
            copyMaterials[i] = GetComponent<Renderer>().sharedMaterials[i];
        }

        while (burnedLevel < 1f)
         {
             fadeValue = Time.deltaTime / fryingTimeInSeconds;
             burnedLevel += fadeValue;
             if(burnedLevel > 0.5f)
             {
                GetComponent<FoodStatus>().SetIsBurned(true);
                break;
             }

             yield return new WaitForSeconds(fadeValue);
         }

         // if it is burned, then it is useless.
        if (GetComponents<FoodCharacteristic>().Length != 0)
        {
            foreach(FoodCharacteristic fc in GetComponents<FoodCharacteristic>())
            {
                Destroy(fc);
            }
        }
        */

    }
}
