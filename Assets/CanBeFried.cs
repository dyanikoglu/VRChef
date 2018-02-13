using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanBeFried : MonoBehaviour
{

    [SerializeField]
    Texture objectTexture;
    Material myMaterial;

    public Shader shaderWithBlending;
    public Texture friedTexture;

    public float fryingTimeInSeconds;
    [SerializeField]
    bool isFried;

    private MaterialPropertyBlock propertyBlock;

    private void Awake()
    {
        GetComponent<Renderer>().material = new Material(GetComponent<Renderer>().material);
        myMaterial = GetComponent<Renderer>().material;
        propertyBlock = new MaterialPropertyBlock();

        isFried = false;
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

        // replace shader with one can blend textures
        myMaterial.shader = shaderWithBlending;

        // set textures of object. (One for initial state, one for fried state)
        myMaterial.SetTexture("_MainTex", objectTexture);
        myMaterial.SetTexture("_Texture2", friedTexture);
        myMaterial.SetFloat("_Blend", 0f);


    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log(collision.collider.name);
        if(collision.collider.name.Contains("Cube") && !isFried)
        {
            Debug.Log("wtf");
            StartFrying();
        }
    }

    void StartFrying()
    {

        // to animate object's frying process
        StartCoroutine("Fry");
    }

    IEnumerator Fry()
    {
        /* <Method Summary>
         * Method animates frying process by changing texture blending relative to 
         * frying time. Once frying time passed, blend value will be 1 implying
         * object's texture include only it's fried texture
         * <Method Summary>
         */

        float blend = myMaterial.GetFloat("_Blend");
        while (blend < 1f)
        {
            //myMaterial.SetFloat("_Blend", blend);
            float fadeValue = Time.deltaTime / fryingTimeInSeconds;
            blend += fadeValue;

            GetComponent<Renderer>().GetPropertyBlock(propertyBlock);
            propertyBlock.SetFloat("_Blend", blend);
            GetComponent<Renderer>().SetPropertyBlock(propertyBlock);

            yield return new WaitForSeconds(fadeValue);
        }
        isFried = true;
    }

}
