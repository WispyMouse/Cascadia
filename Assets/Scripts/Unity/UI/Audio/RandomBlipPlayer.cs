namespace Cascadia.Unity.UI
{
    /// <summary>
    /// Plays basic UI interaction blip sounds.
    /// </summary>
    public class RandomBlipPlayer : RandomAudioPlayer
    {
        /// <summary>
        /// Reference to this Singleton. Once included in a Scene, should never unload.
        /// Relies on placement of "Singletons.prefab" in some loaded scene.
        /// Set in <see cref="Awake"/>; do not refer to Singleton in other <see cref="Awake"/> scripts.
        /// </summary>
        public static RandomBlipPlayer Singleton { get; private set; } = null;

        protected override void Awake()
        {
            // If we already have an instance, don't make another
            if (Singleton != null)
            {
                Destroy(this.gameObject);
                return;
            }

            // This is now the singleton; unparent it so that we can mark as DontDestroyOnLoad
            this.transform.SetParent(null);
            Singleton = this;
            DontDestroyOnLoad(this.gameObject);

            base.Awake();
        }
    }
}
