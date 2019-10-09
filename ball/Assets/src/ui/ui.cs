using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

namespace game
{
  public class UI : MonoBehaviour 
	{
    public static GameObject root;
    public static Camera camera;

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

      camera = root.GetChild("camera").GetComponent<Camera>();
      Error.Verify(camera != null);
    }
  }

  public abstract class UIWindow : MonoBehaviour
	{
		public void Close()
		{
			Error.Verify(UI.root != null);
				
			UI.windows.Remove(this);
			Destroy(gameObject);
		}

		protected void MakeButton(string path, UnityAction func)
		{
			var btn_go = transform.FindRecursive(path);
			var button = btn_go.GetComponent<Button>();
			if(button != null)
				button.onClick.AddListener(func);
		}

		protected T GetUIComponent<T>(string name) where T : Component
		{
			return transform.FindRecursive(name).GetComponent<T>();
		}
	}
}