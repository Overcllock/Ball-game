using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace game 
{
  public class Game : MonoBehaviour
  {
    public static Game self;

    static bool is_paused = false;

    void Awake()
    {
      self = this;
    }

    void Start()
    {
      UI.Init();
    }

    public static void TogglePause()
    {
      is_paused = !is_paused;
      Time.timeScale = is_paused ? 0 : 1;

			//TODO: Show pause UI
    }

    public static void Quit()
    {
      Application.Quit();
    }
  }
}