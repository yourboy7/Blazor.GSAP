export function animateBox() {
    // Use `gsap` directly, as the base class ensures it is loaded.
    gsap.to(".box", { rotation: +360, duration: 1, repeat: -1, ease: "none" });
}