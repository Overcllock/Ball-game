using UnityEngine.UI;

namespace game
{
  public class UIStart : UIWindow
  {
    public static readonly string PREFAB = "ui/ui_start";

    public override void Init()
    {
      MakeButton("btn_start", OnGameStart);
      MakeButton("btn_exit", OnGameExit);
      
      var records_info = GameResult.Load();
      var records_text = GetUIComponent<Text>("best_text");
      Error.Verify(records_text != null);
      records_text.text = string.Format("Distance: {0:0.00}\nScore: {1}", records_info.distance, records_info.score);
    }

    void OnGameStart()
    {
      Game.StartSession();
      Close();
    }

    void OnGameExit()
    {
      Game.Quit();
    }
  }
}