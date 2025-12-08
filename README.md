# ⚡ Blazor.GSAP

[![NuGet](https://img.shields.io/nuget/v/Blazor.GSAP.svg)](https://www.nuget.org/packages/Blazor.GSAP)[![NuGet downloads](https://img.shields.io/nuget/dt/Blazor.GSAP.svg)](https://www.nuget.org/packages/Blazor.GSAP)[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)[![.NET Support](https://img.shields.io/badge/.NET-6.0%20%7C%208.0%20%7C%209.0%20%7C%2010.0-512BD4?style=flat-square&logo=dotnet)](https://www.nuget.org/packages/Blazor.GSAP)

> **A lightweight, zero-boilerplate Blazor wrapper for GSAP (GreenSock Animation Platform).**

It simplifies JavaScript interoperability by providing a base class that handles **dynamic script loading**, **collocated file imports**, and **automatic resource cleanup** (disposing timelines) on component destruction.

Designed to be fully compatible with **Blazor Server**, **WebAssembly**, and **.NET MAUI Hybrid**.

---

## ✨ Features

| Feature                      | Description                                                                                                    |
| :--------------------------- | :------------------------------------------------------------------------------------------------------------- |
| **🚀 Zero Setup**            | No need to manually add `<script>` tags to `index.html`. It injects GSAP dynamically.                          |
| **✨ Boilerplate-Free**      | Just inherit from `GsapComponentBase` and start animating.                                                     |
| **📱 MAUI Hybrid Ready**     | Uses a robust script injection strategy that works perfectly in strict WebView environments.                   |
| **🧹 Auto-Cleanup**          | Automatically calls `killAll()` when the component is disposed to prevent memory leaks and "ghost" animations. |
| **📂 Collocated JS Support** | Automatically detects and loads `[Component].razor.js` files.                                                  |

---

## 📦 Installation

Install via NuGet Package Manager:

```bash
dotnet add package Blazor.GSAP
```

---

## 🚀 Quick Start

Get started in 3 simple steps.

### 1️⃣ Write your animation logic

Create a JavaScript file with the **same name** as your component in the same directory.

📄 **Pages/Home.razor.js**

```javascript
export function animateBox() {
  // ✅ Use gsap directly. The base class ensures it is loaded.
  gsap.to(".box", {
    rotation: 360,
    duration: 2,
    repeat: -1,
    ease: "none",
  });
}
```

### 2️⃣ Create your component

Inherit from `GsapComponentBase` in your Razor component (`.razor`).

📄 **Pages/Home.razor**

```razor
@page "/"
@using Blazor.GSAP
@inherits GsapComponentBase

<div class="box"></div>

@code {
    /// <summary>
    /// Lifecycle hook invoked after the GSAP core library and the component-specific JavaScript module have been successfully loaded and initialized.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <strong>Override this method to implement your GSAP animation logic.</strong>
    /// </para>
    /// <para>
    /// This method is intended to be used instead of <see cref="GsapComponentBase.OnAfterRenderAsync"/> for animation initialization.
    /// It guarantees that both the GSAP core and the current component's module (<see cref="GsapComponentBase.JSModule"/>) are available, preventing potential null reference errors during JS interop.
    /// </para>
    /// </remarks>
    protected override async Task OnGsapLoadedAsync() {
        await JSModule.InvokeVoidAsync("animateBox");
    }
}
```

### 3️⃣ That's it\!

- ✅ **No** `index.html` modification required.
- ✅ **No** manual `Dispose` logic required (animations stop automatically when you leave the page).

---

## ⚙️ How it works

### 💉 Dynamic Injection

The library uses a smart injection strategy. Instead of standard ES modules (which can cause issues in MAUI Hybrid WebViews due to strict mode), it dynamically injects a `<script>` tag. This ensures GSAP is registered globally on the `window` object, making it compatible with all standard GSAP plugins.

### ♻️ Automatic Disposal

`GsapComponentBase` implements `IAsyncDisposable`. When your Blazor component is disposed:

1.  🛑 It calls `gsap.globalTimeline.getChildren().forEach(t => t.kill())`.
2.  🗑️ It disposes of the collocated JS module.
3.  🛡️ It ensures no memory leaks occur in your SPA.

---

## 📜 License

**The C# source code in this package is licensed under the MIT License.**

> ⚠️ **Important License Notice**
>
> This is an **unofficial** wrapper. The underlying **GreenSock Animation Platform (GSAP)** is a proprietary library subject to the [Standard GreenSock License](https://greensock.com/licensing/).
>
> While GSAP is free for most use cases, it is **not MIT licensed**. Commercial usage in products sold to multiple customers (e.g., SaaS, templates) may require a "Business Green" license from GreenSock.
>
> _This package is not affiliated with or endorsed by GreenSock._
