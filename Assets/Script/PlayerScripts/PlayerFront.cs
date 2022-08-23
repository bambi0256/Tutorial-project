using UnityEngine;

namespace PlayerScripts
{
    public class PlayerFront : MonoBehaviour
    {
        private GameObject parent;
        private PlayerController playerController;
        private GameObject front;


        private void Start()
        {
            parent = transform.parent.gameObject;
            playerController = parent.GetComponent<PlayerController>();
        }


        private void OnTriggerEnter2D(Collider2D other)
        {
            front = other.gameObject;

            if (!(front.CompareTag("Breakable") || front.CompareTag("InPortal") || front.CompareTag("Turret") || front.CompareTag("Hole") || front.CompareTag("Tile")))
                return;
            
            playerController.setFrontObject(front);
        }


        private void OnTriggerStay2D(Collider2D other)
        {
            front = other.gameObject;

            // layer 8 is Obstruction
            if (front.layer == 8)
                playerController.setIsObstruct(true);
        }


        private void OnTriggerExit2D(Collider2D other)
        {
            front = other.gameObject;

            // layer 8 is Obstruction
            if (front.layer == 8)
                playerController.setIsObstruct(false);
            

            if (!(front.CompareTag("Breakable") || front.CompareTag("InPortal") || front.CompareTag("Turret") || front.CompareTag("Hole") || front.CompareTag("Tile")))
                return;

            playerController.resetFrontObject();
        }
    }
}
