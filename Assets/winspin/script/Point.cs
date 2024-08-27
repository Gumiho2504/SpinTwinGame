using UnityEngine;

public class Point : MonoBehaviour
{
    private Rigidbody2D rb;

    private void Awake()
    {
        
        rb = GetComponent<Rigidbody2D>();
    }

    //private void onTri(Collision2D collision)
    //{
       
    //    if (collision.gameObject.CompareTag("point"))
    //    {
    //         AudioController.Instance.PlaySFX("point");
          
    //    }
        
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("item"))
        {
            AudioController.Instance.PlaySFX("point");

        }
    }
}
