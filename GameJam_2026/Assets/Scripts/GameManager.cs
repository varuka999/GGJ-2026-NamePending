using UnityEngine;

public class GameManager : MonoBehaviour
{
    static GameManager instance = null;
    static public GameManager Instance {get {return instance;}}

    bool detectiveView = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public bool GetDetectiveView()
    {
        return detectiveView;
    }

    public void SetDetectiveView(bool view)
    {
        detectiveView = view;
    }
}
