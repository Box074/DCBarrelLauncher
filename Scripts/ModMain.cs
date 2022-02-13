
namespace DCBarrelLauncherMod;

class DCBarrelLauncher : ModBase
{
    public static Texture2D gunTex;
    public static Sprite gunSprite;
    public static Texture2D barrelTex;
    public static Sprite barrelSprite;
    public static GameObject expPrefab;
    public static AudioClip weapon_firecrossbow_shot;
    public override void Initialize()
    {
        using (var stream = GetType().Assembly.GetManifestResourceStream("DCBarrelLauncher.shot.wav"))
        {
            byte[] temp = new byte[stream.Length];
            stream.Read(temp, 0, temp.Length);
            weapon_firecrossbow_shot = WavUtility.ToAudioClip(temp);
        }

        gunTex = LoadTexture2D("DCBarrelLauncher.gun.png");
        gunSprite = Sprite.Create(gunTex, new Rect(0, 0, gunTex.width, gunTex.height),
            new Vector2(0.25f, 0.5f), 72);

        barrelTex = LoadTexture2D("DCBarrelLauncher.barrel.png");
        barrelSprite = Sprite.Create(barrelTex, new Rect(0, 0, barrelTex.width, barrelTex.height),
            new Vector2(0.5f, 0.5f), 64);


        expPrefab = UnityEngine.Object.Instantiate(Resources.FindObjectsOfTypeAll<GameObject>().FirstOrDefault(
                x => x.name == "Gas Explosion Recycle L"
                ));
        expPrefab.transform.parent = null;
        expPrefab.SetActive(false);
        UnityEngine.Object.DontDestroyOnLoad(expPrefab);
        var de = expPrefab.LocateMyFSM("damages_enemy");
        expPrefab.GetComponent<DamageHero>().damageDealt = 0;
        FSMUtility.SetInt(de, "damageDealt", 45);

        On.HeroController.Start += (orig, self) =>
        {
            orig(self);
            var go = new GameObject("Barrel");
            go.transform.parent = self.transform;
            go.AddComponent<Gun>();
        };
        On.HeroController.Attack += (orig, self, _) => {};
    }
    public static void Fire()
    {
        var barrelGO = new GameObject("Barrel");
        var barrel = barrelGO.AddComponent<Barrel>();
        barrel.rig.velocity = new Vector2(HeroController.instance.cState.facingRight ? 25 : -25, 5);
        barrel.rig.AddTorque(60, ForceMode2D.Force);
        barrel.rig.position = HeroController.instance.GetComponent<Collider2D>().bounds.center;
    }
}
