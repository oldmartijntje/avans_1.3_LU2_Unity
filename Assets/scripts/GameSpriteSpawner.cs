using Assets.scripts.Models;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.scripts
{
    internal class GameSpriteSpawner: DragDropToGrid
    {
        private Vector2 BaseLocation;
        private int spriteIdentifier;
        private ApiConnecter apiConnecter;
        public GameObject self;
        private SandboxLogic sandboxLogic;

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

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            apiConnecter = FindFirstObjectByType<ApiConnecter>();
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
            FindSandboxLogic();
        }

        public override void AfterDrag(PointerEventData data)
        {
            Debug.Log(data.position);
            if (data.position.x > 1100)
            {
                ReturnHome();
                return;
            }
            if (apiConnecter == null)
            {
                Debug.LogError("No API Connector found!");
                sandboxLogic.Refresh(true);
            }
            else
            {
                var gridPos = CalculateGridToLocationPosWithValidation(data.position);
                Debug.Log($"api/Object2D/{MainManager.Instance.environmentSelected}");
                var object2d = new Object2DTemplate()
                {
                    PositionX = gridPos.x,
                    PositionY = gridPos.y,
                    PrefabId = spriteIdentifier,
                    ScaleX = 1,
                    ScaleY = 1,
                    RotationZ = 1,
                    SortingLayer = 0,
                    EnvironmentId = MainManager.Instance.environmentSelected
                };
                string jsonString = JsonConvert.SerializeObject(object2d);

                StartCoroutine(apiConnecter.SendAuthPostRequest(jsonString, $"/api/Object2D", (string result, string error) =>
                {
                    ReturnHome();
                    sandboxLogic.Refresh(false);
                }));
            }
        }

        public void ReturnHome()
        {
            self.transform.position = BaseLocation;
        }

        public void SetBasePos(Vector2 pos)
        {
            BaseLocation = pos;
        }
    }
}
