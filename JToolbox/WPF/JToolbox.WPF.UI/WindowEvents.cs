using JToolbox.WPF.Core.Awareness;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace JToolbox.WPF.UI
{
    public class WindowEvents
    {
        public WindowEvents(Window window)
        {
            Window = window;
        }

        public Window Window { get; }
        public bool WindowRendered { get; private set; }
        public bool WindowInitialized { get; private set; }
        private object DataContext => Window.DataContext;

        public void Attach()
        {
            Window.Initialized += Window_Initialized;
            Window.ContentRendered += Window_ContentRendered;
            Window.Closing += Window_Closing;
            Window.Closed += Window_Closed;
            Window.Loaded += Window_Loaded;
            Window.KeyDown += Window_KeyDown;
        }

        public void Detach()
        {
            Window.Initialized -= Window_Initialized;
            Window.ContentRendered -= Window_ContentRendered;
            Window.Closing -= Window_Closing;
            Window.Closed -= Window_Closed;
            Window.Loaded -= Window_Loaded;
            Window.KeyDown -= Window_KeyDown;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            (DataContext as IOnClosedAware)?.OnClosed();
        }

        private void Window_Initialized(object sender, EventArgs e)
        {
            if (!WindowInitialized)
            {
                if (DataContext is ICloseSource closeAware)
                {
                    closeAware.OnClose += Window.Close;
                }
                WindowInitialized = true;
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            if (!WindowRendered)
            {
                (DataContext as IOnShowAware)?.OnShow();
                WindowRendered = true;
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (DataContext is IOnClosingAware closingAware)
            {
                e.Cancel = closingAware.OnClosing();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is IOnLoadedAware loadedAware)
            {
                loadedAware.OnLoaded();
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (DataContext is IOnKeyDownAware keyDownAware)
            {
                keyDownAware.OnKeyDown(e);
            }
        }
    }
}