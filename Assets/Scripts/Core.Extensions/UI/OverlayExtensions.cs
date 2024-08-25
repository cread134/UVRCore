using UnityEditor.Overlays;

namespace Core.DevTools.UI
{
    public static class OverlayExtensions
    {
        public static void Redraw(this Overlay overlay)
        {
            //this fucking sucks, but its the only way to repaint :(
            if (overlay != null)
            {
                overlay.collapsed = true;
                overlay.collapsed = false;
            }
        }
    }
}
