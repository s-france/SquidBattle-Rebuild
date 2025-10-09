using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

public class SceneLoader : MonoBehaviour
{
    bool loading;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void Awake()
    {
        Debug.Log("SCENE LOADER AWAKE!!");
        

        loading = false;

    }

    public async void LoadScene(string sceneName)
    {
        if(!loading)
        {
            loading = true;

            //run LevelController EndLevel() function
            LevelController.Instance.EndLevel();
        

            var scene = SceneManager.LoadSceneAsync(sceneName);
            scene.allowSceneActivation = false;

            do {
                //INSERT LOADING SCREEN HERE
                //loading stuff
                Debug.Log("loading " + sceneName + "...");

                //artificial delay
                await Task.Delay(50);

            } while (scene.progress < 0.9f);

            scene.allowSceneActivation = true;
            loading = false;

        }
    }

    
}
