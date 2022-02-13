
namespace DCBarrelLauncherMod;

class Gun : MonoBehaviour
{
    public bool fire = false;
    public SpriteRenderer sr;
    public AudioSource audioSource;
    private void Awake() {
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = DCBarrelLauncher.gunSprite;
        sr.enabled = false;
        audioSource = gameObject.AddComponent<AudioSource>();
    }
    private bool CanAttack()
    {
        var hc = HeroController.instance;
        return !hc.cState.attacking && !hc.cState.dashing && !hc.cState.dead && !hc.cState.hazardDeath 
        && !hc.cState.hazardRespawning && !hc.controlReqlinquished && hc.hero_state != ActorStates.no_input &&
        hc.hero_state != ActorStates.hard_landing && hc.hero_state != ActorStates.dash_landing;
    }
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            DCBarrelLauncher.Fire();
        }
        if(InputHandler.Instance.inputActions.attack.IsPressed && CanAttack())
        {
            if(!fire)
            {
                fire = true;
                StartCoroutine(Fire());
            }
        }
    }
    public IEnumerator Fire()
    {
        transform.localPosition = new Vector3(-0.15f, -0.65f, -0.1f);
        transform.localScale = new Vector3(-1, 1, 1);
        fire = true;
        yield return null;
        var hc = HeroController.instance;
        hc.AffectedByGravity(false);
        hc.StopAnimationControl();
        hc.RelinquishControl();
        var anim = hc.GetComponent<tk2dSpriteAnimator>();
        anim.Play("Idle");
        yield return null;
        float st = Time.time;
        yield return null;
        sr.enabled = true;
        while(Time.time - st < 0.05f)
        {
            transform.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(90, 45, (Time.time - st) * 20));
            yield return null;
        }
        yield return new WaitForSeconds(0.2f);
        audioSource.PlayOneShot(DCBarrelLauncher.weapon_firecrossbow_shot);
        DCBarrelLauncher.Fire();
        yield return new WaitForSeconds(0.25f);
        st = Time.time;
        while (Time.time - st < 0.05f)
        {
            transform.localEulerAngles = new Vector3(0, 0, Mathf.Lerp(45, 90, (Time.time - st) * 20));
            yield return null;
        }
        sr.enabled = false;
        hc.StartAnimationControl();
        hc.AffectedByGravity(true);
        hc.RegainControl();
        yield return new WaitForSeconds(0.5f);
        fire = false;
    }
}
