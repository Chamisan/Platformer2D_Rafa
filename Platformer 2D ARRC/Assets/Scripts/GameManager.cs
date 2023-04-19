using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
public class GameManager : MonoBehaviour
{
    //La siguiente línea logra que la clase GM no necesite instanciar un objeto para utilizar sus métodos
    public static GameManager instance { get; private set; }

    public static int points = 0;
    public static int maxPoints;
    public static int lifesPlayer = 3;
    public static TextMeshProUGUI score;
    //Este Awake es complementario a lo anterior, hace una instancia de esta clase desde el principio de todo.
    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
    void Start()
    {   
        if(SceneManager.GetActiveScene().name!="Main") //Si no se está en la escena Main
            score = GameObject.Find("ScorePoints").GetComponent<TextMeshProUGUI>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void Update()
    {
        if (SceneManager.GetActiveScene().name != "Main")
            score.SetText(points.ToString().PadLeft(5, '0'));
    }
    public static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Obtiene el componente TextMeshPro del objeto encontrado
        score = GameObject.Find("ScorePoints").GetComponent<TextMeshProUGUI>();
    }    
    public static void StartGame() => SceneManager.LoadScene(1);
    public static void PlayGame() => Time.timeScale = 1;
    public static void PauseGame() => Time.timeScale = 0;
    public static void MainMenu()
    {
        PauseGame();
        SceneManager.LoadScene(0, LoadSceneMode.Additive);
    }
    public static void RestartLvl()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        lifesPlayer = 3; //Reinicia conteo de vidas 
        points = maxPoints;
    }   
    public static void NextLvl()
    {
        SceneManager.LoadScene(2);
        score = GameObject.Find("ScorePoints")?.GetComponent<TextMeshProUGUI>();
        score.SetText(points.ToString().PadLeft(5, '0'));
        maxPoints = points;
    }
    public static void RestartGame() => SceneManager.LoadScene("Main");
    public static void ExitGame() => Application.Quit();
}
