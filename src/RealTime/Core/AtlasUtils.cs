namespace RealTime.Core
{
    using ColossalFramework.UI;
    using UnityEngine;

    public static class AtlasUtils
    {
        public static string[] LockUnLockSpriteNames =
        [
            "UnLock",
            "Lock"
        ];

        public static void CreateAtlas()
        {
            if (TextureUtils.GetAtlas("LockUnLockAtlas") == null)
            {
                TextureUtils.InitialiseAtlas("LockUnLockAtlas");
                for (int i = 0; i < LockUnLockSpriteNames.Length; i++)
                {
                    TextureUtils.AddSpriteToAtlas(new Rect(36 * i, 2, 32, 32), LockUnLockSpriteNames[i], "LockUnLockAtlas");
                }
            }
        }

        /// <summary>
        /// Returns a reference to the specified named atlas.
        /// </summary>
        /// <param name="atlasName">Atlas name.</param>
        /// <returns>Atlas reference (null if not found).</returns>
        public static UITextureAtlas GetTextureAtlas(string atlasName)
        {
            // No cache entry - get game atlases and iterate through, looking for a name match.
            var atlases = Resources.FindObjectsOfTypeAll(typeof(UITextureAtlas)) as UITextureAtlas[];
            for (int i = 0; i < atlases.Length; ++i)
            {
                if (atlases[i].name.Equals(atlasName))
                {
                    // Got it - add to cache and return.
                    return atlases[i];
                }
            }

            // If we got here, we couldn't find the specified atlas.
            return null;
        }

    }
}