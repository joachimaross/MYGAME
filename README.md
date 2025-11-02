# Market Hustle — Unity Mobile (URP) Setup & Guide

This repository contains the starting scaffolding for "Market Hustle" — a mobile-first 3D life/business simulation (supermarket + housing/decoration).

Overview
- Folder structure to create inside Unity's `Assets/`.
- Base scripts for player movement, economy, real estate, furniture placement, and save system.
- Setup guide to configure URP, scenes, and build settings for iOS & Android.

Quick Unity Setup (copy/paste friendly)
1. Create a new Unity project (3D, URP template recommended).
2. Import this repository's `Assets/` into the Unity project's `Assets/` folder.
3. In Project Settings -> Graphics, ensure URP pipeline asset is assigned.
4. Scenes: create the following scenes and save them under `Assets/Scenes/`:
   - SupermarketScene.unity
   - ApartmentScene.unity
   - CondoScene.unity
   - VillaScene.unity
   - MansionScene.unity
5. Create root GameObjects in scenes for systems and attach manager scripts:
   - GameManager (attach `EconomyManager`, `RealEstateManager`, `SaveSystem`)
   - Player (attach `PlayerController`)
   - FurnitureManager (attach `FurnitureManager`)

Prefabs and UI
- Create a `Prefabs/` folder. Add furniture prefabs (bed, couch, lamp, table). Mark static where appropriate.
- Create UI canvases for Supermarket and Home. The scripts provide hooks and public methods to wire buttons.

Build Tips
- For Android: set minimum API to 21, IL2CPP for best performance on release builds, strip engine code.
- For iOS: enable ARM64, use Xcode export, and profile on device.
- Texture sizes: prefer 512x512 or 1k for mobile.

Notes
- This README is a bootstrap guide. See `Assets/Prefabs/README.md` for prefab expectations and `MARKET_HUSTLE_PROMPT_v2.txt` for the full design prompt.

---

# JACAMENO — Phase 1 Setup ("The Start of Jacameno")

This section is a copy of the Phase 1 step-by-step setup to build the Jacameno prototype: a third-person open-world starter with an enterable supermarket and a working money UI.

Phase 1 Goal
- Build a working third-person prototype with:
   - City environment
   - Player movement (third-person)
   - Basic money UI
   - Enterable supermarket (placeholder building)

Step 1 — Project Setup
1. Open Unity Hub → Create a new project.
2. Choose 3D (URP) template.
3. Name the project `Jacameno`.
4. Set platform to Android (File → Build Settings → Android → Switch Platform).

Step 2 — Import Essential Assets
Import the following packages via Package Manager / Asset Store:
- Cinemachine
- Starter Assets – Third Person Character Controller (by Unity)
- ProBuilder
- TextMeshPro
Optional:
- Low poly city or simple city packs, skybox assets.

Step 3 — Scene Layout
1. Create a new Scene → Name it `Jacamenoville`.
2. Add a Plane and scale to (1000,1,1000).
3. Add placeholder buildings (ProBuilder cubes) for Supermarket, Apartment block, Restaurant.
4. Add Directional Light (sun) and a Skybox.

Step 4 — Player Controller
1. From Starter Assets: drag `PlayerArmature` prefab into the scene.
2. Add a Cinemachine Virtual Camera and set its Follow to the Player.
3. Verify movement with joystick or WASD.

Step 5 — Basic Money System
Create a UI Canvas and a TextMeshPro - Text element for money.
Create `Assets/Scripts/Game/MoneySystem.cs` (added in this repo). Attach to an empty GameObject.
Assign the TMP text to the `moneyText` field in the inspector.

Step 6 — Enter / Exit Supermarket
1. Place a Box Collider at the supermarket entrance and check `Is Trigger`.
2. Create `Assets/Scripts/Game/StoreEntrance.cs` (added in this repo) and attach to the collider GameObject.
3. Ensure the player GameObject has the `Player` tag.
4. Create a scene named `SupermarketInterior` and add it to Build Settings.

Step 7 — Test Build
1. Press Play to test movement and the store entry transition.
2. Build and run on a device via File → Build Settings → Build and Run.

Next phases:
- Phase 2: Supermarket interior + shelving + item interactions
- Phase 3: Apartment interiors + furniture system
- Phase 4: Vehicles and dealerships
- Phase 5: Business dashboard and empire UI

