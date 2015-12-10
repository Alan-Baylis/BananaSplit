using UnityEngine;

public class MenuController : MonoBehaviour
{
    public delegate void PauseEvent(bool paused);

    public static event PauseEvent PauseStateChanged;

    [SerializeField]
    private GameObject _menuGo = null;

    public static bool Paused { get; private set; }

    private void Start()
    {
        Paused = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (!Paused)
            Pause();
        else
            Unpause();

        if (PauseStateChanged != null)
            PauseStateChanged(Paused);
    }

    private void Pause()
    {
        Time.timeScale = 0f;

        _menuGo.SetActive(true);

        Paused = true;
    }

    private void Unpause()
    {
        Time.timeScale = 1f;

        _menuGo.SetActive(false);

        Paused = false;
    }

    public void LoadLevel(string levelName)
    {
        Unpause();
        Application.LoadLevel(levelName);
    }
}
