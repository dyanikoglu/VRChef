using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class ChangeScene : MonoBehaviour {

    public Canvas middleMenu;
    public Canvas template;
    public Object buttonPrefab;

    // Use this for initialization
	void Start () {

    }

    // Update is called once per frame
    void Update () {
		
	}

    public void GetRecipeUI()
    {
        SceneManager.LoadScene("RecipeUI");
    }
    
    public void CreateRecipeButtons()
    {        
        List<string> recipes = getRecipeNames();

        for (int r=0; r<recipes.Count; r++)
        {
            GameObject newButton = Instantiate(buttonPrefab) as GameObject;
            newButton.transform.SetParent(middleMenu.gameObject.transform, false);
            newButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(-80,-25-(50+10)*r,0);
            newButton.GetComponentInChildren<Text>().text = recipes[r];
            //newButton.GetComponent<Button>().onClick.AddListener(yaz);
        }
        
        Vector2 sizeMiddle = middleMenu.GetComponent<RectTransform>().sizeDelta;
        middleMenu.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeMiddle.x, sizeMiddle.y + 50*recipes.Count+10*(recipes.Count-1));
        Vector2 sizeTemplate = template.GetComponent<RectTransform>().sizeDelta;
        sizeMiddle = middleMenu.GetComponent<RectTransform>().sizeDelta;
        template.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeTemplate.x, sizeTemplate.y + sizeMiddle.y);
        middleMenu.gameObject.SetActive(true);
    }

    public List<string> getRecipeNames()
    {
        List<string> recipeNames = new List<string>();
        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/Recipes/");
        FileInfo[] info = dir.GetFiles("*.vrcr");
        foreach (FileInfo f in info)
        {
            recipeNames.Add(f.Name.Remove(f.Name.Length - 5));
        }
        return recipeNames;
    }
}

