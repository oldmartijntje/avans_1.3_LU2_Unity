using Assets.scripts.Models;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.scripts
{
    internal class GameSprite: DragDropToGrid
    {
        public GameObject self;

        

        public Object2D ObjectData;

        private int spriteIdentifier;
        private ApiConnecter apiConnecter;
        private SandboxLogic sandboxLogic;

        
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            apiConnecter = FindFirstObjectByType<ApiConnecter>();
            FindSandboxLogic();
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

        private void FindSandboxLogic()
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
            var gridPos = CalculateGridToLocationPosWithValidation(pos);
            var locationPos = CalculateLocationToGridPos(pos);

            if (pos.x / PixelsPerCoordinate > 200)
            {
                if (apiConnecter == null)
                {
                    Debug.LogError("No API Connector found!");
                    sandboxLogic.Refresh(true);
                } else
                {
                    locationPos = new Vector2(-10000, -10000);
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
                    ObjectData.PositionX = gridPos.x;
                    ObjectData.PositionY = gridPos.y;
                    string jsonString = JsonConvert.SerializeObject(ObjectData);

                    StartCoroutine(apiConnecter.SendAuthPutRequest(jsonString, $"/api/Object2D", (string result, string error) =>
                    {
                        sandboxLogic.Refresh(false);
                    }));
                }
            }
            self.transform.position = locationPos;
        }
    }
}
