using Microsoft.Phone.Controls;
using System;
using System.Windows;
using System.Windows.Navigation;

namespace AlmaCloud.pages
{
    public partial class AnnotationPage : PhoneApplicationPage
    {
        public AnnotationPage()
        {
            InitializeComponent();

            var gl = GestureService.GetGestureListener(LayoutRoot);
            gl.Flick += new EventHandler<FlickGestureEventArgs>(GestureListener_Flick);
        }
        private void GestureListener_Flick(object sender, FlickGestureEventArgs e)
        {
            if (e.Direction == System.Windows.Controls.Orientation.Horizontal)
            {
                if (e.HorizontalVelocity < 0) // determine direction (Right > 0)
                {
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                    Animation(TurnstileTransitionMode.ForwardOut);
                }
                else
                {
                    NavigationService.Navigate(new Uri("/pages/Title.xaml", UriKind.Relative));
                    Animation(TurnstileTransitionMode.BackwardIn);
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
            try { while (NavigationService.RemoveBackEntry() != null) ; }
            catch (NullReferenceException ex) {
                MessageBox.Show("NullReferenceException during AnnotationPage.LayoutRoot_Loaded: " + ex.Message, "Error", MessageBoxButton.OK);
            }
        }
    }
}