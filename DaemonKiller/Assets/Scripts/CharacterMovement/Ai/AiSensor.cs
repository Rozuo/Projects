using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Based on https://www.youtube.com/watch?v=znZXmmyBF-o&ab_channel=TheKiwiCoder
namespace CharacterMovement.AI
{
    /// <summary>
    /// Ai sensor is the eyes of all ai. 
    /// If a target layer will enter it's range it will be detected.
    /// </summary>
    /// 
    /// Author: Rozario (Ross) Beaudin (RB)
    /// 
    /// Public Variables:
    /// distance            Distance of the meshes detection
    /// angle               The angle of the mesh
    /// height              Height of the mesh
    /// meshColor           Color of the mesh for debugging
    /// scanFrequency       How often the ai scans for targets
    /// layers              The layer of the designated target
    /// obstructionLayers   What layers will obstruct vision
    /// gO                  Game objects that have been detected
    /// debug               Determines whether to display the debug of the gizmos.
    /// 
    /// Undeclared access Modifiers:
    /// mesh            The mesh of the sensor
    /// count           Number of game object within the detection radius
    /// scanInterval    Interval of scans
    /// scanTimer       Timer until the next scan
    /// 
    //[ExecuteInEditMode]
    public class AiSensor : MonoBehaviour
    {
        public float distance = 10f;
        public float angle = 30f;
        public float height = 1.0f;
        public Color meshColor = Color.red;
        public int scanFrequency = 30;
        public LayerMask layers;
        public LayerMask obstructionLayers;
        public List<GameObject> gO = new List<GameObject>();
        Collider[] colliders = new Collider[50];
        Mesh mesh;
        int count;
        float scanInterval;
        float scanTimer;

        [Header("Debugging")]
        public bool debug = true;

        /// <summary>
        /// Set the scan interval.
        /// </summary>
        /// 
        /// 2021-06-10 RB Initial Documentation
        /// 
        private void Start()
        {
            scanInterval = 1.0f / scanFrequency;
        }

        /// <summary>
        /// Scans after a set amount of time.
        /// </summary>
        /// 
        /// 2021-06-10 RB Initial Documentation
        /// 
        private void Update()
        {
            scanTimer -= Time.deltaTime;
            if (scanTimer < 0)
            {
                scanTimer += scanInterval;
                Scan();
            }
        }

        /// <summary>
        /// Scan for targets
        /// </summary>
        /// 
        /// 2021-06-10 RB Initial Documentation
        /// 
        private void Scan()
        {
            count = Physics.OverlapSphereNonAlloc(transform.position, distance, colliders, layers, QueryTriggerInteraction.Collide);
            gO.Clear();
            for (int i = 0; i < count; i++)
            {
                GameObject obj = colliders[i].gameObject;
                if (IsInSight(obj))
                {
                    gO.Add(obj);
                }
            }
        }

        /// <summary>
        /// Is the target line of sight
        /// </summary>
        /// 
        /// 2021-06-10 RB Initial Documentation
        /// 
        public bool IsInSight(GameObject gO)
        {
            Vector3 origin = transform.position;
            Vector3 dest = gO.transform.position;
            Vector3 direction = dest - origin;
            if (direction.y < 0 || direction.y > height)
            {
                return false;
            }

            direction.y = 0;
            float deltaAngle = Vector3.Angle(direction, transform.forward);
            if(deltaAngle > angle)
            {
                return false;
            }

            origin.y += height / 2;
            dest.y = origin.y;
            if(Physics.Linecast(origin, dest, obstructionLayers))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Create the mesh for the detection of targets.
        /// </summary>
        /// 
        /// 2021-06-10 RB Initial Documentation
        /// 
        Mesh CreateWedgeMesh()
        {
            Mesh mesh = new Mesh();

            int segements = 10;
            // the two + 2 are for the left and the right side while the *4 is for the front side and the top and bottom
            int numTrangles = (segements * 4) + 2 + 2;
            int numVertices = numTrangles * 3;

            Vector3[] vertices = new Vector3[numVertices];
            int[] triangles = new int[numVertices];

            Vector3 bottomCenter = Vector3.zero;
            Vector3 bottomLeft = Quaternion.Euler(0, -angle, 0) * Vector3.forward * distance;
            Vector3 bottomRight = Quaternion.Euler(0, angle, 0) * Vector3.forward * distance;

            Vector3 topCenter = bottomCenter + Vector3.up * height;
            Vector3 topRight = bottomRight + Vector3.up * height;
            Vector3 topLeft = bottomLeft + Vector3.up * height;

            int vert = 0;

            // left side of triangle
            vertices[vert++] = bottomCenter;
            vertices[vert++] = bottomLeft;
            vertices[vert++] = topLeft;

            vertices[vert++] = topLeft;
            vertices[vert++] = topCenter;
            vertices[vert++] = bottomCenter;

            // right side of triangle
            vertices[vert++] = bottomCenter;
            vertices[vert++] = topCenter;
            vertices[vert++] = topRight;

            vertices[vert++] = topRight;
            vertices[vert++] = bottomRight;
            vertices[vert++] = bottomCenter;

            float currentAngle = -angle;
            float deltaAngle = (angle * 2 / segements);
            for(int i = 0; i < segements; i++)
            {
                bottomLeft = Quaternion.Euler(0, currentAngle, 0) * Vector3.forward * distance;
                bottomRight = Quaternion.Euler(0, currentAngle + deltaAngle, 0) * Vector3.forward * distance;

                topRight = bottomRight + Vector3.up * height;
                topLeft = bottomLeft + Vector3.up * height;
                
                // front side of triangle
                vertices[vert++] = bottomLeft;
                vertices[vert++] = bottomRight;
                vertices[vert++] = topRight;

                vertices[vert++] = topRight;
                vertices[vert++] = topLeft;
                vertices[vert++] = bottomLeft;

                // top of triangle
                vertices[vert++] = topCenter;
                vertices[vert++] = topLeft;
                vertices[vert++] = topRight;

                // bottom of triangle
                vertices[vert++] = bottomCenter;
                vertices[vert++] = bottomRight;
                vertices[vert++] = bottomLeft;

                currentAngle += deltaAngle;
            }
            

            for(int i = 0; i < numVertices; i++)
            {
                triangles[i] = i;
            }

            mesh.vertices = vertices;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();

            return mesh;
        }

        /// <summary>
        /// Updates whenever any values of AiSensor is updated or when loaded.
        /// </summary>
        /// 
        /// 
        /// 
        private void OnValidate()
        {
            mesh = CreateWedgeMesh();
            scanInterval = 1.0f / scanFrequency;
        }

        /// <summary>
        /// Returns the number of detect gameobjects
        /// </summary>
        /// <returns>The game objects that are detected.</returns>
        /// 
        /// 2021-06-10 RB Initial Documentation
        /// 
        public List<GameObject> GetGo()
        {
            return gO;
        }

        /// <summary>
        /// Debugging only. It will display the line of sight wedge and detection sphere.
        /// If a target is able to be detected a red sphere will appear. If the target can be detected
        /// and will turn green if it is detected.
        /// </summary>
        /// 
        /// 2021-06-10 RB Initial Documentation
        /// 
        private void OnDrawGizmos()
        {
            if (debug)
            {
                if (mesh)
                {
                    Gizmos.color = meshColor;
                    Gizmos.DrawMesh(mesh, transform.position, transform.rotation);
                }

                Gizmos.DrawWireSphere(transform.position, distance);
                for(int i = 0; i < count; i++)
                {
                    Gizmos.DrawSphere(colliders[i].transform.position, 0.2f);
                }

                Gizmos.color = Color.green;
                foreach (var obj in gO)
                {
                    Gizmos.DrawSphere(obj.transform.position, 0.2f);
                }
            }
            
        }
    }
}

