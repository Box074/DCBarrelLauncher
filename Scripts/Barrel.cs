
namespace DCBarrelLauncherMod;

class Barrel : MonoBehaviour
{
    public Rigidbody2D rig;
    public CircleCollider2D col;
    public SpriteRenderer sr;
    public float saveTime = 0;
    private void Awake()
    {
        transform.localScale = new Vector3(0.5f, 0.5f, 1);

        var rigCol = new GameObject();
        rigCol.transform.parent = transform;
        rigCol.AddComponent<CircleCollider2D>().radius = 0.8f;
        rigCol.layer = (int)GlobalEnums.PhysLayers.ENEMIES;
        rigCol.transform.localPosition = Vector3.zero;

        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = DCBarrelLauncher.barrelSprite;

        col = gameObject.AddComponent<CircleCollider2D>();
        col.radius = 0.8f;
        col.offset = Vector2.zero;
        col.isTrigger = true;
        rig = gameObject.AddComponent<Rigidbody2D>();
        var rigMaterial = new PhysicsMaterial2D();
        rigMaterial.bounciness = 1f;
        rigMaterial.friction = 0f;
        rig.sharedMaterial = rigMaterial;
    }
    private void Update() {
        saveTime += Time.deltaTime;
        if(saveTime > 5)
        {
            Fire();
        }
    }
    private void Fire()
    {
        Instantiate(DCBarrelLauncher.expPrefab, transform.position, Quaternion.identity, null).SetActive(true);
        
        Destroy(gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.GetComponent<HealthManager>() != null)
        {
            Fire();
            FSMUtility.SendEventToGameObject(other.gameObject, "EXPLODE");
        }
    }
}
