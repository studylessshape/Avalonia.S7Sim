using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using System;

namespace Avalonia.S7Sim.UserControls
{
    [TemplatePart(PART_CloseButton, typeof(Button))]
    [TemplatePart(PART_RestoreButton, typeof(Button))]
    [TemplatePart(PART_MinimizeButton, typeof(Button))]
    [TemplatePart(PART_FullScreenButton, typeof(Button))]
    [PseudoClasses(":minimized", ":normal", ":maximized", ":fullscreen")]
    public class OverrideCaptionButtons : Ursa.Controls.CaptionButtons
    {
        private const string PART_CloseButton = "PART_CloseButton";
        private const string PART_RestoreButton = "PART_RestoreButton";
        private const string PART_MinimizeButton = "PART_MinimizeButton";
        private const string PART_FullScreenButton = "PART_FullScreenButton";

        private Action<WindowCloseReason, bool, bool>? CloseCore;

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);

            var method = HostWindow?.GetType()?.GetMethod("CloseCore", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (method != null)
            {
                CloseCore = (reason, isProgrammatic, ignoreCancel) =>
                {
                    method.Invoke(HostWindow, [reason, isProgrammatic, ignoreCancel]);
                };
            }
            else
            {
                CloseCore = (reason, isProgrammatic, ignoreCancel) =>
                {
                    base.OnClose();
                };
            }
        }

        protected override void OnClose()
        {
            CloseCore?.Invoke(WindowCloseReason.WindowClosing, false, false);
        }
    }
}
