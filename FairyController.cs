using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyController : MonoBehaviour
{
    [SerializeField] private ParticleSystem particle;

    private SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start() 
    {
        particle.GetComponent<Renderer>().sortingLayerName = "Background";
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Lets the fairy orbit around the parent object, in this case the Player
        transform.position = RotatePointAroundPivot(transform.position, transform.parent.position, Quaternion.Euler(0, 250 * Time.deltaTime, 0));
        // Flips the sprite according to the parent object location, could also use two points and make it flip upon reaching those points. Future works note.
        switch (transform.position.x > transform.parent.position.x)
        {
            case true:
                sprite.flipX = true;
                break;
            case false:
                sprite.flipX = false;
                break;
        }
    }

    public static Vector3 RotatePointAroundPivot(Vector3 point, Vector3 pivot, Quaternion angle)
    {
        return angle * (point - pivot) + pivot;
    }
}
