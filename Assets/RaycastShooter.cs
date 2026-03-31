using UnityEngine;
using UnityEngine.InputSystem;

public class RaycastShooter : MonoBehaviour
{
    [Header("Shooting")]
    public float range = 100f;
    public int damage = 50;
    public float fireRate = 0.2f;
    public LayerMask hitLayers;

    [Header("References")]
    public Transform shootOrigin;     
    public GunAnimator gunAnimator;    

    private PlayerInput _input;
    private InputAction _shoot;
    private float _nextFireTime;

    [Header("Audio")]
    public AudioClip shootSound;
    public float shootVolume = 0.8f;

    private AudioSource _audioSource;

    void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
            _audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnEnable()
    {
        _shoot = _input.actions.FindAction("Shoot", true);
        _shoot.Enable();
    }

    void Update()
    {
        if (_shoot.IsPressed() && Time.time >= _nextFireTime)
        {
            Fire();
            _nextFireTime = Time.time + fireRate;
        }
    }

    void Fire()
    {
       
        if (gunAnimator != null) gunAnimator.PlayShoot();
        if (shootSound != null)
            _audioSource.PlayOneShot(shootSound, shootVolume);
        Ray ray = new Ray(shootOrigin.position, shootOrigin.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, range, hitLayers))
        {
            EnemyHitbox hitbox = hit.collider.GetComponentInParent<EnemyHitbox>();
            if (hitbox != null)
                hitbox.TakeDamage(damage);
        }
    }

}