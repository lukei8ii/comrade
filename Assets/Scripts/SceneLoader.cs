using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public int nextScene = -1;
    public float changeSceneAfter = 5f;

    // Start is called before the first frame update
    void Start()
    {
        if (nextScene > -1)
        {
            LoadSceneDelayed(nextScene, changeSceneAfter);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void LoadSceneDelayed(int sceneIndex, float delay)
    {
        StartCoroutine(DelayScene(sceneIndex, delay));
    }

    IEnumerator DelayScene(int sceneIndex, float delay)
    {
        yield return new WaitForSeconds(delay);
        LoadScene(sceneIndex);
    }
}
