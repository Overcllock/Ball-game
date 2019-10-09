using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace game 
{
  public static class Extensions 
  {
    public static GameObject GetChild(this GameObject o, string name)
    {
      Transform t = o.transform.Find(name);
      if(t == null)
        Error.Verify(false, "Child not found {0}", name);
      return t.gameObject;
    }

    public static T GetChild<T>(this GameObject o, string name) where T : Component
    {
      return o.GetChild(name).AsComponent<T>();
    }

    public static GameObject FindChild(this GameObject o, string name)
    {
      Transform t = o.transform.Find(name);
      if(t)
        return t.gameObject;
      return null;
    }

    public static Transform GetChild(this Transform o, string name)
    {
      Transform t = o.transform.Find(name);
      if(t == null)
        Error.Verify(false, "Child not found {0}", name);
      return t;
    }

    public static Transform FindRecursive(this Transform current, string name)   
    {
      if(current.parent)
      {
        if(current.parent.Find(name) == current)
          return current;
      }
      else if(current.name == name)
        return current;

      for(int i = 0; i < current.childCount; ++i)
      {
        var chld = current.GetChild(i); 
        var tmp = chld.FindRecursive(name);
        if(tmp != null)
          return tmp;
      }
      return null;
    }

    public static GameObject GetParent(this GameObject o)
    {
      return o.transform.parent.gameObject;
    }
    
    public static T AddComponentOnce<T>(this GameObject self) where T : Component
    {
      T c = self.GetComponent<T>();
      if(c == null)
        c = self.AddComponent<T>();
      return c;
    }

    public static T AsComponent<T>(this GameObject o) where T : Component
    {
      var res = o.GetComponent<T>();
      Error.Verify(res != null);
      return res;
    }

    public static bool AddUnique<T>(this List<T> dst, T o)
    {
      if(dst.Contains(o))
        return false;
      dst.Add(o);
      return true;
    }

    public static T RandomValue<T>(this List<T> list) where T : class
    {
      if(list.Count == 0)
        return null;
      return list[UnityEngine.Random.Range(0, list.Count)];
    }
  }
}
