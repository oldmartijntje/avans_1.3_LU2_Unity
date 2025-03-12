using Assets.scripts.Models;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.scripts
{
    internal class GameSprite: DragDrop
    {
        public Image targetImage;
        public Sprite sprite1;
        public Sprite sprite2;
        public Sprite sprite3;
        public Sprite sprite4;

        public GameObject self;

        public int PixelsPerCoordinate = 16;
        public int PixelsOffset = 16;

        public Object2D ObjectData;

        private int spriteIdentifier;
        private ApiConnecter apiConnecter;
        private SandboxLogic sandboxLogic;

        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            apiConnecter = FindFirstObjectByType<ApiConnecter>();
            FindEnvSelect();
            switch (spriteIdentifier)
            {
                case 0:
                    targetImage.sprite = sprite1;
                    break;
                case 1:
                    targetImage.sprite = sprite2;
                    break;
                case 2:
                    targetImage.sprite = sprite3;
                    break;
                case 3:
                    targetImage.sprite = sprite4;
                    break;
                default:
                    targetImage.sprite = sprite1;
                    break;
            }
        }

        public void SetSprite(int identifier)
        {
            spriteIdentifier = identifier;
        }

        private void FindEnvSelect()
        {
            SandboxLogic sandboxLogicindResult = FindFirstObjectByType<SandboxLogic>();
            if (sandboxLogicindResult != null)
            {
                sandboxLogic = sandboxLogicindResult;
            }
        }

        public override void AfterDrag(PointerEventData data)
        {
            GridSnap(data.position);
        }

        private void GridSnap(Vector2 pos)
        {
            float x = (pos.x / (PixelsPerCoordinate * 2));
            float y = (pos.y / (PixelsPerCoordinate * 2));
            if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
            {
                x = Mathf.FloorToInt(x);
                y = Mathf.FloorToInt(y);
            }
            x = x * 2;
            y = y * 2;
            Debug.Log($"{x} : {y}");
            var env = MainManager.Instance.fullEnvironment2DObject;
            if (y > env.environmentData.MaxHeight)
            {
                y = env.environmentData.MaxHeight;
            } else if (y < 0)
            {
                y = 0;
            }
            if (x > env.environmentData.MaxLength)
            {
                x = env.environmentData.MaxLength;
            } else if (x < 0)
            {
                x = 0;
            }
            x = Mathf.FloorToInt(x);
            y = Mathf.FloorToInt(y);
            Debug.Log($"{x} : {y}"); 
            var location = new Vector2((x * PixelsPerCoordinate) + (PixelsOffset / 2), (y * PixelsPerCoordinate) + (PixelsOffset / 2));
            if (pos.x / PixelsPerCoordinate > 200)
            {
                if (apiConnecter == null)
                {
                    Debug.LogError("No API Connector found!");
                    sandboxLogic.Refresh(true);
                } else
                {
                    location = new Vector2(-10000, -10000);
                    Debug.Log($"api/Object2D/{MainManager.Instance.environmentSelected}");
                    StartCoroutine(apiConnecter.SendAuthDeleteRequest($"/api/Object2D/{ObjectData.Id}", (string result, string error) =>
                    {
                        sandboxLogic.Refresh(false);
                    }));
                }
                
            } else
            {
                if (apiConnecter == null)
                {
                    Debug.LogError("No API Connector found!");
                    sandboxLogic.Refresh(true);
                }
                else
                {
                    Debug.Log($"api/Object2D/{MainManager.Instance.environmentSelected}");
                    ObjectData.PositionX = x;
                    ObjectData.PositionY = y;
                    string jsonString = JsonConvert.SerializeObject(ObjectData);

                    StartCoroutine(apiConnecter.SendAuthPutRequest(jsonString, $"/api/Object2D", (string result, string error) =>
                    {
                        sandboxLogic.Refresh(false);
                    }));
                }
            }
            self.transform.position = location;
        }
    }
}
