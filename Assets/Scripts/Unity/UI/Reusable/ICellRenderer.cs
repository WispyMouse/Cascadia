namespace Cascadia.Unity.UI.Reusable
{
    using Cascadia.DataStructures;
    using UnityEngine;

    public interface ICellRenderer
    {
        public Coordinate Coordinate { get; }

        /// <summary>
        /// Reference to a GameObject that can be instantiated.
        /// Interfaces can't be Instantiated directly, so this is a wrapper around the
        /// expectation that there's a prefab attached properly here.
        /// </summary>
        public GameObject SelfGameObject { get; }
        public void SetHighlightStatus(bool toHighlight);
    }
}
