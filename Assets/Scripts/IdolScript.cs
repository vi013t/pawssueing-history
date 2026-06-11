using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IdolScript : MonoBehaviour, Collectable
{
    Collider2D Collectable.collider => GetComponent<Collider2D>();
    public string nextLevel;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Collect()
    {
        SceneManager.LoadScene(nextLevel);
        Destroy(gameObject);
        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().name);
    }
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Collect();
        }
    }
}
