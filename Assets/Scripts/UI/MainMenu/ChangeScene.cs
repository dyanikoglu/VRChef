using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using RecipeModule;

public class ChangeScene : MonoBehaviour {

    public Canvas middleMenu;
    public Canvas template;
    public Object buttonPrefab;
    public GameObject objectSpawnerPrefab;

    private float initialHeight;
    private bool isRecipeButtonsOpen = false;
    private List<string> recipes;

    private void Awake()
    {
        initialHeight = template.GetComponent<RectTransform>().sizeDelta.y;
    }

    public void GetRecipeUI()
    {
        SceneManager.LoadScene("RecipeUI");
    }
    
    public void CreateRecipeButtons()
    {
        Vector2 sizeMiddle = middleMenu.GetComponent<RectTransform>().sizeDelta;
        Vector2 sizeTemplate = template.GetComponent<RectTransform>().sizeDelta;

        if (!isRecipeButtonsOpen)
        {
            recipes = GetRecipeNames();

            for (int r = 0; r < recipes.Count; r++)
            {
                GameObject newButton = (GameObject) Instantiate(buttonPrefab);
                newButton.transform.SetParent(middleMenu.gameObject.transform, false);
                newButton.GetComponent<RectTransform>().anchoredPosition = new Vector3(-80, -25 - (50 + 10) * r, 0);
                newButton.GetComponentInChildren<Text>().text = recipes[r];
                int tempInt = r;
                newButton.GetComponent<Button>().onClick.AddListener(() => ButtonHandler(tempInt));
            }

            middleMenu.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeMiddle.x, sizeMiddle.y + 50 * recipes.Count + 10 * (recipes.Count - 1));
            sizeMiddle = middleMenu.GetComponent<RectTransform>().sizeDelta;
            template.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeTemplate.x, sizeTemplate.y + sizeMiddle.y);
            middleMenu.gameObject.SetActive(true);
            isRecipeButtonsOpen = true;
        }
        else
        {
            template.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeTemplate.x, initialHeight);
            middleMenu.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeMiddle.x, 0);
            middleMenu.gameObject.SetActive(false);
            isRecipeButtonsOpen = false;
        }
    }

    void ButtonHandler(int buttonInt)
    {
        objectSpawnerPrefab.GetComponent<CreateRecipeScene>().recipeString = recipes[buttonInt];
        SceneManager.LoadScene("DuplicateKitchen");
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public List<string> GetRecipeNames()
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

