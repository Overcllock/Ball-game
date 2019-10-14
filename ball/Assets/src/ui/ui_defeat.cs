using UnityEngine.UI;

namespace game
{
  public class UIDefeat : UIWindow
  {
    public static readonly string PREFAB = "ui/ui_defeat";

    GameField field;

    public override void Init()
    {
      Error.Verify(field != null);

      MakeButton("btn_restart", OnGameRestart);
      MakeButton("btn_exit", OnGameExit);
      
      var record_info = GameResult.Load();
      var result_info = field.result;

      var record_label = GetUIComponent<Text>("record_label");
      Error.Verify(record_label != null);
      var result_text = GetUIComponent<Text>("result_text");
      Error.Verify(result_text != null);

      record_label.text = result_info.score == record_info.score ? "New record!" : "Your result:";
      result_text.text = string.Format("Distance: {0:0.00}\nScore: {1}", result_info.distance, result_info.score);
    }

    public void SetField(GameField field)
    {
      this.field = field;
    }

    void OnGameRestart()
    {
      Game.StartSession();
      Close();
    }

    void OnGameExit()
    {
      var ui_start = UI.Open<UIStart>();
      ui_start.Init();
      Close();
    }
  }
}