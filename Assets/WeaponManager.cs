using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;


public class WeaponManager : MonoBehaviour
{
    [System.Serializable]
    public class Weapon
    {
        public string name;
        public GameObject gunObject;    
        public GunAnimator gunAnimator; 
        public int damage;
        public float fireRate;
        public float range = 100000f;
    }

    [Header("Weapons")]
    public List<Weapon> weapons = new List<Weapon>();

    [Header("References")]
    public RaycastShooter shooter;

    private int _currentIndex = 0;
    private PlayerInput _input;
    private InputAction _gun1;
    private InputAction _gun2;

    void Awake() { }

    void OnEnable() { }

    void Start()
    {
        _input = GetComponent<PlayerInput>();

        _gun1 = _input.actions.FindAction("Gun1", false);
        _gun2 = _input.actions.FindAction("Gun2", false);

        _gun1?.Enable();
        _gun2?.Enable();

        if (weapons.Count > 0)
            EquipWeapon(0);
    }

    void Update()
    {
        if (_gun1 != null && _gun1.triggered) EquipWeapon(0);
        if (_gun2 != null && _gun2.triggered) EquipWeapon(1);
    }

    void EquipWeapon(int index)
    {
        if (index >= weapons.Count) return;

        _currentIndex = index;

       
        for (int i = 0; i < weapons.Count; i++)
            if (weapons[i].gunObject != null)
                weapons[i].gunObject.SetActive(i == index);

        
        Weapon w = weapons[index];
        shooter.damage = w.damage;
        shooter.fireRate = w.fireRate;
        shooter.range = w.range;
        shooter.gunAnimator = w.gunAnimator;

        Debug.Log($"Equipped {w.name} — damage: {w.damage}, fireRate: {w.fireRate}");
    }
}
