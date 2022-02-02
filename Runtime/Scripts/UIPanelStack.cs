using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace Bodardr.UI.Runtime
{
    public static class UIPanelStack
    {
        private static LinkedList<UIPanel> panelStack;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize() => panelStack = new LinkedList<UIPanel>();

        public static void Push(UIPanel panel)
        {
            if (panelStack.Count > 0)
            {
                var last = panelStack.Last.Value;

                if (last.HideOnNewPanelPushed || panel.HideLastPanel)
                    last.HideInternal(true);
            }

            panelStack.AddLast(panel);
            panel.ShowInternal();
        }

        public static void Pop()
        {
            if (panelStack.Count > 0)
            {
                if (panelStack.Last.Value.IsPersistent)
                    return;

                panelStack.Last.Value.HideInternal();
                panelStack.RemoveLast();
            }

            if (panelStack.Count > 0)
                panelStack.Last.Value.ShowInternal();
        }

        public static bool Remove(UIPanel uiPanel, bool fromOnDestroy = false)
        {
            if (panelStack.Last != null && panelStack.Last.Value == uiPanel && !fromOnDestroy)
            {
                Pop();
                return true;
            }

            var contains = Contains(uiPanel);

            if (!contains)
                return false;

            panelStack.Remove(uiPanel);

            if (!fromOnDestroy)
                uiPanel.HideInternal();

            return true;
        }

        public static bool Contains(UIPanel uiPanel)
        {
            return panelStack.Contains(uiPanel);
        }
    }
}