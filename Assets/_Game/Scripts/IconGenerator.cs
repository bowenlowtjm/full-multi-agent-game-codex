using UnityEngine;

namespace Pully.Game
{
    /// <summary>
    /// Generates a simple app icon texture procedurally.
    /// Run in Editor to create the icon asset.
    /// </summary>
    public static class IconGenerator
    {
        public static Texture2D GenerateIcon(int size = 512)
        {
            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);

            // Flappy-Bird inspired colors
            Color skyColor = new Color(112f / 255f, 197f / 255f, 206f / 255f); // #70C5CE
            Color accentColor = new Color(247f / 255f, 226f / 255f, 107f / 255f); // #F7E26B
            Color outlineColor = new Color(40f / 255f, 40f / 255f, 40f / 255f);

            int center = size / 2;
            int radius = size / 3;

            for (int x = 0; x < size; x++)
            {
                for (int y = 0; y < size; y++)
                {
                    float dist = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));

                    if (dist < radius)
                    {
                        // Circle body (accent)
                        tex.SetPixel(x, y, accentColor);
                    }
                    else if (dist < radius + size / 32)
                    {
                        // Outline
                        tex.SetPixel(x, y, outlineColor);
                    }
                    else
                    {
                        // Background (sky)
                        tex.SetPixel(x, y, skyColor);
                    }
                }
            }

            // Add a simple "P" shape in the center
            int pSize = size / 4;
            for (int x = center - pSize / 2; x < center + pSize / 2; x++)
            {
                for (int y = center - pSize / 2; y < center + pSize / 2; y++)
                {
                    // Simple vertical bar for "P"
                    if (x > center - pSize / 3 && x < center - pSize / 6)
                    {
                        tex.SetPixel(x, y, outlineColor);
                    }
                    // Top curve of "P"
                    if (y > center + pSize / 4 && y < center + pSize / 3)
                    {
                        if (x >= center - pSize / 3 && x < center + pSize / 4)
                        {
                            tex.SetPixel(x, y, outlineColor);
                        }
                    }
                }
            }

            tex.Apply();
            return tex;
        }
    }
}
