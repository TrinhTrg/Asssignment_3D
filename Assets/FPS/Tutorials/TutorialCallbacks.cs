using UnityEngine;
using UnityEditor;

namespace Unity.Tutorials
{
    /// <summary>
    /// Implement your Tutorial callbacks here.
    /// </summary>
    public class TutorialCallbacks : ScriptableObject
    {
        public Unity.Tutorials.Core.Editor.FutureObjectReference futureRoomInstance = default;
        public Unity.Tutorials.Core.Editor.FutureObjectReference futureBotInstance = default;
        Unity.AI.Navigation.NavMeshSurface navMeshSurface = default;

        public bool NavMeshIsBuilt()
        {
            if (!navMeshSurface)
            {
                navMeshSurface = UnityEngine.Object.FindFirstObjectByType<Unity.AI.Navigation.NavMeshSurface>();
            }
            return navMeshSurface != null && navMeshSurface.navMeshData != null;
        }

        public void ClearAllNavMeshes()
        {
            if (!navMeshSurface)
            {
                navMeshSurface = UnityEngine.Object.FindFirstObjectByType<Unity.AI.Navigation.NavMeshSurface>();
            }
            UnityEditor.AI.NavMeshBuilder.ClearAllNavMeshes();
            if (navMeshSurface != null)
            {
                navMeshSurface.navMeshData = null;
            }
        }

        /// <summary>
        /// Keeps the Room selected during a tutorial.
        /// </summary>
        public void KeepRoomSelected()
        {
            SelectSpawnedGameObject(futureRoomInstance);
        }

        /// <summary>
        /// Keeps the Bot selected during a tutorial.
        /// </summary>
        public void KeepBotSelected()
        {
            SelectSpawnedGameObject(futureBotInstance);
        }

        /// <summary>
        /// Selects a GameObject in the scene, marking it as the active object for selection.
        /// </summary>
        public void SelectSpawnedGameObject(Unity.Tutorials.Core.Editor.FutureObjectReference futureObjectReference)
        {
            if (futureObjectReference == null || futureObjectReference.SceneObjectReference == null) { return; }
            Selection.activeObject = futureObjectReference.SceneObjectReference.ReferencedObjectAsGameObject;
        }

        public void SelectMoveTool()
        {
            Tools.current = Tool.Move;
        }

        public void SelectRotateTool()
        {
            Tools.current = Tool.Rotate;
        }
    }
}
