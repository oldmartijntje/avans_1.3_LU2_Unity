using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.scripts
{
    internal class DragDropToGrid: DragDrop
    {
        public int PixelsPerCoordinate = 8;
        public int PixelsOffset = 32;

        public Image targetImage;
        public Sprite sprite1;
        public Sprite sprite2;
        public Sprite sprite3;
        public Sprite sprite4;

        public Vector2 CalculateLocationToGridPos(Vector2 pos)
        {
            float x = pos.x;
            float y = pos.y;


            return new Vector2((x * PixelsPerCoordinate) + (PixelsOffset / 2), (y * PixelsPerCoordinate) + (PixelsOffset / 2));
        }

        public Vector2 CalculateGridToLocationPosWithValidation(Vector2 pos)
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
            }
            else if (y < 0)
            {
                y = 0;
            }
            if (x > env.environmentData.MaxLength)
            {
                x = env.environmentData.MaxLength;
            }
            else if (x < 0)
            {
                x = 0;
            }
            x = Mathf.FloorToInt(x);
            y = Mathf.FloorToInt(y);
            Debug.Log($"{x} : {y}");
            return new Vector2(x, y);
        }
    }
}
