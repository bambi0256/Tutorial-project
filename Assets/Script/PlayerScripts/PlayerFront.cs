using UnityEngine;

namespace Script.PlayerScripts
{
    public class PlayerFront : MonoBehaviour
    {
        [SerializeField] private bool isBlock = false;
        [SerializeField] private bool isPortal = false;
        [SerializeField] private bool isBreakable = false;

        private void OnTriggerStay2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Obstruction"))
            {
                isBlock = true;
            }
            else if (other.gameObject.CompareTag("Portal"))
            {
                isPortal = true;
            }
        }


        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.gameObject.CompareTag("Obstruction"))
            {
                isBlock = false;
            }
            else if (other.gameObject.CompareTag("Portal"))
            {
                isPortal = false;
            }
        }
    }
}
