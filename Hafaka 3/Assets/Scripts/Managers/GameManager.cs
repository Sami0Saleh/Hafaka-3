using UnityEditor.ShaderGraph;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public float GetTime { get { return time; } set { time = value; } }
    [SerializeField]float time;

    [SerializeField] private GameObject _pause;

    private PlayerNewInput _input;

    public bool IsPaused = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

        }
        else
        {
            Destroy(instance.gameObject);
        }

        _input = new PlayerNewInput();
        Time.timeScale = 1;
    }

    private void OnEnable()
    {
        _input.Enable();
        _input.Player.Escape.performed += _ => Pause();
        _input.UI.Escape.performed += _ => ClosePause();
        _input.UI.Escape.Disable();
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    void Update()
    {
        time += Time.time;
    }

    private void Pause()
    {
        _pause.SetActive(true);
        IsPaused = true;
        _input.UI.Escape.Enable();
        Time.timeScale = 0;
    }

    private void ClosePause()
    {
        _pause.SetActive(false);
        IsPaused = false;
        _input.UI.Escape.Disable();
        Time.timeScale = 1;
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(0);
    }
}
