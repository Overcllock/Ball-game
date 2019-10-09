using UnityEngine;
using System;
using Object = UnityEngine.Object;

namespace game 
{
  public static class Assets
  {
    public const string NO_PREFAB = "";

    public static GameObject New(string prefab, Transform parent = null)
    {
      if(prefab == NO_PREFAB)
        return new GameObject();
        
      var tpl = ResourceLoad(prefab);
      var go = parent == null
        ? Object.Instantiate(tpl)
        : Object.Instantiate(tpl, parent);

      if(go != null)
        return go;

      var error_message = prefab != string.Empty
        ? "Can't load prefab, path: " + prefab
        : "Can't load prefab, path string is empty.";
      throw new Exception(error_message);
    }

    public static GameObject ResourceLoad(string prefab)
    {
      return ResourceLoad<GameObject>(prefab);
    }

    public static T ResourceLoad<T>(string prefab) where T : UnityEngine.Object
    {
      var o = Resources.Load<T>(prefab);
      if(o != null)
        return o;

      Error.Verify(false, "Can't load resource: " + prefab);
      return null;
    }
  }
}
