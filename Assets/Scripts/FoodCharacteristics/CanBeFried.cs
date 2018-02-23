using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBeFried : FoodCharacteristic
{

    [SerializeField]
    public Texture objectTexture;
    public Material myMaterial;

    public Shader shaderWithBlending;

    public Texture friedTexture;

    public float fryingTimeInSeconds;
    [SerializeField]

    //public bool isFried;
    public bool fryingStarted;
    public bool fryingStopped;
    public bool onCanFry;
    public GameObject canFryObject;

    private MaterialPropertyBlock propertyBlock;

    public Material friedMaterial;

    private void Awake()
    {
        GetComponent<Renderer>().material = new Material(GetComponent<Renderer>().material);
        myMaterial = GetComponent<Renderer>().material;
        propertyBlock = new MaterialPropertyBlock();

        fryingStarted = false;
        //isFried = false;
        onCanFry = false;
        fryingStopped = false;
    }

    // Use this for initialization
    void Start()
    {
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

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<CanFry>())
        {
            //Debug.Log("start frying");
            onCanFry = true;
            canFryObject = collision.gameObject;
            //StartFrying();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<CanFry>())
        {
            //Debug.Log("stop frying");
            onCanFry = false;
            //StartFrying();
        }
    }

    private void Update()
    {
        if (onCanFry)
        {
            //Debug.Log(!isFried + " - " + !fryingStarted + " - " + canFryObject.GetComponent<CanFry>().getCanFry());
            if(!GetIsFried() && !fryingStarted && canFryObject.GetComponent<CanFry>().GetCanFry())
            {
                StartFrying();
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
        // to animate object's frying process

        // replace shader with one can blend textures
        myMaterial.shader = shaderWithBlending;

        // set textures of object. (One for initial state, one for fried state)
        myMaterial.SetTexture("_MainTex", objectTexture);
        myMaterial.SetTexture("_Texture2", friedTexture);
        myMaterial.SetFloat("_Blend", 0f);



        StartCoroutine("Fry");
        Debug.Log("start frying");
    }

    public void StopFrying()
    {
        // to stop object's frying process
        StopCoroutine("Fry");
        fryingStopped = true;

        Debug.Log("stop frying");

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
        float blend = myMaterial.GetFloat("_Blend");
        while (blend < 1f)
        {
            //myMaterial.SetFloat("_Blend", blend);
            float fadeValue = Time.deltaTime / fryingTimeInSeconds;
            blend += fadeValue;

            GetComponent<Renderer>().GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat("_Blend", blend);
            GetComponent<Renderer>().SetPropertyBlock(propertyBlock);
            //if (GetComponent<CanBeChopped>())
            //{
            //    GetComponent<CanBeChopped>().capMaterial = GetComponent<Renderer>().material;
            //}
            yield return new WaitForSeconds(fadeValue);
        }
        SetIsFried(true);

        // if this food has been fried properly, then make it's inside fried too.
        // this block makes cutting fried objects works.
        if (GetComponent<CanBeChopped>())
        {
            GetComponent<CanBeChopped>().capMaterial = friedMaterial;
        }

    }

}
