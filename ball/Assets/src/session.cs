using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

namespace game
{
  public class GameResult
  {
    static readonly string SAVEFILE_PATH = "/userdata/score.json";

    public float distance;
    public uint score;

    public void Clear()
    {
      distance = 0;
      score = 0;
    }

    public void Save()
    {
      try
      {
        if(!Directory.Exists(Application.streamingAssetsPath + "/userdata"))
          Directory.CreateDirectory(Application.streamingAssetsPath + "/userdata");

        JSON.Write(Application.streamingAssetsPath + SAVEFILE_PATH, this);
        Debug.Log("Data saved successfully.");
      }
      catch (Exception ex)
      {
        Debug.LogError("Data not saved. " + ex.Message);
      }
    }

    public static GameResult Load()
    {
      try
      {
        if(!Directory.Exists(Application.streamingAssetsPath + "/userdata"))
          Directory.CreateDirectory(Application.streamingAssetsPath + "/userdata");

        var account = JSON.Read<GameResult>(Application.streamingAssetsPath + SAVEFILE_PATH);
        if(account != null)
        {
          Debug.Log("Data loaded successfully.");
          return account;
        }

        throw new Exception("User data is null or empty.");
      }
      catch (Exception ex)
      {
        Debug.LogError("Data not loaded. " + ex.Message);
        return new GameResult();
      }
    }
  }

  public abstract class GameFieldItem
  {
    protected GameObject obj;
    public Renderer renderer
    {
      get
      {
        Error.Verify(obj != null);
        var renderer = obj.GetComponentInChildren<Renderer>();
        Error.Verify(renderer != null);
        return renderer;
      }
    }

    public abstract void Release();
  }

  public class GameFieldBlock : GameFieldItem
  {
    const int NORMAL_BLOCK_CHANCE = 60;

    public EnumBlockType type;
    public EnumBlockDirection direction;

    public Vector3 position
    {
      get { return obj.transform.position; }
    }

    public static GameFieldBlock Create(GameField field, EnumBlockType type, EnumBlockDirection direction)
    {
      var block = new GameFieldBlock();
      
      block.type = type;
      block.direction = direction;

      var path = "field_items/blocks/";
      if(type == EnumBlockType.barrier)
        path += string.Format("barrier_{0}", direction);
      else
        path += type;

      block.obj = Assets.TryReuse(path, parent: field.transform);
      Error.Verify(block.obj != null);

      var pos = new Vector2();
      var border = direction == EnumBlockDirection.up ? field.upper_left_border : field.bottom_left_border;
      var blocks = direction == EnumBlockDirection.up ? field.blocks.GetUpperBlocks() : field.blocks.GetBottomBlocks();
      GameFieldBlock last_block = null;
      if(blocks != null && blocks.Count > 0)
        last_block = blocks[blocks.Count - 1];

      pos.x = last_block != null ? last_block.obj.transform.position.x + block.obj.transform.localScale.x : border.x;
      pos.y = border.y;

      block.obj.transform.position = pos;

      return block;
    }

    public static GameFieldBlock CreateRandom(GameField field, EnumBlockDirection direction)
    {
      EnumBlockType type;
      if(UnityEngine.Random.Range(1, 101) <= NORMAL_BLOCK_CHANCE)
        type = EnumBlockType.normal;
      else
        type = (EnumBlockType)UnityEngine.Random.Range((int)EnumBlockType.normal, Enum.GetNames(typeof(EnumBlockType)).Length);
      return Create(field, type, direction);
    }

    public override void Release()
    {
      Assets.Release(obj);
    }
  }

  public class GameFieldBall : GameFieldItem
  {
    MovementController mctl;
    GameField field;

    public bool is_valid
    {
      get { return mctl != null; }
    }

    public bool is_falling
    {
      get { return mctl != null && mctl.is_falling; }
    }

    public Vector3 position
    {
      get { return obj.transform.position; }
    }

    public GameFieldBall(GameField field)
    {
      this.field = field;

      obj = Assets.TryReuse("field_items/ball", field.transform);
      Error.Verify(obj != null);
   
      obj.transform.position = GetSpawnPos();
      mctl = obj.AddComponentOnce<MovementController>();
    }

    public void OnReuse()
    {
      obj = Assets.TryReuse("field_items/ball", field.transform);
      Error.Verify(obj != null);

      obj.transform.position = GetSpawnPos();

      mctl.OnReuse();
      mctl.ForceCameraAtObject();
    }

    Vector2 GetSpawnPos()
    {
      var pos = new Vector2();
      pos.x = field.bottom_left_border.x;
      pos.y = field.bottom_left_border.y + obj.transform.localScale.y;

      return pos;
    }

    public void SetSpeed(float value)
    {
      mctl.SetSpeed(value);
    }

    public float GetSpeed()
    {
      return mctl.GetSpeed();
    }

    public void Jump()
    {
      InverseGravity();
      field.CollectScore();
    }

    public void InverseGravity()
    {
      mctl.InverseGravity();
    }

    public override void Release()
    {
      Assets.Release(obj);
    }
  }

  public class GameField : MonoBehaviour
  {
    const uint MAX_BLOCKS = 12;
    const float BASE_DIST_DELTA = 0.1f;
    const float OBSTACLE_RADIUS = 2.0f;
    const int SCORE_MULT = 10;

    GameObject obj;
    GameFieldBall ball;
    public bool is_game_over;
    public GameResult result;
    public float dist_delta
    {
      get
      {
        if(!ball.is_valid)
          return BASE_DIST_DELTA;
        return BASE_DIST_DELTA * ball.GetSpeed();
      }
    }

    public Vector3 upper_left_border;
    public Vector3 bottom_left_border;

    public List<GameFieldBlock> blocks = new List<GameFieldBlock>();

    public void Init(GameObject obj)
    {
      Error.Verify(obj != null);
      this.obj = obj;

      result = new GameResult();
      is_game_over = false;

      upper_left_border = obj.GetChild("upper_left_border").transform.position;
      bottom_left_border = obj.GetChild("bottom_left_border").transform.position;

      InitBlocks();
      InitBall();
    }

    public void OnReuse()
    {
      result.Clear();
      InitBlocks();
      ball.OnReuse();
      is_game_over = false;
    }

    void InitBall()
    {
      ball = new GameFieldBall(this);
      Error.Verify(ball.is_valid);
    }

    void InitBlocks()
    {
      for(int i = 0; i < MAX_BLOCKS; ++i)
      {
        var upper_block = GameFieldBlock.Create(this, EnumBlockType.normal, EnumBlockDirection.up);
        var bottom_block = GameFieldBlock.Create(this, EnumBlockType.normal, EnumBlockDirection.bottom);

        blocks.Add(upper_block);
        blocks.Add(bottom_block);
      }
    }

    void Update()
    {
      if(is_game_over)
        return;

      ProcessInput();
    }

    void FixedUpdate()
    {
      if(is_game_over)
        return;

      if(!IsBallInWorldBounds())
      {
        Game.StopSession();
        return;
      }

      result.distance += dist_delta * Time.fixedDeltaTime;
      ball.SetSpeed(result.distance * 0.1f);

      DestroyBlocks();
      GenerateBlocks();
    }

    void ProcessInput()
    {
      if(!ball.is_valid || ball.is_falling)
        return;

#if UNITY_ANDROID || UNITY_IOS
      foreach(var touch in Input.touches) 
      {
        if(touch.phase == TouchPhase.Began) 
        {
          if(touch.position.y > Screen.height * 0.7f)
            continue;
          
          ball.Jump();
        }
      }
#else
      if(Input.GetMouseButtonDown(0))
      {
        if(Input.mousePosition.y < Screen.height * 0.7f)
          ball.Jump();
      }
#endif
    }

    public void CollectScore()
    {
      uint obstacles_count = 0;
      var all_blocks = GetNearbyBlocks(ball.position);
      foreach(var block in all_blocks)
      {
        if(block.type != EnumBlockType.normal)
          obstacles_count++;
      }
      
      if(obstacles_count > 0)
        result.score += (uint)Mathf.RoundToInt((obstacles_count + ball.GetSpeed() * 0.01f) * SCORE_MULT);
    }

    List<GameFieldBlock> GetNearbyBlocks(Vector3 position)
    {
      var nearby_blocks = new List<GameFieldBlock>();
      foreach(var block in blocks)
      {
        if(Mathf.Abs(block.position.x - position.x) < OBSTACLE_RADIUS)
          nearby_blocks.Add(block);
      }

      return nearby_blocks;
    }

    public void GameOver()
    {
      is_game_over = true;
      Reset();
    }

    bool IsBallInWorldBounds()
    {
      return Camera.main.IsObjectVisible(ball.renderer);
    }

    void DestroyBlocks()
    {
      for(int i = 0; i < blocks.Count; ++i)
      {
        var block = blocks[i];
        if(Camera.main.IsObjectVisible(block.renderer))
          break;
        
        block.Release();
        blocks.RemoveAt(i);
      }
    }

    void GenerateBlocks()
    {
      if(blocks.Count == 0)
        return;

      var control_block = blocks[blocks.Count - 1];
      if(!Camera.main.IsObjectVisible(control_block.renderer))
        return;

      GameFieldBlock upper_block;
      GameFieldBlock bottom_block;

      //Trying create new bottom block
      bottom_block = GameFieldBlock.CreateRandom(this, EnumBlockDirection.bottom);
      
      //If it's normal block - create random upper block
      if(bottom_block.type == EnumBlockType.normal)
        upper_block = GameFieldBlock.CreateRandom(this, EnumBlockDirection.up);
      //Or create normal upper block
      else
        upper_block = GameFieldBlock.Create(this, EnumBlockType.normal, EnumBlockDirection.up);
      
      blocks.Add(upper_block);
      blocks.Add(bottom_block);
    }

    void Reset()
    {
      ball.Release();
      foreach(var block in blocks)
        block.Release();

      blocks.Clear();
      Release();
    }

    void Release()
    {
      Assets.Release(obj);
    }
  }
  public class GameSession
  {
    GameField field;

    public void Start()
    {
      var field_go = Assets.TryReuse("field_items/field");
      field = field_go.GetComponent<GameField>();
      if(field == null)
      {
        field = field_go.AddComponentOnce<GameField>();
        field.Init(field_go);
      }
      else
        field.OnReuse();

      var hud = UI.Open<UIHud>();
      hud.Init();
      hud.SetField(field);
    }

    public void Stop()
    {
      var hud = UI.Find<UIHud>();
      if(hud != null)
        hud.Close();

      field.GameOver();
      TryWriteResult(field.result);
      
      var defeat_ui = UI.Open<UIDefeat>();
      defeat_ui.SetField(field);
      defeat_ui.Init();

      field.result.Clear();
    }

    void TryWriteResult(GameResult result)
    {
      var record = GameResult.Load();
      if(result.score > record.score)
        result.Save();
    }
  }
}