using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text scoreText;

    private void LateUpdate()
    {
        scoreText.text = "Score: " + Globals.instance.Score;
    }
}
