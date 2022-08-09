using System;
using UnityEngine;

namespace PlayerScripts
{
    public class PlayerAnchor : MonoBehaviour
    {
        [SerializeField] private bool isCannonball;
        [SerializeField] private bool isTileOn;
        private GameObject parent;

        // Start is called before the first frame update
        private void Start()
        {
            parent = transform.parent.gameObject;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Tile"))
            {
                isTileOn = true;
            }
            else if (!other.gameObject.CompareTag("Cannonball")) return;
            isCannonball = true;

            parent.GetComponent<PlayerController>().getShot();
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if(other.gameObject.CompareTag("Tile"))
            {
                isTileOn = false;
            }
        }
    }
}
