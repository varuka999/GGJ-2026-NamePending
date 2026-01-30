using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance = null;
    static public GameManager Instance {get {return instance;}}

    [SerializeField] private GameObject cinemachinePrefab = null;
    [SerializeField] private GameObject playerPrefab = null;

    bool detectiveView = false;

    Material material = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        material = new Material(Resources.Load<Material>("MasterShader"));

        // Player & Camera Setup
        GameObject player = Instantiate(playerPrefab);
        player.SetActive(false);
        player.GetComponent<PlayerController>().Initialize(cinemachinePrefab);
    }

    void Start()
    {
        SetGlobalMaterial();
        Interactible[] interactibles = FindObjectsByType<Interactible>(FindObjectsSortMode.None);
        foreach(Interactible interactible in interactibles)
        {
            interactible.InitMaterial();
        }
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
        int saturation = 0;
        if(value)
        {
            saturation++;
        }

        material.SetInt("_Saturation",saturation);
    }
}
