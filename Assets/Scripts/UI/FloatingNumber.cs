using TMPro;
using UnityEngine;

public class FloatingNumber : MonoBehaviour
{
    TextMeshPro text;
    [SerializeField] float speed = 1f;
    float timeToLive = 3f;
    float randomPositionRange = 120f;

    public void SetNumber(float number, Color color)
    {
        text = transform.GetComponent<TextMeshPro>();
        transform.localPosition = new Vector3(Random.Range(-randomPositionRange, randomPositionRange), Random.Range(0, randomPositionRange-30f), 0);

        string textToShow = number.ToString();
        if(number > 0)
        {
            textToShow = "+" + textToShow;
        }
        else if(number == 0) 
        {
            textToShow = "";
        }

        text.SetText(textToShow);
        text.color = color;
    }

    void Update()
    {
        if (timeToLive > 0)
        {
            transform.Translate(Vector3.up * speed);
            timeToLive -= Time.deltaTime;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
