Prefabs used by Market Hustle

This file lists the expected Prefabs and how to configure them. Unity prefab binary files are not included here; instead, create prefabs in the Editor and follow the names below so the scripts can reference them.

Required Prefabs (suggested names):
- Furniture_Bed (contains collider, optionally an Interaction script)
- Furniture_Couch
- Furniture_Table
- Furniture_Lamp
- Furniture_TV

Each furniture prefab should:
- Have a root GameObject with a proper pivot (for placement)
- Contain a collider (BoxCollider) for placement snapping
- Optionally include a small helper script/component to expose anchor points

Scene-specific prefabs:
- Supermarket registers: RegisterPrefab (for cashier interactions)
- ShelfPrefab: shelf that can hold Item prefabs

Prefab workflow:
1. Create GameObjects in scene.
2. Add required components (collider, rigidbody (if needed), visual mesh).
3. Drag to `Assets/Prefabs/` to create a prefab with matching name.
