// This file is responsible for two things:
// 1. Ensure the GSAP library is loaded (dynamically load it if not present).
// 2. Provide global cleanup functionality.

// Helper function: Load script
function loadScript(src) {
    return new Promise((resolve, reject) => {
        // Check if it already exists
        if (document.querySelector(`script[src="${src}"]`)) {
            resolve();
            return;
        }
        const script = document.createElement("script");
        script.src = src;
        script.async = true;
        script.onload = () => resolve();
        script.onerror = (e) => reject(e);
        document.body.appendChild(script);
    });
}

// Initialization function: Receives the list of plugins
export async function initGsap(plugins) {
    const baseUrl = import.meta.url;
    // 1. Ensure the GSAP core is loaded.
    if (typeof gsap === 'undefined') {
        const corePath = new URL("./gsap.min.js", baseUrl).href;
        await loadScript(corePath);
    }

    // 2. Register plugins (if the user provides a list of plugin URLs)
    if (plugins && Array.isArray(plugins) && plugins.length > 0) {
        const loadingPromises = plugins.map(pluginUrl => {
            const pluginPath = new URL(`./${pluginUrl}`, baseUrl).href;
            return loadScript(pluginPath);
        });

        await Promise.all(loadingPromises);
    }
}

export function killAllGlobal() {
    if (typeof gsap !== 'undefined') {
        // Safe cleanup of the global timeline to prevent errors
        gsap.globalTimeline.getChildren().forEach(t => t.kill());
        // If using ScrollTrigger, consider adding: ScrollTrigger.getAll().forEach(t => t.kill());
    }
}