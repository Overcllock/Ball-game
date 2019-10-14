using UnityEngine;

namespace game 
{
  public class Game : MonoBehaviour
  {
    public static Game self;

    static GameSession session = new GameSession();
    public static bool is_paused = false;

    void Awake()
    {
      Init();
    }

    void Init()
    {
      self = this;

#if UNITY_EDITOR
      Assets.InitForEditor();
#endif

      UI.Init();
      
      var ui_start = UI.Open<UIStart>();
      ui_start.Init();
    }

    public static void TogglePause()
    {
      is_paused = !is_paused;
      Time.timeScale = is_paused ? 0 : 1;
    }

    public static void StartSession()
    {
      session.Start();
    }

    public static void StopSession()
    {
      session.Stop();
    }

    public static void Quit()
    {
      Application.Quit();
    }
  }
}