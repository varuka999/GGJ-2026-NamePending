using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneInteractible : Interactible
{
    [SerializeField] string sceneName;
    public override void OnInteract()
    {
        SceneManager.LoadScene(sceneName);
    }
}
