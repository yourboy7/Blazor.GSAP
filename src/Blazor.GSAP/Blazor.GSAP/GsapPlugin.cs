namespace Blazor.GSAP;

public enum GsapPlugin {
    // === Core Functionality / Common Plugins ===
    Draggable,
    Flip,
    Observer,
    ScrollTo,
    ScrollTrigger,
    Text,

    // === Visual / Rendering Plugins ===
    DrawSVG,
    Easel,
    MorphSVG,
    Pixi,
    ScrambleText,
    SplitText,

    // === Motion / Physics Plugins ===
    Inertia,
    MotionPath,
    MotionPathHelper,
    Physics2D,
    PhysicsProps,

    // === Development Tools ===
    GSDevTools,

    // === Advanced Scroll (Requires ScrollTrigger) ===
    ScrollSmoother,

    // === Custom Easing (Requires CustomEase) ===
    CustomEase,
    CustomBounce,
    CustomWiggle,

    // === EasePack Bundle (Rough, ExpoScale, SlowMo) ===
    RoughEase,
    ExpoScaleEase,
    SlowMo
}