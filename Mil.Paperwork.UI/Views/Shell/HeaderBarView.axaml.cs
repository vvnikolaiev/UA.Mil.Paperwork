using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Mil.Paperwork.UI.ViewModels;
using System.Runtime.InteropServices;

namespace Mil.Paperwork.UI.Views.Shell
{
    public partial class HeaderBarView : UserControl
    {
        private const double MacOsTrafficLightsInset = 70;
        private const double WindowsCaptionButtonsReserve = 176;
        private const double CaptionButtonsPadding = 8;
        private const int MaximizeClickCount = 2;

        private Window? _hostWindow;

        public HeaderBarView()
        {
            InitializeComponent();

            ApplyPlatformInsets();

            HeaderRoot.PointerPressed += OnHeaderPointerPressed;
            GlobalSearchBox.KeyDown += OnSearchBoxKeyDown;
        }

        private void OnSearchBoxKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
            {
                return;
            }

            var vm = DataContext as MainWindowViewModel;
            if (vm == null)
            {
                return;
            }

            vm.ExecuteGlobalSearch(GlobalSearchBox.Text ?? string.Empty);
            e.Handled = true;
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            _hostWindow = TopLevel.GetTopLevel(this) as Window;
            if (_hostWindow != null)
            {
                _hostWindow.PropertyChanged += OnWindowPropertyChanged;
                UpdateCaptionSpacers(_hostWindow.WindowDecorationMargin);
            }
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            if (_hostWindow != null)
            {
                _hostWindow.PropertyChanged -= OnWindowPropertyChanged;
                _hostWindow = null;
            }
        }

        private void OnWindowPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
        {
            if (e.Property == Window.WindowDecorationMarginProperty && sender is Window window)
            {
                UpdateCaptionSpacers(window.WindowDecorationMargin);
            }
        }

        // Reserve exactly the area occupied by the framework-drawn caption buttons.
        private void UpdateCaptionSpacers(Thickness margin)
        {
            if (margin.Right > 0)
            {
                CaptionButtonsSpacer.Width = margin.Right + CaptionButtonsPadding;
            }

            if (margin.Left > 0)
            {
                MacInsetSpacer.Width = margin.Left + CaptionButtonsPadding;
            }
        }

        private void ApplyPlatformInsets()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                MacInsetSpacer.Width = MacOsTrafficLightsInset;
                CaptionButtonsSpacer.Width = 0;
            }
            else
            {
                MacInsetSpacer.Width = 0;
                CaptionButtonsSpacer.Width = WindowsCaptionButtonsReserve;
            }
        }

        private void OnHeaderPointerPressed(object? sender, PointerPressedEventArgs e)
        {
            var window = TopLevel.GetTopLevel(this) as Window;
            if (window == null)
            {
                return;
            }

            if (!e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
            {
                return;
            }

            if (e.ClickCount == MaximizeClickCount)
            {
                window.WindowState = window.WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;
            }
            else
            {
                window.BeginMoveDrag(e);
            }
        }
    }
}
