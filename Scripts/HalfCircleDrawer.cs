using System.Collections;
using UnityEngine;


namespace Calibration.AutomaticCalibration
{
    public class HalfCircleDrawer : MonoBehaviour
    {
        public enum Plane
        {
            XY,
            XZ,
            YZ
        }

        public int segments = 50;
  public void StartDrawingSemiOval(Vector3 start, Vector3 end, float duration, Plane plane, Vector3 openDirection, LineRenderer lineRenderer, bool semiOval, bool isLeftToRight)
    {
     

        StartCoroutine(DrawSemiOvalIncrementally(start, end, duration, plane, openDirection.normalized, lineRenderer, semiOval, isLeftToRight));
    }

    public IEnumerator DrawSemiOvalIncrementally(Vector3 start, Vector3 end, float duration, Plane plane, Vector3 openDirection, LineRenderer lineRenderer, bool semiOval, bool isLeftToRight)
    {
        float startTime = Time.time;
        Vector3 center = (start + end) / 2;
        float radius = Vector3.Distance(start, end) / 2;

        while (Time.time - startTime < duration)
        {
            float progress = (Time.time - startTime) / duration;
            int currentSegment = Mathf.FloorToInt(progress * segments);
            DrawArc(center, radius, currentSegment, segments, plane, openDirection, lineRenderer,  semiOval, isLeftToRight);
            yield return null;
        }

        DrawArc(center, radius, segments, segments, plane, openDirection, lineRenderer, semiOval, isLeftToRight);
    }
    

    void DrawArc(Vector3 center, float radius, int currentSegment, int totalSegments, Plane plane, Vector3 openDirection, LineRenderer lineRenderer, bool semiOval, bool isLeftToRight)
    {
        lineRenderer.positionCount = currentSegment + 1;

        for (int i = 0; i <= currentSegment; i++)
        {
            //float angle = semiOval?  Mathf.Lerp(0, Mathf.PI, (float)i / totalSegments) :  Mathf.Lerp(0, Mathf.PI / 2, (float)i / segments); //1/4 or 1/2
            float angle;
            if (semiOval) {
                // SemiOval
                if (plane == Plane.YZ)
                {
                    angle = isLeftToRight
                        ? Mathf.Lerp(Mathf.PI, 0, (float)i / totalSegments)
                        : Mathf.Lerp(-Mathf.PI/4, Mathf.PI/4, (float)i / totalSegments);
                }
                else
                {
                    angle = isLeftToRight
                        ? Mathf.Lerp(Mathf.PI, 0, (float)i / totalSegments)
                        : Mathf.Lerp(0, Mathf.PI, (float)i / totalSegments);
                }
            } else {
                // 1/4
                angle = isLeftToRight ? Mathf.Lerp(Mathf.PI, Mathf.PI / 2, (float)i / totalSegments): Mathf.Lerp(0, Mathf.PI / 2, (float)i / totalSegments) ;
            }
            Vector3 point = Vector3.zero;
            if (plane == Plane.XZ) {
                point = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
                
            }
            else
            {
                switch (plane)
                {
                    case Plane.XY:
                        point = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius;
                        break;
                    case Plane.XZ:
                        point = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
                        break;
                    case Plane.YZ:
                        point = new Vector3(0, Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
                        break;
                }

                // Add rotation
                Quaternion rotation = Quaternion.FromToRotation(Vector3.up, openDirection);
                point = rotation * point;
            }
            
         
            
         //plane
            
            
            lineRenderer.SetPosition(i, center + point);
        }
    }
 
 

        public void StartDrawingHalfCircle(Vector3 start, Vector3 end, float duration, LineRenderer lineRenderer,
            Plane plane)
        {
            StartCoroutine(DrawHalfCircleIncrementally(start, end, duration, lineRenderer, plane));
        }

        IEnumerator DrawHalfCircleIncrementally(Vector3 start, Vector3 end, float drawDuration,
            LineRenderer lineRenderer, Plane plane)
        {
            Vector3 center = (start + end) / 2;
            float radius = Vector3.Distance(start, center) / 2; 

            Vector3 up = Vector3.zero;
            switch (plane)
            {
                case Plane.XY:
                    up = Vector3.forward;
                    break;
                case Plane.XZ:
                    up = Vector3.up;
                    break;
                case Plane.YZ:
                    up = Vector3.right;
                    break;
            }

            float drawStartTime = Time.time;

            while (Time.time - drawStartTime < drawDuration)
            {
                float proportionCompleted = (Time.time - drawStartTime) / drawDuration;
                int currentSegment = Mathf.FloorToInt(proportionCompleted * segments);
                DrawArc(lineRenderer, center, radius, start, currentSegment, segments, up, plane);
                yield return null;
            }

            // Dibuja el semicírculo completo al final
            DrawArc(lineRenderer, center, radius, start, segments, segments, up, plane);
        }

        void DrawArc(LineRenderer lr, Vector3 center, float radius, Vector3 start, int currentSegment,
            int totalSegments, Vector3 up, Plane plane)
        {
            lr.positionCount = currentSegment + 1;
            for (int i = 0; i <= currentSegment; i++)
            {
                float angle = Mathf.Lerp(0, Mathf.PI, (float)i / totalSegments); // Medio círculo
                Vector3 direction;
                switch (plane)
                {
                    case Plane.XY:
                        direction = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0);
                        break;
                    case Plane.XZ:
                        direction = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
                        break;
                    case Plane.YZ:
                        direction = new Vector3(0, Mathf.Cos(angle), Mathf.Sin(angle));
                        break;
                    default:
                        direction = Vector3.zero;
                        break;
                }

                Vector3 point = center + direction * radius;
                lr.SetPosition(i, point);
            }
        }
    }
}