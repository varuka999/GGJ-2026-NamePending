using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance = null;
    static public GameManager Instance {get {return instance;}}

    [SerializeField] private GameObject uiManagerPrefab = null;
    [SerializeField] private GameObject cinemachinePrefab = null;
    [SerializeField] private GameObject playerPrefab = null;
    [SerializeField] private Transform playerSpawnTransform = null;
    [SerializeField] private List<MaskType> startingMasks;

    bool detectiveView = false;

    Material material = null;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        material = new Material(Resources.Load<Material>("MasterShader"));

        Instantiate(uiManagerPrefab);
        // Player & Camera Setup
        GameObject player = Instantiate(playerPrefab, playerSpawnTransform.position, Quaternion.identity);
        player.SetActive(false);
        player.GetComponent<PlayerController>().Initialize(cinemachinePrefab);
        foreach (MaskType mask in startingMasks)
        {
            player.GetComponent<PlayerController>().ObtainMask(mask);
        }
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

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ToggleClueText(view);
        }
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
