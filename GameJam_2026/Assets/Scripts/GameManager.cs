using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance = null;
    static public GameManager Instance {get {return instance;}}

    bool detectiveView = false;

    Material material = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        material = new Material(Resources.Load<Material>("MasterShader"));
    }

    public bool GetDetectiveView()
    {
        return detectiveView;
    }

    public void SetDetectiveView(bool view)
    {
        detectiveView = view;
        Grayscale(detectiveView);
    }

    void SetGlobalMaterial()
    {
        Renderer[] allRenderers = FindObjectsByType<Renderer>(FindObjectsSortMode.None);
        foreach (Renderer renderer in allRenderers)
        {
            if (material == null)
            {
                material = renderer.material;
            }
            else
            {
                renderer.material = material;
            }
        }
    }

    public Material GetMaterial()
    {
        return material;
    }

    void Grayscale(bool value)
    {
        SetGlobalMaterial();
        int saturation = 0;
        if(value)
        {
            saturation++;
        }

        material.SetInt("_Saturation",saturation);
    }


}
