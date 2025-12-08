// This file is responsible for two things:
// 1. Ensure the GSAP library is loaded (dynamically load it if not present).
// 2. Provide global cleanup functionality.

export function initGsap() {
    return new Promise((resolve, reject) => {
        // 1. If GSAP is already defined, return immediately
        if (typeof gsap !== 'undefined') {
            resolve();
            return;
        }

        // 2. Calculate the absolute path
        const baseUrl = import.meta.url;
        const libraryPath = new URL("./gsap.min.js", baseUrl).href;

        // 3. Create the <script> tag
        const script = document.createElement("script");
        script.src = libraryPath;
        script.async = true;

        // 4. Handle load completion
        script.onload = () => {
            resolve();
        };

        // 5. Handle errors
        script.onerror = (e) => {
            console.error("GSAP load failed:", e);
            reject(e);
        };

        // 6. Append to the document body
        document.body.appendChild(script);
    });
}

export function killAllGlobal() {
    if (typeof gsap !== 'undefined') {
        // Safe cleanup of the global timeline to prevent errors
        gsap.globalTimeline.getChildren().forEach(t => t.kill());
        // If using ScrollTrigger, consider adding: ScrollTrigger.getAll().forEach(t => t.kill());
    }
}