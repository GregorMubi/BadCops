using UnityEngine;

public class HumanController : MonoBehaviour {

    [SerializeField] private MeshRenderer BodyMeshRendere = null;
    [SerializeField] private Rigidbody RigidBody = null;

    private float TurnTimer = 0.0f;
    private float MinTurnTime = 0.0f;
    private float MaxTurnTime = 0.0f;
    private float MovementSpeed = 0.0f;
    private float MaxDistanceFromSpawn = 0.0f;
    private Vector3 SpawnPosition = Vector3.zero;
    private Vector3 TargetDirection = Vector3.zero;

    void Start() {
        RigidBody.centerOfMass += Vector3.down * 0.3f;
        SpawnPosition = transform.position;
        SetupParameters();
    }

    public void Init(float maxDistanceFromSpawn) {
        MaxDistanceFromSpawn = maxDistanceFromSpawn;
    }

    void Update() {
        TurnTimer -= Time.deltaTime;
        if (TurnTimer < 0) {
            if ((SpawnPosition - transform.position).magnitude > MaxDistanceFromSpawn) {
                TargetDirection = (SpawnPosition - transform.position).normalized;
            } else {
                TargetDirection = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));
            }


            TurnTimer = Random.Range(MinTurnTime, MaxTurnTime);
        }

        RigidBody.velocity = Vector3.Lerp(RigidBody.velocity, TargetDirection * MovementSpeed * 0.1f, 0.05f);
    }

    private void SetupParameters() {
        BodyMeshRendere.material.color = StaticGameHelper.GetRandomColor();
        MinTurnTime = Random.Range(1.0f, 4.0f);
        MaxTurnTime = MinTurnTime + Random.Range(1.0f, 4.0f);
        TurnTimer = Random.Range(MinTurnTime, MaxTurnTime);
        MovementSpeed = Random.Range(3.0f, 6.0f);
        TargetDirection = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));
    }
}
