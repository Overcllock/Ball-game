using UnityEngine;
using UnityEngine.UI;

namespace game
{
  public class UIHud : UIWindow
  {
    public static readonly string PREFAB = "ui/ui_hud";

    GameField field;
    Text distance_text;
    Text score_text;

    public override void Init()
    {
      MakeButton("btn_pause", OnTogglePaused);
      MakeButton("btn_play", OnTogglePaused, set_active: false);
      
      distance_text = GetUIComponent<Text>("dist_label");
      Error.Verify(distance_text != null);
      score_text = GetUIComponent<Text>("score_label");
      Error.Verify(score_text != null);
    }

    public void SetField(GameField field)
    {
      this.field = field;
    }

    void Update()
    {
      distance_text.text = string.Format("Distance: {0}", Mathf.RoundToInt(field.result.distance));
      score_text.text = string.Format("Score: {0}", field.result.score);
    }

    void OnTogglePaused()
    {
      Game.TogglePause();
      UIComponentSetActive("btn_pause", !Game.is_paused);
      UIComponentSetActive("btn_play", Game.is_paused);
    }
  }
}