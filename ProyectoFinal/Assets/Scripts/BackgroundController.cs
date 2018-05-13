using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundController : MonoBehaviour {

    public GameObject startBackground;
    public GameObject waterBackground;
    public GameObject[] listBackgrounds;
	public float scrollVelocity = 1f;

    private float waterBackgroundSize;
    private float startBackgroundSize;
    private GameObject currentBackground;
    private GameObject middleBackground;
    private GameObject previousBackground;

	void Start () {
        SpriteRenderer[] sprites = startBackground.GetComponentsInChildren<SpriteRenderer>();

        for (int i = 0; i < sprites.Length; i++)
        {
            startBackgroundSize += sprites[i].bounds.size.y;
        }
        
        sprites = waterBackground.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < sprites.Length; i++)
        {
            waterBackgroundSize += sprites[i].bounds.size.y;
        }

        previousBackground = GameObject.Instantiate(startBackground);

        int rand = Random.Range(0, listBackgrounds.Length);

        middleBackground = GameObject.Instantiate(waterBackground, new Vector2(previousBackground.transform.position.x, waterBackgroundSize), Quaternion.Euler(Vector2.zero));
        currentBackground = GameObject.Instantiate(listBackgrounds[rand], new Vector2(middleBackground.transform.position.x, listBackgrounds[rand].GetComponent<SpriteRenderer>().bounds.size.y), Quaternion.Euler(Vector2.zero));


    }

    void FixedUpdate () {

        if(previousBackground != null && middleBackground != null)
        {
            previousBackground.transform.position += -new Vector3(0f, scrollVelocity, 0f) * Time.fixedDeltaTime;
            middleBackground.transform.position += -new Vector3(0f, scrollVelocity, 0f) * Time.fixedDeltaTime;
        }
        currentBackground.transform.position += -new Vector3(0f, scrollVelocity, 0f) * Time.fixedDeltaTime;

        if (currentBackground.GetComponent<LoadUnloadBackgroundController>().unloadPrevious)
        {
            Destroy(previousBackground);
            Destroy(middleBackground);
            previousBackground = null;
            middleBackground = null;
            currentBackground.GetComponent<LoadUnloadBackgroundController>().unloadPrevious = false;
        }

        if(currentBackground.GetComponent<LoadUnloadBackgroundController>().loadNext)
        {
            previousBackground = currentBackground;
            middleBackground = GameObject.Instantiate(waterBackground, new Vector2(previousBackground.transform.position.x, waterBackgroundSize), Quaternion.Euler(Vector2.zero));
            int rand = Random.Range(0, listBackgrounds.Length);
            currentBackground = GameObject.Instantiate(listBackgrounds[rand], new Vector2(middleBackground.transform.position.x, listBackgrounds[rand].GetComponent<SpriteRenderer>().bounds.size.y), Quaternion.Euler(Vector2.zero));
        }
    }
}
