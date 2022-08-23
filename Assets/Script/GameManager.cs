using System;
using UIScripts;
using UnityEngine;


//클리어 시간이나 게임오버 상태를 관리하는 게임 매니저
    public class GameManager : MonoBehaviour
    {
        private static GameManager instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = FindObjectOfType<GameManager>();
                }
                return m_instance;
            }
        }

        private static GameManager m_instance;


        public static Sprite[] TileSprite;

        private void Awake()
        {
            if (instance != this)
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            TileSprite = Resources.LoadAll<Sprite>("RoadSp");
        }

        public void EndGame()
        {
            UIManagers.Instance.SetActiveGameoverUI(true);
        }
    }