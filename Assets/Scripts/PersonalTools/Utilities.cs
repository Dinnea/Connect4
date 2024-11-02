using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Personal.Utilities
{
    public static class TextTools
    {
        /// <summary>
        /// Create Text in the World default
        /// </summary>
        /// <param name="text"></param>
        /// <param name="parent"></param>
        /// <param name="localPosition"></param>
        /// <param name="fontSize"></param>
        /// <param name="color"></param>
        /// <param name="rotation"></param>
        /// <param name="textAnchor"></param>
        /// <param name="textAlignment"></param>
        /// <returns></returns>
        public static TextMesh CreateTextInWorld(string text, Transform parent = null, Vector3 localPosition = default, int fontSize = 40, Color? color = null,
                                               Vector3 rotation = default, TextAnchor textAnchor = TextAnchor.UpperLeft, TextAlignment textAlignment = TextAlignment.Left)
        {
            if (color == null) color = Color.white;
            return CreateTextInWorld(parent, text, localPosition, fontSize, (Color)color, rotation, textAnchor, textAlignment);
        }

        /// <summary>
        /// Create Text in the World, customized
        /// </summary>
        /// <param name="text"></param>
        /// <param name="parent"></param>
        /// <param name="localPosition"></param>
        /// <param name="fontSize"></param>
        /// <param name="color"></param>
        /// <param name="rotation"></param>
        /// <param name="textAnchor"></param>
        /// <param name="textAlignment"></param>
        /// <returns></returns>
        public static TextMesh CreateTextInWorld(Transform parent, string text, Vector3 localPosition, int fontSize, Color color,
                                               Vector3 rotation, TextAnchor textAnchor, TextAlignment textAlignment)
        {
            GameObject gameObject = new GameObject("World_Text", typeof(TextMesh));
            Transform transform = gameObject.transform;
            transform.SetParent(parent, false);
            transform.localPosition = localPosition;
            transform.rotation = Quaternion.Euler(rotation);
            TextMesh textMesh = gameObject.GetComponent<TextMesh>();
            textMesh.anchor = textAnchor;
            textMesh.alignment = textAlignment;
            textMesh.text = text;
            textMesh.fontSize = fontSize;
            textMesh.color = color;
            return textMesh;
        }

    }
    public static class VectorMath
    {
        public static Vector3 GetRandomDirXZ()
        {
            return new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f)).normalized;
        }
        public static Vector3 GetVectorFromAngle(float angle)
        {
            // angle = 0 -> 360
            float angleRad = angle * (Mathf.PI / 180f);
            return new Vector3(Mathf.Sin(angleRad), Mathf.Cos(angleRad));
        }
        public static Vector3 GetVectorFromAngleXZ(float angle)
        {
            // angle = 0 -> 360
            float angleRad = angle * (Mathf.PI / 180f);
            return new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));
        }
        public static Vector3 GetVectorFromAngleXZ(float angle, Transform transform, bool isAngleGlobal = false)
        {
            if (!isAngleGlobal)
            {
                angle += transform.eulerAngles.y;

            }
            // angle = 0 -> 360
            float angleRad = angle * (Mathf.PI / 180f);
            return new Vector3(Mathf.Sin(angleRad), 0, Mathf.Cos(angleRad));
        }
        public static float GetAngleFromVectorFloat(Vector3 dir)
        {
            dir = dir.normalized;
            float floatAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            if (floatAngle < 0) floatAngle += 360;

            return floatAngle;
        }
        public static int GetAngleFromVector(Vector3 dir)
        {
            float floatAngle = GetAngleFromVectorFloat(dir);
            int intAngle = Mathf.RoundToInt(floatAngle);

            return intAngle;
        }
        public static Vector3 GetMousePositionWorld(Camera camera, LayerMask layermask)
        {
            Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, layermask))
            {
                return raycastHit.point;
            }
            return default;

        }
    }
    public static class Search
    {
        public static T[] FindComponentsInChildrenWithTag<T>(this GameObject parent, string tag, bool onlyActive = false) where T : Component
        {
            List<T> list = new List<T>(parent.GetComponentsInChildren<T>(onlyActive));
            if (list.Count == 0) return null;

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i].CompareTag(tag) == false) list.RemoveAt(i);
            }
            return list.ToArray();
        }
        public static T FindComponentInChildrenWithTag<T>(GameObject parent, string tag, bool onlyActive = false) where T : Component
        {
            T[] array = parent.FindComponentsInChildrenWithTag<T>(tag, onlyActive);
            return array[0];
        }
    }
}
