using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementController : MonoBehaviour
{
    [Tooltip("The desired movement speed of character.")]
    public float moveSpeed = 20.0f;
    //public Pathfinder pathfinder;

    Animator animController;
    Camera   cam;

    // start and end points of movement
    Vector3 startPos;
    Vector3 startObjectPos;

    Vector3 targetPos;
    Vector3 targetObjectPos;

    List<Vector3> pathPos = new List<Vector3>();
    // linear interpolation parameter t
    float t = 0.0f;

    // co-routine method variable (pointer)
    IEnumerator moveCoRoutine;

    Pathfinder pathfinder;

    private IEnumerator MoveToLocation()
    {
        startObjectPos = GetStartPos();

        Debug.Log("StartObjectPose" + startObjectPos);
        Debug.Log("targetObjectPose" + targetObjectPos);

        startPos = transform.position;

        pathPos = pathfinder.PathFind(startObjectPos, targetObjectPos);
        pathPos.RemoveAt(0);
        pathPos.RemoveAt(pathPos.Count - 1);
        pathPos.Insert(0, startPos);
        pathPos.Add(targetPos);
        

        Debug.Log("got the pos list");

        
        // Iterate over the path positions
        foreach (Vector3 targetPos in pathPos)
        {
            Vector3 adjustedTargetPos = new Vector3(targetPos.x, 0.5f, targetPos.z);
            t = 0f; // Reset interpolation parameter for each new target position

            // Keep interpolating until reaching the target position
            while (t < 1.0f)
            {
                // Call animation
                animController.SetBool("Run", true);

                // Interpolate character's position towards the target position
                transform.position = Vector3.Lerp(startPos, adjustedTargetPos, t);

                // Calculate direction vector towards the target position
                Vector3 direction = (adjustedTargetPos - transform.position).normalized;

                // Rotate the player to look in the direction of movement
                if (direction != Vector3.zero)
                {
                    Quaternion lookRotation = Quaternion.LookRotation(direction);
                    transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 6f);
                }

                // Increment interpolation parameter based on movement speed
                t += Time.deltaTime * moveSpeed / Vector3.Distance(startPos, adjustedTargetPos);

                // Wait for the next frame
                yield return null;
            }

            // Set the character's position to the target position to ensure accuracy
            transform.position = adjustedTargetPos;

            // Update start position for the next iteration
            startPos = adjustedTargetPos;
        }

        animController.SetBool("Run", false); 
    }


    public Vector3 GetStartPos()
    {
        Vector3 raycastOrigin = transform.position + Vector3.up * 2f;
        RaycastHit[] hits = Physics.RaycastAll(raycastOrigin, Vector3.down);

        GameObject floorObject = null;
        foreach (var hit in hits)
        {
            if (hit.collider.gameObject.name == "Cube_Floor")
            {
                floorObject = hit.collider.gameObject;
                break;
            }
        }

        if (floorObject != null)
        {
            Vector3 ObjectPos = floorObject.transform.position;
            Debug.Log(floorObject.name);
            return ObjectPos;
        }
        else
        {
            Debug.Log("No floor object found!");
            return transform.position;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        // get the animation controller on startup
        animController = GetComponent<Animator>();

        // find and save reference to scene camera
        cam = (Camera) FindObjectOfType (typeof (Camera));

        // Initialize pathfinder
        pathfinder = FindObjectOfType<Pathfinder>();
        if (pathfinder == null)
        {
            Debug.LogError("Pathfinder component not found!");
            return;
        }

    }

    // Update is called once per frame
    
            
    void Update()
    {
        // Left mouse button (on map): Move to location
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            pathPos = new List<Vector3>();
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                GameObject hitObject = hit.collider.gameObject;
                targetPos = hit.point;
                targetObjectPos = hitObject.transform.position;

                // Check if there's an existing movement coroutine
                if (moveCoRoutine != null)
                {
                    // If so, stop it to allow for the new movement
                    StopCoroutine(moveCoRoutine);
                }

                // Store a pointer to the active coroutine
                moveCoRoutine = MoveToLocation();

                // Execute coroutine to carry out move
                StartCoroutine(moveCoRoutine);
            }
        }

        // Right mouse button or spacebar: Magical sword attack
        if (Input.GetKeyDown(KeyCode.Mouse1) || Input.GetKeyDown(KeyCode.Space))
        {
            animController.SetTrigger("Attack");
        }
    }

}
