using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

// Check this out we can require components be on a game object!
[RequireComponent(typeof(MeshFilter))]

public class BParticleSimMesh : MonoBehaviour
{
    public struct BSpring
    {
        public float kd;                        // damping coefficient
        public float ks;                        // spring coefficient
        public float restLength;                // rest length of this spring
        public int attachedParticle;            // index of the attached other particle (use me wisely to avoid doubling springs and sprign calculations)
    }

    public struct BContactSpring
    {
        public float kd;                        // damping coefficient
        public float ks;                        // spring coefficient
        public float restLength;                // rest length of this spring (think about this ... may not even be needed o_0
        public Vector3 attachPoint;             // the attached point on the contact surface
    }

    public struct BParticle
    {
        public Vector3 position;                // position information
        public Vector3 velocity;                // velocity information
        public float mass;                      // mass information
        public BContactSpring contactSpring;    // Special spring for contact forces
        public bool attachedToContact;          // is thi sparticle currently attached to a contact (ground plane contact)
        public List<BSpring> attachedSprings;   // all attached springs, as a list in case we want to modify later fast
        public Vector3 currentForces;           // accumulate forces here on each step        
    }

    public struct BPlane
    {
        public Vector3 position;                // plane position
        public Vector3 normal;                  // plane normal
    }

    public float contactSpringKS = 1000.0f;     // contact spring coefficient with default 1000
    public float contactSpringKD = 20.0f;       // contact spring daming coefficient with default 20

    public float defaultSpringKS = 100.0f;      // default spring coefficient with default 100
    public float defaultSpringKD = 1.0f;        // default spring daming coefficient with default 1

    public bool debugRender = true;            // To render or not to render


    /*** 
     * I've given you all of the above to get you started
     * Here you need to publicly provide the:
     * - the ground plane transform (Transform)
     * - handlePlaneCollisions flag (bool)
     * - particle mass (float)
     * - useGravity flag (bool)
     * - gravity value (Vector3)
     * Here you need to privately provide the:
     * - Mesh (Mesh)
     * - array of particles (BParticle[])
     * - the plane (BPlane)
     ***/

    public Transform groundPlaneTrans;
    public bool handlePlaneCollisions;
    public float particleMass;
    public bool use_gravity = true;
    public Vector3 gravity = new Vector3(0f, -9.8f, 0f);

    private Mesh mesh;
    private BParticle[] particleArray;
    private BPlane plane;



    /// <summary>
    /// Init everything
    /// HINT: in particular you should probbaly handle the mesh, init all the particles, and the ground plane
    /// HINT 2: I'd for organization sake put the init particles and plane stuff in respective functions
    /// HINT 3: Note that mesh vertices when accessed from the mesh filter are in local coordinates.
    ///         This script will be on the object with the mesh filter, so you can use the functions
    ///         transform.TransformPoint and transform.InverseTransformPoint accordingly 
    ///         (you need to operate on world coordinates, and render in local)
    /// HINT 4: the idea here is to make a mathematical particle object for each vertex in the mesh, then connect
    ///         each particle to every other particle. Be careful not to double your springs! There is a simple
    ///         inner loop approach you can do such that you attached exactly one spring to each particle pair
    ///         on initialization. Then when updating you need to remember a particular trick about the spring forces
    ///         generated between particles. 
    /// </summary>

    /// creates particles from mesh vertices and adds to particleArray
void addParticles()
{
    particleArray = new BParticle[mesh.vertices.Length];

    for (int i = 0; i < particleArray.Length; i++)
    {
        BParticle particle = new BParticle();
        particle.position = transform.TransformPoint(mesh.vertices[i]);
        particle.velocity = Vector3.zero;
        particle.mass = particleMass;

        particle.attachedSprings = new List<BSpring>();

        particleArray[i] = particle;

    }
}

void addSprings()
{
    for (int i = 0; i < particleArray.Length; i++)
    {
        for (int j = i + 1; j < particleArray.Length; j++)
        {
            BSpring spring = new BSpring();

            spring.kd = defaultSpringKD;
            spring.ks = defaultSpringKS;

            spring.restLength = Vector3.Distance(
                particleArray[i].position,
                particleArray[j].position);

            spring.attachedParticle = j;

                particleArray[i].attachedSprings.Add(spring);

        }
    }
}
void Start()
{
    mesh = GetComponent<MeshFilter>().mesh;

    addParticles();

    addSprings();

}

    //updateMesh loop thru every particle, in this loop accumulate force from every spring,
    //then change particle's position (f = ma)
/*void updateMesh(p)
    {
        for 
    }
*/
    /*** BIG HINT: My solution code has as least the following functions
     * InitParticles()
     * InitPlane()
     * UpdateMesh() (remember the hint above regarding global and local coords)
     * ResetParticleForces()
     * ...
     ***/



    /// <summary>
    /// Draw a frame with some helper debug render code
    /// </summary>
public void Update()
{
    /* This will work if you have a correctly made particles array */
    if (debugRender)
    {
        for (int i = 0; i < particleArray.Length; i++)
        {
            // Draw particle position
            Debug.DrawLine(
                particleArray[i].position,
                particleArray[i].position + Vector3.up * 0.05f,
                Color.blue
            );

            // Draw all springs connected to this particle
            for (int j = 0; j < particleArray[i].attachedSprings.Count; j++)
            {
                int otherIndex =
                    particleArray[i].attachedSprings[j].attachedParticle;

                Debug.DrawLine(
                    particleArray[i].position,
                    particleArray[otherIndex].position,
                    Color.red
                );
            }
        }
    }
}
}
