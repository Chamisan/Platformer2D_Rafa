using UnityEngine;
using UnityEngine.SceneManagement;
public class DontDestroyOnLoad : MonoBehaviour
{
    void Start() => DontDestroyOnLoad(gameObject);
}
