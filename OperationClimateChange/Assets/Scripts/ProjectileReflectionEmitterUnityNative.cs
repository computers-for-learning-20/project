using UnityEditor;
using UnityEngine;

/*
 * Projectile reflection demonstration in Unity 3D
 * 
 * Demonstrates a projectile reflecting in 3D space a variable number of times.
 * Reflections are calculated using Raycast's and Vector3.Reflect
 * 
 * Developed on World of Zero: https://youtu.be/GttdLYKEJAM
 */
public class ProjectileReflectionEmitterUnityNative : MonoBehaviour
{
    public int maxReflectionCount = 5;
    public float maxStepDistance = 5;

    public LineRenderer line;
    void Start()
    {
        // Grabbed our laser.
        line = GetComponent<LineRenderer>();
        line.startWidth = 0.2f;
        line.endWidth = 0.2f;

    }

    void Update(){
        line.SetPosition(0, transform.position);
        line.SetPosition(0, transform.position);

        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit))
        {
            if (hit. collider)
            {
                
                Vector3 direction = Vector3.Reflect(transform.forward, hit.normal);
                line.SetPosition(1, hit.point);
                line.SetPosition(2, hit.point + direction);
            }
        }
        else {line.SetPosition(1, transform.forward*10);}
    }
    
    void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Handles.ArrowHandleCap(0, this.transform.position + this.transform.forward * 0.25f, 
        this.transform.rotation, 0.5f, EventType.Repaint);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(this.transform.position, 0.25f);

        DrawPredictedReflectionPattern(this.transform.position + this.transform.forward * 0.75f, 
        this.transform.forward, maxReflectionCount);
    }

    private void DrawPredictedReflectionPattern(Vector3 position, Vector3 direction, int reflectionsRemaining)
    {
        if (reflectionsRemaining == 0) {
            return;
        }

        Vector3 startingPosition = position;

        Ray ray = new Ray(position, direction);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, maxStepDistance))
        {
            direction = Vector3.Reflect(direction, hit.normal);
            position = hit.point;
        }
        else
        {
            position += direction * maxStepDistance;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawLine(startingPosition, position);

        DrawPredictedReflectionPattern(position, direction, reflectionsRemaining - 1);
    }
}