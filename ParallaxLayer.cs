using UnityEngine;

namespace Platformer.View
{
    /// <summary>
    /// Used to move a transform relative to the main camera position with a scale factor applied.
    /// This is used to implement parallax scrolling effects on different branches of gameobjects.
    /// </summary>
    public class ParallaxLayer : MonoBehaviour
    {
        [SerializeField] private Vector2 parallaxEffectMultiplier;
        private Transform cameraTransform;
        private Vector3 lastCameraPosition;
        //private float textureUnitSizeX;                             // Used in the infinite Scroll

        void Start()
        {
            //Initialization of key variables
            cameraTransform = Camera.main.transform;
            lastCameraPosition = cameraTransform.position;
            Sprite sprite = GetComponent<SpriteRenderer>().sprite;
            Texture2D texture = sprite.texture;
           //textureUnitSizeX = texture.width / sprite.pixelsPerUnit;   // Used for the infinite scroll
        }

        void LateUpdate()
        {
            //Parallax Camera Implementation            
            Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
            transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier.x, deltaMovement.y * parallaxEffectMultiplier.y);
            lastCameraPosition = cameraTransform.position;
            
            /* This is code to control the infinite scroll capability however this is for a much later implementation of this works, in the optimization phase*/
            //if(Mathf.Abs(cameraTransform.position.x - transform.position.x)>= textureUnitSizeX)
            //{
            //    float offsetPositionX = (6 * cameraTransform.position.x - 6 * transform.position.x)%textureUnitSizeX;
            //    transform.position = new Vector3(cameraTransform.position.x + offsetPositionX, cameraTransform.position.y);
            //}
        
        }

    }
}