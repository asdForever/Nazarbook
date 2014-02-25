using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace AlmaCloud.pages
{
    public partial class Title : PhoneApplicationPage
    {
        public Title()
        {
            InitializeComponent();

            var gl = GestureService.GetGestureListener(LayoutRoot);
            gl.Flick += new EventHandler<FlickGestureEventArgs>(GestureListener_Flick);


        }
        private void startAnimation()
        {
            DispatcherTimer timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(5)
            };

            timer.Start();

            timer.Tick += (s, u) =>
            {
                whiteGrid.Opacity -= 0.02;
                backgroundGrid.Opacity += 0.02;
                if (whiteGrid.Opacity <= 0)
                {
                    timer.Stop();
                    startAnimation1();
                }
            };
        }
        private void startAnimation1()
        {
            DispatcherTimer timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromMilliseconds(5)
            };

            timer.Start();

            timer.Tick += (s, u) =>
            {
                whiteGrid.Opacity += 0.02;
                if (whiteGrid.Opacity >= 1)
                {
                    timer.Stop();
                    whiteGrid.Visibility = Visibility.Collapsed;
                    logoGrid.Visibility = Visibility.Collapsed;
                    background1Grid.Visibility = Visibility.Visible;
                }
            };
        }
        private void GestureListener_Flick(object sender, FlickGestureEventArgs e)
        {
            if (e.Direction == System.Windows.Controls.Orientation.Horizontal)
            {
                if (e.HorizontalVelocity < 0) // determine direction (Right > 0)
                {
                    Animation(TurnstileTransitionMode.ForwardOut);
                    NavigationService.Navigate(new Uri("/pages/AnnotationPage.xaml", UriKind.Relative));
                }
            }
        }
        private void Animation(TurnstileTransitionMode animationMode)
        {
            TurnstileTransition turnstileTransition = new TurnstileTransition { Mode = animationMode };
            ITransition transition = turnstileTransition.GetTransition(LayoutRoot);
            transition.Completed += delegate
            {
                transition.Stop();
            };
            transition.Begin();
        }
        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            startAnimation();
            //try { while (NavigationService.RemoveBackEntry() != null) ; }
            //catch (NullReferenceException ex) {
            //    MessageBox.Show("NullReferenceException during Title.LayoutRoot_Loaded: " + ex.Message, "Error", MessageBoxButton.OK);
            //}
        }
    }
}