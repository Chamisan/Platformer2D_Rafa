using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //La siguiente línea logra que la clase GM no necesite instanciar un objeto para utilizar sus métodos
    public static GameManager Instance { get; private set; }

    public static int points;
    public static int maxPoints;
    public static int lifesPlayer = 3;
    //Este Awake es complementario a lo anterior, hace una instancia de esta clase desde el principio de todo.
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(this);
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        maxPoints = PlayerPrefs.GetInt("Maxpoints", 0); //Cargar el valor de maxPoints desde PlayerPrefs
        points = 0; 
    }

    //De aquí para adelante empiezan los métodos generales del Game Manager:
    public static void StartGame() => SceneManager.LoadScene(1);
    public static void PlayGame() => Time.timeScale = 1;
    public static void PauseGame() => Time.timeScale = 0;
    public static void MainMenu()
    {
        PauseGame();
        SceneManager.LoadScene(0, LoadSceneMode.Additive);
    }
    public static void RestartLvl() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    public static void NextLvl() => SceneManager.LoadScene(2);
    public static void RestartGame() => SceneManager.LoadScene("Main");
    public static void ExitGame() => Application.Quit();
}
