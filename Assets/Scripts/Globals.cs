using UnityEngine;

public class Globals : MonoBehaviour
{

    public static Globals instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            return;
        }

        Destroy(this);
    }

    public int Score;
    public int KillCount;

}
