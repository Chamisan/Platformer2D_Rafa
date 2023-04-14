using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    //La siguiente línea logra que la clase GM no necesite instanciar un objeto para utilizar sus métodos
    public static GameManager instance { get; private set; }

    public static int points;
    public static int maxPoints;
    public static int lifesPlayer = 3;
    public TextMeshProUGUI score;
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

        maxPoints = PlayerPrefs.GetInt("Maxpoints", 0); //Cargar el valor de maxPoints desde PlayerPrefs
        points = 0; 
    }
    void Start()
    {
        GameObject scoreObject = GameObject.Find("ScorePoints");
        // Obtiene el componente TextMeshPro del objeto encontrado
        score = scoreObject.GetComponent<TextMeshProUGUI>();
        
        SceneManager.sceneLoaded += OnSceneLoaded; //ubica la escena en la que se está
    }
    private void Update()
    {
        score.SetText(points.ToString().PadLeft(5, '0'));
    }
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject scoreObject = GameObject.Find("ScorePoints");
        // Obtiene el componente TextMeshPro del objeto encontrado
        score = scoreObject.GetComponent<TextMeshProUGUI>();
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
    public static void RestartLvl()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        lifesPlayer = 3; //Reinicia conteo de vidas 
    }   
    public static void NextLvl()
    {
        PlayerPrefs.SetInt("points", points); //Pasar el valor de points al PlayerPrefs
        SceneManager.LoadScene(2);
        points = PlayerPrefs.GetInt("points"); //Obtener el valor nuevamente de PlayerPrefs

    }
        
    public static void RestartGame() => SceneManager.LoadScene("Main");
    public static void ExitGame() => Application.Quit();
}
