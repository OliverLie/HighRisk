using UnityEngine;
using TMPro;


public class HeightCounter : MonoBehaviour
{
    [SerializeField] Transform reference;
    [SerializeField] Transform player;
    [SerializeField] private TextMeshProUGUI mytext;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        CalculateHeight();
    }

    void CalculateHeight()
    {
        float distance = -reference.position.y - -player.position.y;
        mytext.text =((distance/9)*10).ToString("F0") + "\nMeters";
        
    }
}
