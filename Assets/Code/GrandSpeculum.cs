using UnityEngine;
using TMPro;

public class GrandSpeculum : MonoBehaviour
{
    private int health;
    private int maxHealth = 100;
    public static GrandSpeculum playerHealth;
    [SerializeField] private TMP_Text healthText;
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

    public double HealthPercentage()
    {
        return health / (double)maxHealth;
    }
}
