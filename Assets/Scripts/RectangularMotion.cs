using UnityEngine;

public class SharedRectangleOppositeMotion : MonoBehaviour
{
    [Header("Objects")]
    public GameObject sphere1;  // leading sphere
    public GameObject sphere2;  // trailing sphere (half cycle behind)

    [Header("Motion Settings")]
    public float speed = 1f;

    [Header("Rectangle Definition")]
    public Vector3 topRight = new Vector3(0.691f, 2.046f, 3.46f);

    private float width = 2f * 0.691f;       // 1.382
    private float height = 2.046f + 0.0476f;  // 2.0936

    private Vector3 topLeft;
    private Vector3 bottomLeft;
    private Vector3 bottomRight;

    private float perimeter;

    // Progress variables (distance along path)
    private float t1; // sphere1
    private float t2; // sphere2, offset

    void Start()
    {
        // Compute rectangle corners
        topLeft = new Vector3(topRight.x - width, topRight.y, topRight.z);
        bottomLeft = new Vector3(topLeft.x, topLeft.y - height, topRight.z);
        bottomRight = new Vector3(topRight.x, topRight.y - height, topRight.z);

        perimeter = 2f * (width + height);

        // Initialize progress values
        t1 = 0f;
        t2 = perimeter / 2f; // half the perimeter apart (opposite)

        // Setup trails
        SetupTrail(sphere1, new Color(0.2f, 0.9f, 0.2f)); // green
        SetupTrail(sphere2, new Color(0.9f, 0.2f, 0.2f)); // red

        // Position them at their starting points on the path
        if (sphere1 != null) sphere1.transform.position = PositionOnRectCCW(t1);
        if (sphere2 != null) sphere2.transform.position = PositionOnRectCCW(t2);
    }

    void Update()
    {
        float move = speed * Time.deltaTime;

        t1 += move;
        if (t1 >= perimeter) t1 -= perimeter;

        t2 += move;
        if (t2 >= perimeter) t2 -= perimeter;

        sphere1.transform.position = PositionOnRectCCW(t1);
        sphere2.transform.position = PositionOnRectCCW(t2);
    }


    // Moves counterclockwise: top-right → top-left → bottom-left → bottom-right → back up
    Vector3 PositionOnRectCCW(float d)
    {
        if (d < width) // top edge (→ left)
            return new Vector3(topRight.x - d, topRight.y, topRight.z);
        else if (d < width + height) // left edge (↓)
            return new Vector3(topLeft.x, topLeft.y - (d - width), topRight.z);
        else if (d < 2 * width + height) // bottom edge (→ right)
            return new Vector3(topLeft.x + (d - (width + height)), bottomLeft.y, topRight.z);
        else // right edge (↑)
            return new Vector3(topRight.x, bottomRight.y + (d - (2 * width + height)), topRight.z);
    }

    void SetupTrail(GameObject obj, Color color)
    {
        if (obj == null) return;

        // Color the sphere itself
        var rend = obj.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material = new Material(Shader.Find("Standard"));
            rend.material.color = color;
        }

        // Add TrailRenderer
        var trail = obj.AddComponent<TrailRenderer>();
        trail.time = 1.5f; // shorter tail, faster fading
        trail.minVertexDistance = 0.01f; // smoother curves
        trail.widthCurve = new AnimationCurve(
            new Keyframe(0f, 0.06f),
            new Keyframe(0.3f, 0.08f),
            new Keyframe(1f, 0f)
        );
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        trail.receiveShadows = false;
        trail.autodestruct = false;

        // Use a transparent material for smooth fading
        Material mat = new Material(Shader.Find("Sprites/Default"));
        mat.color = color;
        mat.renderQueue = 3000; // ensure it renders on top
        trail.material = mat;

        // Color gradient: starts bright, fades to transparent
        Gradient g = new Gradient();
        g.SetKeys(
            new GradientColorKey[] {
            new GradientColorKey(color, 0f),
            new GradientColorKey(color * 0.5f, 0.5f),
            new GradientColorKey(color * 0.1f, 1f)
            },
            new GradientAlphaKey[] {
            new GradientAlphaKey(1f, 0f),
            new GradientAlphaKey(0.7f, 0.5f),
            new GradientAlphaKey(0f, 1f)
            }
        );
        trail.colorGradient = g;
    }

}
