using UnityEngine;
using TMPro;

public class HighScore : MonoBehaviour
{
    public static TextMeshProUGUI highScore;

    void Start()
    {
        highScore = GetComponent<TextMeshProUGUI>();
        highScore.SetText(PlayerPrefs.GetInt("MaxPoints", 0).ToString().PadLeft(5, '0')); //Cargar el valor de maxPoints desde PlayerPrefs
    }
    private void Update() => highScore.SetText(PlayerPrefs.GetInt("MaxPoints").ToString().PadLeft(5, '0'));
}
