using UnityEngine;
using UnityEngine.UI;

namespace Script.UIScripts
{
    public class Sound : MonoBehaviour
    {
        private Sprite soundOnImage;
        public Sprite soundOffImage;
        public Button button;
        private bool isOn = true;
        public AudioSource audioSource;

        private void Start()
        {
            soundOnImage = button.image.sprite;
        }

        public void ButtonOnClicked()
        {
            if (isOn)
            {
                button.image.sprite = soundOffImage;
                isOn = false;
                audioSource.mute = true;
            }
            else
            {
                button.image.sprite = soundOnImage;
                isOn = true;
                audioSource.mute = false;
            }
        }
    }
}
