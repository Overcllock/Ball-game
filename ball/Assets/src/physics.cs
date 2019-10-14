using UnityEngine;

namespace game
{
  public class MovementController : MonoBehaviour
  {
    public const float base_speed = 100.0f;
    const float default_gravity_scale = 30.0f;
    const float camera_offset = 8.0f;

    Camera cam;
    Rigidbody2D _rigidbody;

    float speed = base_speed;

    public bool is_falling
    {
      get { return Mathf.Abs(_rigidbody.velocity.y) > 0.01f; }
    }

    void Awake()
    {
      cam = Camera.main;
      Error.Verify(cam != null);

      _rigidbody = GetComponent<Rigidbody2D>();
      Error.Verify(_rigidbody != null);
    }

    void Update()
    {
      MoveCamera();
    }
    
    void FixedUpdate()
    {
      MoveObject();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
      if(collision.gameObject.tag != "normal")
        Game.StopSession();
    }

    public void OnReuse()
    {
      speed = base_speed;
      _rigidbody.gravityScale = default_gravity_scale;
    }

    void MoveObject()
    {
      var force = Vector2.right * speed * Time.fixedDeltaTime;
      _rigidbody.velocity = force;
    }

    void MoveCamera()
    {
      var target_pos = new Vector3(transform.position.x + camera_offset, transform.position.y, transform.position.z);
      var old_pos = cam.transform.position;
      var new_pos = Vector3.Lerp(cam.transform.position, target_pos, Time.deltaTime * speed);

      new_pos.y = old_pos.y;
      new_pos.z = old_pos.z;

      cam.transform.position = new_pos;
    }

    public void ForceCameraAtObject()
    {
      var old_pos = cam.transform.position;
      var target_pos = new Vector3(transform.position.x + camera_offset, old_pos.y, old_pos.z);

      cam.transform.position = target_pos;
    }

    public void SetSpeed(float value)
    {
      speed = base_speed + value;
    }

    public float GetSpeed()
    {
      return speed;
    }

    public void InverseGravity()
    {
      _rigidbody.gravityScale = -_rigidbody.gravityScale;
    }
  }
}