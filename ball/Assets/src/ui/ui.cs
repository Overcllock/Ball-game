using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace game
{
  public class UI : MonoBehaviour 
  {
    public static GameObject root;

    [HideInInspector]
    public static List<UIWindow> windows = new List<UIWindow>();

    public static void Init()
    {
     Init(Assets.New("ui/ui_root"));
    }

    static void Init(GameObject new_root)
    {
      root = new_root;
      Error.Verify(root != null);
    }

    public static T Open<T>() where T : UIWindow
    {
      string prefab = GetPrefab(typeof(T));
      var ui_window_go = Assets.New(prefab, UI.root.transform);
      var window = ui_window_go.AddComponentOnce<T>();
      if(window != null)
        windows.Add(window);

      return window as T;
    }

    public static T Find<T>() where T : UIWindow
    {
      foreach(var window in windows)
      {
        if(window is T)
          return window as T;
      }

      return null;
    }

    static string GetPrefab(System.Type type)
    {
      var field = type.GetField("PREFAB");
      if(field == null)
      {
        Debug.LogError("Field \"PREFAB\" not found in type " + type.Name);
        return string.Empty;
      }

      object val = field.GetValue(null);
      string prefab = val as string;

      if(prefab == null)
        return string.Empty;
      return prefab;
    }
  }

  public abstract class UIWindow : MonoBehaviour
  {
    public abstract void Init();

    public void Close()
    {
      UI.windows.Remove(this);
      Destroy(gameObject);
    }

    protected void MakeButton(string path, UnityAction func, bool set_active = true)
    {
      var btn_go = transform.FindRecursive(path);
      var button = btn_go.GetComponent<Button>();
      if(button != null)
      {
        button.onClick.AddListener(func);
        button.gameObject.SetActive(set_active);
      }
    }

    protected void UIComponentSetActive(string path, bool active)
    {
      transform.FindRecursive(path).gameObject.SetActive(active);
    }

    protected T GetUIComponent<T>(string name) where T : Component
    {
      return transform.FindRecursive(name).GetComponent<T>();
    }
  }
}