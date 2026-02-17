# Procedural Hex Grid Generator

A fully procedural hexagonal grid system built from scratch — no third-party libraries, no tutorials. Generates a complete hex grid as a **single mesh** using axial coordinates, custom vertex calculation, and procedural mesh generation.

![Hex Grid Demo](screenshots/hex.gif)
*Real-time hex grid generation with adjustable size, inner radius, and height*

## 🎯 What This Does

Given a radius (in hex circles), this system:
1. **Calculates hex positions** using axial coordinate math
2. **Generates vertices** for each hexagon (inner + outer radius)
3. **Builds triangles** for top face, bottom face, inner wall, and outer wall
4. **Outputs a single mesh** — the entire grid is one draw call

## 🔧 Core Systems

### 1. Axial Coordinate System

Hexagons are positioned using **axial coordinates (q, r)** — a standard approach for hex grids that makes neighbor lookups and world position calculations trivial.

**Grid Generation:**
```csharp
for (int q = -radius; q <= radius; q++)
    for (int r = -radius; r <= radius; r++)
        if (Mathf.Abs(q + r) <= radius)  // Diamond filter → Hex shape
            CreateHex(q, r);
```

The `Abs(q + r) <= radius` check is the key — it filters a square loop into a perfect hexagonal shape.

**Axial to World Position:**
```
x = √3 × q × size + √3/2 × r × size
z = 3/2 × r × size
```

This math comes from the geometry of hexagons — the √3 factor comes from the relationship between a hexagon's flat width and its point-to-point radius.

### 2. Vertex Generation

Each hexagon face has 6 edges. Each edge generates 4 vertices (inner top, outer top, inner bottom, outer bottom):

```csharp
float angle = 60 * edgeIndex - 30;  // Pointy-top orientation
float rad = angle * Mathf.PI / 180f;

Vector3 point = new Vector3(
    center.x + size * Mathf.Cos(rad),
    center.y + height,
    center.z + size * Mathf.Sin(rad)
);
```

The `-30` offset rotates the hexagon to pointy-top orientation.

### 3. Triangle Generation

Each edge generates 4 faces — top, bottom, outer wall, inner wall — with correct winding order for proper normal direction:

```
Per edge × 6 edges × N hexagons = complete mesh
```

The entire grid is stored in a single vertex list and triangle index list, then uploaded to the GPU as **one mesh**.

### 4. World ↔ Hex Coordinate Conversion

Converting world position back to hex coordinates uses **cube coordinate rounding**:

```csharp
// World → fractional hex
float r = worldPos.z / 1.5f;
float q = (worldPos.x - √3/2 × r) / √3;
float s = -q - r;  // Cube coordinate constraint: q + r + s = 0

// Round to nearest hex
roundQ, roundR, roundS = Round(q, r, s)

// Fix rounding errors (largest diff gets corrected)
if (diffQ > diffR && diffQ > diffS)
    roundQ = -roundR - roundS;
```

This is the standard cube rounding algorithm — because cube coordinates always sum to zero, we can fix rounding drift by correcting whichever axis drifted most.

### 5. Neighbor System

Each hex has 6 predefined neighbors in axial space:
```csharp
directions = { (1,0), (-1,0), (0,1), (0,-1), (1,-1), (-1,1) }

HexCoord neighbor = hex.GetNeighbor(index);
```

Simple addition in axial coordinates — no complex world-space math needed.

## 💡 What I Learned

**Hexagonal Mathematics**
- Axial coordinates make hex math elegant
- The √3 factor is fundamental to hex geometry
- Pointy-top vs flat-top orientation changes only the angle offset
- Cube coordinates (q, r, s where q+r+s=0) simplify many operations

**Procedural Mesh Generation**
- Vertex and triangle lists build the entire grid in one pass
- Triangle winding order determines face direction (normals)
- Single mesh = single draw call = much better performance
- Index offset math is critical when building multi-hex meshes

**Coordinate Systems**
- World ↔ grid conversions require careful math
- Cube rounding is the standard solution for hex snapping
- Grid algorithms are cleaner in axial/cube space than world space

## 📊 Performance: Single Mesh Approach

Building the entire grid as one mesh instead of one object per hexagon:

| Approach | Draw Calls | Objects | Scalability |
|----------|-----------|---------|-------------|
| One GameObject per hex | N | N | Poor |
| Single shared mesh | 1 | 1 | Excellent |

At 100+ hexagons, the single mesh approach is dramatically faster.

## 🛠️ Technical Stack

- **Unity 6000.0.2**
- **Pure C#** — no external libraries
- **Unity Mesh API** for procedural generation
- **Axial + Cube coordinate system**

## 📂 Code Structure

```
Assets/
├── HexGridManager.cs    # Grid generation, axial coordinates, world conversion
└── HexRenderer.cs       # Vertex/triangle generation, single mesh output
```

## 🔍 Challenges & Solutions

**Correct Triangle Winding**
- Problem: Faces rendering inside-out
- Solution: Carefully ordered vertex indices per face (clockwise = front-facing in Unity)

**Single Mesh Index Offset**
- Problem: Each hex needs correct vertex offset in shared list
- Solution: `hexCount × 24` offset per hexagon (24 vertices per hex)

**Grid Shape from Square Loop**
- Problem: Nested q/r loop generates a diamond, not a hex
- Solution: `Abs(q + r) <= radius` filter creates perfect hex shape

**Coordinate Rounding Drift**
- Problem: World → hex conversion has floating point errors
- Solution: Cube coordinate rounding — fix largest drift axis

## 🎯 Future Improvements

- [ ] Different hex orientations (flat-top)
- [ ] Per-hex height variation (terrain generation)
- [ ] UV mapping for texturing
- [ ] Chunk-based generation for infinite grids
- [ ] A* pathfinding using neighbor system

## ⚠️ Note

Built entirely independently — no tutorials or third-party libraries used. The math is based on hexagonal geometry fundamentals and standard axial coordinate theory.

---

**Developer**: Mert Özzencir  
**GitHub**: [MertOzzencir](https://github.com/MertOzzencir)  
**Unity Version**: 6000.0.2  
**Learning Focus**: Hexagonal mathematics, procedural mesh generation, coordinate systems