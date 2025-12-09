// Save instance state so that it can be shared between different functions.
let splitInstance;
let currentAnimation;

// Internal set function
function setup() {
    // If an instance already exists, restore it first to prevent duplicate splitting.
    if (splitInstance) splitInstance.revert();
    if (currentAnimation) currentAnimation.revert();

    // Create a new SplitText instance
    // Note: You can use either new SplitText or SplitText.create.
    splitInstance = new SplitText(".text", { type: "chars,words,lines" });
}

// 1. Initialization function (to be called by OnGsapLoadedAsync)
export function init() {
    gsap.registerPlugin(SplitText);
    setup();
    // Listen for window size changes to reset text splitting
    window.addEventListener("resize", setup);
}

// 2. Character animation (for C# calls)
export function animateChars() {
    if (currentAnimation) currentAnimation.revert();

    currentAnimation = gsap.from(splitInstance.chars, {
        x: 150,
        opacity: 0,
        duration: 0.7,
        ease: "power4",
        stagger: 0.04
    });
}

// 3. Word animation (for C# calls)
export function animateWords() {
    if (currentAnimation) currentAnimation.revert();

    currentAnimation = gsap.from(splitInstance.words, {
        y: -100,
        opacity: 0,
        rotation: "random(-80, 80)",
        duration: 0.7,
        ease: "back",
        stagger: 0.15
    });
}

// 4. Line animation (for C# calls)
export function animateLines() {
    if (currentAnimation) currentAnimation.revert();

    currentAnimation = gsap.from(splitInstance.lines, {
        rotationX: -100,
        transformOrigin: "50% 50% -160px",
        opacity: 0,
        duration: 0.8,
        ease: "power3",
        stagger: 0.25
    });
}