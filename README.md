# 🛒 AR Grocery Assistant

An augmented reality (AR) and generative AI-powered shopping assistant developed using Qualcomm Snapdragon Spaces and Unity. Designed to elevate in-store shopping, the system uses real-time object recognition and AI to provide detailed product information through a head-mounted display (HMD).

---

## 📌 Overview

The AR Grocery Assistant enhances how users interact with products while shopping by overlaying digital information—such as nutritional facts, pricing, and allergen data—on top of recognized grocery items. The system leverages on-device AR for seamless tracking and cloud-based AI for product-specific insights, all displayed in an intuitive user interface.

---

## 🧠 Core Features

- 📷 **Real-time object detection** via YOLOv8 and Snapdragon Spaces
- 🧠 **Generative AI integration** using Google Gemini to provide rich, context-aware item data
- 🛒 **Smart cart management** with interactive visual elements
- 🔐 **Privacy-conscious architecture**: no data stored or sold without consent
- 🔁 **Dual Render Fusion** for simultaneous display on both phone and AR headset
- 🧭 **UI designed for efficiency**: minimal clutter and easy navigation

---

## ⚙️ Tech Stack

- **Unity (C#)** – Development environment
- **Snapdragon Spaces SDK** – AR integration and spatial tracking
- **YOLOv8** – Real-time object detection
- **Google Gemini API** – Generative AI for contextual responses
- **ONNX / Barracuda** – Model integration and on-device inference
- **Git** – Version control

---

## 👥 Project Team

This project was developed as a capstone for the **Software Engineering program** at **California State University, San Marcos**.

### Developers
- **Aaron Edward Hamilton** – Software Engineer  
- **Elijah Esteban Munoz** – Software Engineer  
- **Kyle Anthony Beck** – Software Engineer  
- **Mason Thomas Vick** – Software Engineer  

### Faculty & Industry Involvement
- **Course Instructor**: Simon Fan  
- **Faculty Advisor**: Yongjie Zheng  
- **Industry Sponsor**: Qualcomm  
- **Project Mentor**: Karen Weeks  
- **Project Proposer**: Emma Lacey  

---

## 🏗️ Architecture Highlights

- **Model-View-Controller (MVC)** pattern with clean architecture principles
- **Design Patterns**:
  - Strategy Pattern for generating and updating item info
  - Composite Pattern for cart-item relationships
  - Singleton for API management
  - Builder Pattern for category-specific prompt generation
- **User Interface**:
  - Screen-level overlays (not world-locked) for accessibility
  - Visual slider to view item history and cart

---

## 🧭 Deployment Overview

To run the application:

1. Clone the repository and open it in Unity.
2. Import the Snapdragon Spaces SDK and required packages (YOLO, ONNX).
3. Add your Google Gemini API credentials.
4. Build the project for Android.
5. Install the APK on a Snapdragon Spaces-compatible Android device.
6. Connect the device to a Lenovo ThinkReality A3 HMD or equivalent.

---

## 🙌 Acknowledgments

We are deeply grateful to:

- **Qualcomm** for providing Snapdragon Spaces hardware and platform support  
- **Karen Weeks**, our mentor, for consistent guidance throughout development  
- **Dr. Yongjie Zheng**, for helping define project scope and system requirements  
- **Professor Simon Fan**, for teaching Agile project management fundamentals  
- **Emma Lacey**, for sponsoring and proposing the project vision  

---

## 🔗 References

- [Snapdragon Spaces Documentation](https://docs.spaces.qualcomm.com/unity/)
- [Unity Barracuda - Neural Network Inference](https://docs.unity3d.com/Packages/com.unity.barracuda)
- [YOLOv8 - Ultralytics](https://docs.ultralytics.com)
- [ONNX Model Integration](https://onnx.ai/)
- [Google Gemini API](https://developers.google.com/)

---

> CSU-SM-CSIS-30-2025-SE-001-Team-005 • February 2025
