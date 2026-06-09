# Sprite Atlas Guide

This document explains how to replace the procedural runtime-generated sprites with a proper packed sprite atlas.

## Current State

The game currently uses **procedural sprites** generated at runtime:
- `TargetRuntime.cs` generates shapes using `Texture2D` at startup
- Shapes: Circle, Square, Triangle, Star
- Colors defined in `RulesetDefinition` (Flappy-Bird palette)

## Why Upgrade to Sprite Atlas?

| Aspect | Procedural | Sprite Atlas |
|--------|-----------|--------------|
| Draw calls | 1 per target | 1 batch for all |
| Memory | Dynamic generation | Pre-packed |
| Visual quality | Basic shapes | Custom art |
| Build size | Code only | Asset size |
| Flexibility | Limited | Full art control |

## Step-by-Step Migration

### 1. Create Art Assets

Create 4 sprite files (PNG with transparency):

**File: `Assets/_Game/Sprites/target-circle.png`**
- Size: 128x128px
- Style: Flappy-Bird inspired (simple, cartoon, bold outline)
- Color: White (tinted at runtime)

**File: `Assets/_Game/Sprites/target-square.png`**
- Size: 128x128px
- Style: Rounded corners, bold outline

**File: `Assets/_Game/Sprites/target-triangle.png`**
- Size: 128x128px
- Style: Equilateral, bold outline

**File: `Assets/_Game/Sprites/target-star.png`**
- Size: 128x128px
- Style: 5-pointed, bold outline

### 2. Configure Sprite Import Settings

For each sprite:
1. Select in Project window
2. Inspector settings:
   - **Texture Type**: Sprite (2D and UI)
   - **Sprite Mode**: Single
   - **Pixels Per Unit**: 100
   - **Mesh Type**: Full Rect
   - **Wrap Mode**: Clamp
   - **Filter Mode**: Point (pixel art) or Bilinear (smooth)

### 3. Create Sprite Atlas

1. Right-click in Project window
2. **Create > 2D > Sprite Atlas**
3. Name: `TargetAtlas`
4. Add sprites to "Objects for Packing" list

**Atlas Settings:**
- Type: Master
- Include in Build: ✓
- Allow Rotation: ✓
- Tight Packing: ✓
- Padding: 4
- Read/Write: Disabled
- Generate Mip Maps: Disabled (2D game)

### 4. Modify TargetRuntime.cs

Replace procedural generation with atlas references:

```csharp
public class TargetRuntime : MonoBehaviour
{
    [SerializeField] private SpriteAtlas targetAtlas; // Add this
    
    // Replace GenerateProceduralSprite() with:
    private void LoadSpriteFromAtlas()
    {
        string spriteName = rule.shape.ToString().ToLower();
        Sprite sprite = targetAtlas.GetSprite($"target-{spriteName}");
        _spriteRenderer.sprite = sprite;
    }
}
```

### 5. Update CoreLoopBootstrap

Pass atlas reference during initialization:

```csharp
[SerializeField] private SpriteAtlas targetAtlas; // Drag in Inspector

private void Awake()
{
    // ...
    spawner.Configure(ruleset, Camera.main, _state, targetAtlas);
}
```

## Art Style Guidelines

Based on `DESIGN.md` (Flappy-Bird inspired):

**Palette:**
- Sky: `#70C5CE`
- Accent: `#F7E26B`
- Green: `#7ED957`
- Red: `#FF5A5F`
- Blue: `#4D96FF`
- Yellow: `#FFD93D`
- Purple: `#A66CFF`

**Style:**
- Simple cartoon shapes
- Bold black outlines (2-3px)
- Flat colors (no gradients)
- Slight drop shadow for depth
- Cute/friendly aesthetic

## Testing the Migration

1. Replace one shape at a time
2. Check all 5 gestures work with new art
3. Verify color tinting works correctly
4. Test on target device (Android)
5. Profile draw calls (should decrease)

## Rollback Plan

If issues arise:
1. Revert `TargetRuntime.cs` changes
2. Procedural generation still works as fallback
3. Or set `useProceduralSprites = true` flag

## Future Enhancements

- Animation frames per target type
- Particle effects for hits
- Glow/outline effects
- Variations per gesture type
