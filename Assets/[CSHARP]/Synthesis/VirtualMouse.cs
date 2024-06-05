using Darklight.UnityExt.Input;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// Hacky system to work with Unity's UI using events.
/// </summary>
/// <remarks>
/// FIXME: This only works for UXML objects when the cursor is moving to the left or up. For whatever reason.
/// Some hypotheses: Maybe it's just the nature of the function I'm using to call it?
/// Maybe it's something to do with the <see cref="position"/> and <see cref="deltaPosition"/> variables?
/// Maybe it's the call order of the events?
/// I tried to revise these to be PointerDown, then PointerMove, then PointerUp (per the documentation), but maybe that's wrong.
/// Maybe I just need to follow this tutorial? https://docs.unity3d.com/6000.0/Documentation/Manual/UIE-Events-Synthesizing.html
/// </remarks>
public class VirtualMouse : IPointerEvent
{
    #region Interface Stuff We Need
    public Vector3 position { get; protected set; }

    public int pointerId => 2;

    public string pointerType => "Virtual";

    public bool isPrimary => false;

    public int button => -1;

    public int pressedButtons => 0;

    public Vector3 localPosition => position;

    public Vector3 deltaPosition {get; protected set; }

    public float deltaTime { get; protected set; }

    public int clickCount => 0;
    #endregion

    public void Awake()
    {
        position = Vector2.zero;
    }

    protected void Start() {
        //Invoke("Initialize", 0.1f);
    }

    void Initialize() {
        if (UniversalInputManager.Instance == null) { Debug.LogError("UniversalInputManager is not initialized"); return; }
        UniversalInputManager.OnMoveInput += MoveCursor;
        UniversalInputManager.OnMoveInputCanceled += StopMoveCursor;
        UniversalInputManager.OnPrimaryInteract += Click;
        UniversalInputManager.OnPrimaryInteractCanceled += StopClick;

        /*var e = new Event();
        e.type = EventType.MouseEnterWindow;
        e.mousePosition = position;
        e.delta = position;
        e.displayIndex = 0;
        var mouseEvent = PointerEnterEvent.GetPooled(e);
        mouseEvent.target = activeHook;
        activeHook.SendEvent(mouseEvent);*/
    }

    Vector2 intendedMoveDir;
    void MoveCursor(Vector2 move)
    {
        intendedMoveDir = move;
        intendedMoveDir.y *= -1;
    }

    void StopMoveCursor()
    {
        intendedMoveDir = Vector2.zero;
    }

    /// <summary>
    /// Can we send move events?
    /// </summary>
    bool downSent = false;
    bool sendDown = false;
    void Click()
    {
        sendDown = true;
    }

    bool sendUp = false;
    void StopClick()
    {
        sendUp = true;
    }

    VisualElement activeHook;
    void Update() {
        deltaTime = Time.deltaTime;

        Vector3 oldPos = position;
        if (intendedMoveDir != Vector2.zero) {
            position += deltaTime * 100 * new Vector3(intendedMoveDir.x, intendedMoveDir.y, 0);
            position = new Vector3(Mathf.Clamp(position.x, 0, Screen.width), Mathf.Clamp(position.y, 0, Screen.height));
        }
        deltaPosition = position - oldPos;

        if (activeHook != null) {
            if (sendUp) {
                downSent = false;
                sendUp = false;
                // To dispose when we're done:
                using (PointerUpEvent eUp = PointerUpEvent.GetPooled(this)) {
                    activeHook.SendEvent(eUp);
                }
            } else if (downSent) {
                // TODO: Why does this only work when moving left or up?
                using (PointerMoveEvent eMove = PointerMoveEvent.GetPooled(this)) {
                    activeHook.SendEvent(eMove);
                }
            }

            if (sendDown) {
                downSent = true;
                sendDown = false;
                using (PointerDownEvent eDown = PointerDownEvent.GetPooled(this)) {
                    activeHook.SendEvent(eDown);
                }
            }
        }
    }

    /// <summary>
    /// Set a thing we're gonna call events on.
    /// </summary>
    /// <param name="toHook">The thing to call events on.</param>
    public void HookTo(VisualElement toHook) {
        activeHook = toHook;
    }

    #region Interface crap

    public float pressure => 0;

    public float tangentialPressure => 0;

    public float altitudeAngle => 0;

    public float azimuthAngle => 0;

    public float twist => 0;

    public Vector2 tilt => Vector2.zero;

    public PenStatus penStatus => PenStatus.None;

    public Vector2 radius => Vector2.zero;

    public Vector2 radiusVariance => Vector2.zero;

    public EventModifiers modifiers => EventModifiers.None;

    public bool shiftKey => false;

    public bool ctrlKey => false;

    public bool commandKey => false;

    public bool altKey => false;

    public bool actionKey => false;
    #endregion
}
