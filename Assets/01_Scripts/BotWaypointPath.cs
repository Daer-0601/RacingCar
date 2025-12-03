using UnityEngine;
using System.Collections.Generic;

public class BotWaypointPath : MonoBehaviour
{
    [Header("Waypoints del Camino")]
    public List<Transform> waypoints = new List<Transform>();
    
    [Header("Configuración")]
    public bool loopPath = true; // Si el camino es circular
    public float waypointReachDistance = 2f; // Distancia para considerar que llegó al waypoint

    private int currentWaypointIndex = 0;

    public Transform GetCurrentWaypoint()
    {
        if (waypoints == null || waypoints.Count == 0)
            return null;

        return waypoints[currentWaypointIndex];
    }

    public Transform GetNextWaypoint()
    {
        if (waypoints == null || waypoints.Count == 0)
            return null;

        int nextIndex = currentWaypointIndex + 1;
        
        if (loopPath)
        {
            nextIndex = nextIndex % waypoints.Count;
        }
        else
        {
            nextIndex = Mathf.Min(nextIndex, waypoints.Count - 1);
        }

        return waypoints[nextIndex];
    }

    public bool HasReachedWaypoint(Vector3 position)
    {
        Transform current = GetCurrentWaypoint();
        if (current == null) return false;

        float distance = Vector2.Distance(position, current.position);
        return distance <= waypointReachDistance;
    }

    public void AdvanceToNextWaypoint()
    {
        if (waypoints == null || waypoints.Count == 0) return;

        currentWaypointIndex++;
        
        if (loopPath)
        {
            currentWaypointIndex = currentWaypointIndex % waypoints.Count;
        }
        else
        {
            currentWaypointIndex = Mathf.Min(currentWaypointIndex, waypoints.Count - 1);
        }
    }

    public void ResetPath()
    {
        currentWaypointIndex = 0;
    }

    // Método para encontrar waypoints automáticamente en la escena
    public void FindWaypointsInScene()
    {
        waypoints.Clear();
        
        // Buscar objetos con tag "Waypoint"
        GameObject[] waypointObjects = GameObject.FindGameObjectsWithTag("Waypoint");
        
        if (waypointObjects.Length > 0)
        {
            System.Array.Sort(waypointObjects, (a, b) => 
            {
                // Ordenar por nombre (Waypoint1, Waypoint2, etc.)
                return string.Compare(a.name, b.name);
            });

            foreach (GameObject wp in waypointObjects)
            {
                waypoints.Add(wp.transform);
            }
            
            Debug.Log("BotWaypointPath: Encontrados " + waypoints.Count + " waypoints");
        }
        else
        {
            Debug.LogWarning("BotWaypointPath: No se encontraron waypoints con tag 'Waypoint' en la escena");
        }
    }

    void Start()
    {
        // Si no hay waypoints asignados, intentar encontrarlos automáticamente
        if (waypoints.Count == 0)
        {
            FindWaypointsInScene();
        }
    }
}

