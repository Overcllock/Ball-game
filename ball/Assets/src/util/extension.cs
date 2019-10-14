using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace game 
{
  public static class Extensions 
  {
    public static bool IsObjectVisible(this UnityEngine.Camera camera, Renderer renderer)
    {
      return GeometryUtility.TestPlanesAABB(GeometryUtility.CalculateFrustumPlanes(camera), renderer.bounds);
    }

    public static GameObject GetChild(this GameObject o, string name)
    {
      Transform t = o.transform.Find(name);
      if(t == null)
        Error.Verify(false, "Child not found {0}", name);
      return t.gameObject;
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

    public static List<GameFieldBlock> GetUpperBlocks(this List<GameFieldBlock> list)
    {
      return list.FindAll(block => block.direction == EnumBlockDirection.up);
    }

    public static List<GameFieldBlock> GetBottomBlocks(this List<GameFieldBlock> list)
    {
      return list.FindAll(block => block.direction == EnumBlockDirection.bottom);
    }
  }

  public static class JSON
  {
    public static T Read<T>(string path)
    {
      T data = default(T);
      using (StreamReader reader = new StreamReader(new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite)))
      {
        data = JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
      }

      return data;
    }

    public static void Write<T>(string path, T obj)
    {
      using (StreamWriter writer = new StreamWriter(new FileStream(path, FileMode.Truncate, FileAccess.Write)))
      {
        string data = JsonConvert.SerializeObject(obj, Formatting.Indented);
        writer.Write(data);
      }
    }
  }
}
