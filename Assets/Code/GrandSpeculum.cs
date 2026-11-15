using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class GrandSpeculum : MonoBehaviour
{
    private int health;
    private int maxHealth = 100;
    public static GrandSpeculum playerHealth;
    [SerializeField] private TMP_Text healthText;
    public Slider healthSlider;
    void Awake()
    {
        playerHealth = this;
    }

    public int Health
    {
        get { return health; }
        
        set 
        { 
            health = value;
            if (health < 0)
            {
                health = 0;
            }
            else if (health > maxHealth)
            {
                health = maxHealth;
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        healthText.text = health.ToString("D3") + "/" + maxHealth.ToString("D3");

        healthSlider.value = HealthPercentage();
    }

    public bool IsBroken()
    {
        if (health == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public float HealthPercentage()
    {
        return (health / (float)maxHealth) * 100;
    }
}
