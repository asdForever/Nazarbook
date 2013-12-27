using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using AlmaCloud.Classes;
using Facebook.Client;
using System.Threading.Tasks;
using Microsoft.Phone.Tasks;
using System.Security.Cryptography;
using AsyncOAuth;
using System.Net.Http;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Text.RegularExpressions;
using TweetSharp;
using System.Linq;

namespace AlmaCloud
{

    public partial class MainPage : PhoneApplicationPage
    {
        TextWorker tw;
        private OAuthRequestToken requestToken;
        private TwitterService service;
        Dispatcher dispatchme = Deployment.Current.Dispatcher;
        private GestureListener gl;

        // Constructor
        public MainPage()
        {
            InitializeComponent();
            ConstructUI();
        }

        public void ConstructUI()
        {
            tw = new TextWorker();
            workWithJson();

            Book.currentAphorism = 0;
            Book.currentChapter = 1;

            Book.chapterList = new List<Chapter>
            {
                new Chapter(1, "ОТАНДЫ СҮЮ - СУЫҒЫНА ШЫДАП, ЫСТЫҒЫНА КҮЮ", "ЕДИНАЯ ОТЧИЗНА - НЕЗАВИСИМЫЙ КАЗАХСТАН", "LOVE FOR YOUR COUNTRY MEANS BEARING TO ITS HARSH COLD AND SULTRY HEAT"),
                new Chapter(2, "ЖЕР ТАҒДЫРЫ - ЕЛ ТАҒДЫРЫ", "СУДЬБА ЗЕМЛИ – СУДЬБА СТРАНЫ", "DESTINY OF THE LAND IS THE DESTINY OF PEOPLE"),
                new Chapter(3, "ӨТКЕНГЕ ҚАРАП, ЕРТЕҢІҢДІ ТҮЗЕ!", "СЛЕДУЯ ТРАДИЦИЯМ, УСТРЕМЛЯТЬСЯ В БУДУЩЕЕ", "HEAD TOWARDS THE FUTURE WHILE FOLLOWING THE TRADITIONS"),
                new Chapter(4, "МЕМЛЕКЕТ МЕРЕЙІ – МЫҒЫМДЫҒЫНДА", "ДОСТОИНСТВО ГОСУДАРСТВА – В ЕГО ПРОЧНОСТИ", "THE COUNTRY’S DIGNITY IS IN ITS STRENGTH")
            };

            gl = GestureService.GetGestureListener(LayoutRoot);
            gl.Flick += new EventHandler<FlickGestureEventArgs>(GestureListener_Flick);

            UpdateUI();
        }

        private string getNewImageSourcePath(System.Windows.Controls.Image sender)
        {
            if (sender.Name.Contains("Active"))
                sender.Name = sender.Name.Replace("Active", "");
            else
                sender.Name = sender.Name + "Active";
            return "/Images/" + sender.Name + ".png";
        }
        private void setImageSource(System.Windows.Controls.Image sender, string imageUrl)
        {
            System.Windows.Media.Imaging.BitmapImage bi = new System.Windows.Media.Imaging.BitmapImage();
            bi.CreateOptions = System.Windows.Media.Imaging.BitmapCreateOptions.BackgroundCreation;
            bi.UriSource = new Uri(imageUrl, UriKind.Relative);
            sender.Source = bi;
        }

        private void GestureListener_Flick(object sender, FlickGestureEventArgs e)
        {
            if (e.Direction == System.Windows.Controls.Orientation.Horizontal)
            {
                if (e.HorizontalVelocity < 0) // determine direction (Right > 0)
                {
                    if (Book.currentAphorism == Book.aphorismList.Count - 1)
                    {
                        Book.currentAphorism = 0;
                        Book.currentChapter = 0;
                    }
                    else
                    {
                        Book.currentAphorism++;
                        Book.currentChapter = getAphorismChapterIndexByID(Book.currentAphorism);
                    }
                    Animation(TurnstileTransitionMode.ForwardOut);
                }
                else
                {
                    if (Book.currentAphorism == 0)
                        NavigationService.Navigate(new Uri("/pages/AnnotationPage.xaml", UriKind.Relative));
                    else
                    {
                        Book.currentAphorism--;
                        Book.currentChapter = getAphorismChapterIndexByID(Book.currentAphorism);
                    }
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
                UpdateUI();
                transition.Stop();
            };
            transition.Begin();
        }
        private void UpdateUI()
        {
            textBlockQuote.Text = Book.aphorismList[Book.currentAphorism].textList[Book.langID];
            textBlockAphorismID.Text = tw.getPageNumberByAphorismID(Book.currentAphorism);
            textBlockQuote.FontSize = tw.getFontSizeByTextLength(textBlockQuote.Text);
            textBlockChapterName.Text = Book.chapterList[Book.currentChapter - 1].titleList[Book.langID];

            addContentToListBox();

            if (Book.aphorismList[Book.currentAphorism].isLiked)
                setImageSource(imgLike, "/Images/buttonLikeActive.png");
            else
                setImageSource(imgLike, "/Images/buttonLike.png");

            if (Book.aphorismList[Book.currentAphorism].isSharedT)
                setImageSource(imgTwitter, "/Images/buttonTwitterActive.png");
            else
                setImageSource(imgTwitter, "/Images/buttonTwitter.png");

            if (Book.aphorismList[Book.currentAphorism].isSharedF)
                setImageSource(imgFacebook, "/Images/buttonFacebookActive.png");
            else
                setImageSource(imgFacebook, "/Images/buttonFacebook.png");
        }
        private void textBlockLanguage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            if (tb != null && menu.Visibility != Visibility.Visible)
            {
                int langID = 0;
                if (tb.Text == GlobalVariables.languageArray[0])
                {
                    langID = 1;
                }
                else if (tb.Text == GlobalVariables.languageArray[1])
                {
                    langID = 2;
                }
                else if (tb.Text == GlobalVariables.languageArray[2])
                {
                    langID = 0;
                }
                Book.langID = langID;
                tb.Text = GlobalVariables.languageArray[langID];
                UpdateUI();
            }
        }
        private void workWithJson()
        {
            JsonParse jp = new JsonParse();
            if (Book.aphorismList.Count == 0)
            {
                jp.getDataFromJSON();
            }
            else
            {
                jp.saveToLocalStorage();
            }
        }
        #region share buttons
        private void ShareButtons_GeneralHandler(object sender, System.Windows.Input.GestureEventArgs e)
        {
            System.Windows.Controls.Grid imgGrid = (System.Windows.Controls.Grid)sender;
            if (imgGrid != null && menu.Visibility != Visibility.Visible)
            {
                switch (imgGrid.Name)
                {
                    case "buttonLike":
                        buttonLikeHandler();
                        break;
                    case "buttonTwitter":
                        buttonTwitterHandler();
                        break;
                    case "buttonFacebook":
                        buttonFacebookHandler();
                        break;
                }
            }
        }
        #region like button
        private void buttonLikeHandler()
        {
            if (Book.aphorismList[Book.currentAphorism].isLiked == true)
            {
                Book.aphorismList[Book.currentAphorism].isLiked = false;
            }
            else
            {
                Book.aphorismList[Book.currentAphorism].isLiked = true;
            }
            workWithJson();
            UpdateUI();
        }
        #endregion
        #region twitter button
        private void buttonTwitterHandler()
        {
            if (Book.aphorismList[Book.currentAphorism].isSharedT != true)
            {
                AuthorizeTwitter();
            }
        }
        public void AuthorizeTwitter()
        {
            gl.Flick -= new EventHandler<FlickGestureEventArgs>(GestureListener_Flick);
            if (String.IsNullOrWhiteSpace(GlobalVariables.twitterAccessToken))
            {
                pinGrid.Visibility = Visibility.Visible;
                BrowserControl.Visibility = Visibility.Visible;

                service = new TwitterService(GlobalVariables.twitterConsumerKey, GlobalVariables.twitterConsumerKeySecret);
                var cb = new Action<OAuthRequestToken, TwitterResponse>(CallBackToken);
                service.GetRequestToken("oob", CallBackToken);
            }
            else
            {
                Tweet();
            }
        }
        void pinButton_Click(object sender, RoutedEventArgs e)
        {
            if (String.IsNullOrEmpty(pinText.Text))
                MessageBox.Show("Please enter PIN");
            else
            {
                try
                {
                    var cb = new Action<OAuthAccessToken, TwitterResponse>(CallBackVerifiedResponse);
                    service.GetAccessToken(requestToken, pinText.Text, CallBackVerifiedResponse);
                }
                catch
                {
                    MessageBox.Show("Something is wrong with the PIN. Try again please.", "Error", MessageBoxButton.OK);
                    gl.Flick += new EventHandler<FlickGestureEventArgs>(GestureListener_Flick);
                }
            }
        }
        private void pinCancelButton_Click(object sender, RoutedEventArgs e)
        {
            pinGrid.Visibility = Visibility.Collapsed;
            BrowserControl.Visibility = Visibility.Collapsed;
            gl.Flick += new EventHandler<FlickGestureEventArgs>(GestureListener_Flick);
        }
        void CallBackToken(OAuthRequestToken rt, TwitterResponse response)
        {
            Uri uri = service.GetAuthorizationUri(rt);
            requestToken = rt;
            BrowserControl.Dispatcher.BeginInvoke(() => BrowserControl.Navigate(uri));
        }
        void CallBackVerifiedResponse(OAuthAccessToken at, TwitterResponse response)
        {
            if (at != null)
            {
                GlobalVariables.twitterAccessToken = at.Token;
                GlobalVariables.twitterAccessTokenSecret = at.TokenSecret;
                Tweet();
            }
            else
            {
                gl.Flick += new EventHandler<FlickGestureEventArgs>(GestureListener_Flick);
            }
        }
        private async void Tweet()
        {
            OAuthUtility.ComputeHash = (key, buffer) => { using (var hmac = new HMACSHA1(key)) { return hmac.ComputeHash(buffer); } };

            var client = OAuthUtility.CreateOAuthClient(GlobalVariables.twitterConsumerKey, GlobalVariables.twitterConsumerKeySecret, new AccessToken(GlobalVariables.twitterAccessToken, GlobalVariables.twitterAccessTokenSecret));
            // Post
            var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("status", Book.aphorismList[Book.currentAphorism].textList[Book.langID]) });
            var response = await client.PostAsync("http://api.twitter.com/1.1/statuses/update.json", content);
            var json = await response.Content.ReadAsStringAsync();

            Dispatcher.BeginInvoke(() =>
            {
                pinGrid.Visibility = Visibility.Collapsed;
                BrowserControl.Visibility = Visibility.Collapsed;

                Book.aphorismList[Book.currentAphorism].isSharedT = true;
                UpdateUI();
                gl.Flick += new EventHandler<FlickGestureEventArgs>(GestureListener_Flick);
            });
        }
        #endregion
        #region facebook button
        private void buttonFacebookHandler()
        {
            gl.Flick -= new EventHandler<FlickGestureEventArgs>(GestureListener_Flick);
            if (Book.aphorismList[Book.currentAphorism].isSharedF != true)
            {
                if (!App.isAuthenticated)
                {
                    Authenticate();
                }
                else
                {
                    PublishStory();
                }
            }
        }
        private FacebookSession session;
        private async void Authenticate()
        {
            string message = String.Empty;
            try
            {
                session = await App.FacebookSessionClient.LoginAsync("user_about_me,publish_actions");
                App.AccessToken = session.AccessToken;
                App.FacebookId = session.FacebookId;

                Dispatcher.BeginInvoke(() => PublishStory());
            }
            catch (InvalidOperationException e)
            {
                message = "Login failed! Exception details: " + e.Message;
                MessageBox.Show(message); 
                gl.Flick += new EventHandler<FlickGestureEventArgs>(GestureListener_Flick);
            }
        }
        private async void PublishStory()
        {
            var facebookClient = new Facebook.FacebookClient(App.AccessToken);

            var postParams = new
            {
                name = "AlmaCloud",
                caption = "Nazarbayev's aphorisms",
                description = "\"" + Book.aphorismList[Book.currentAphorism].textList[Book.langID] + "\". Н.А. Назарбаев",
                link = "http://facebooksdk.net/",
                picture = "http://s27.postimg.org/ywkzynx0j/Application_Icon.png"
            };

            try
            {
                facebookClient.PostAsync("/me/feed", postParams);
                Book.aphorismList[Book.currentAphorism].isSharedF = true;
                App.isAuthenticated = true;
                gl.Flick += new EventHandler<FlickGestureEventArgs>(GestureListener_Flick);
                UpdateUI();
            }
            catch (MethodAccessException ex)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("MethodAccessException during post: " + ex.Message, "Error", MessageBoxButton.OK);
                    gl.Flick += new EventHandler<FlickGestureEventArgs>(GestureListener_Flick);
                    session = new FacebookSession();
                });
            }
            catch (Exception ex)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    MessageBox.Show("Exception during post: " + ex.Message, "Error", MessageBoxButton.OK);
                    gl.Flick += new EventHandler<FlickGestureEventArgs>(GestureListener_Flick);
                    session = new FacebookSession();
                });
            }
        }
        #endregion
        #endregion

        private void LayoutRoot_Loaded(object sender, RoutedEventArgs e)
        {
            try { while (NavigationService.RemoveBackEntry() != null) ; }
            catch (NullReferenceException ex) {
                MessageBox.Show("NullReferenceException during MainPage.LayoutRoot_Loaded: " + ex.Message, "Error", MessageBoxButton.OK);
            }
        }

        private void MenuImage_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (menu.Visibility == Visibility.Visible)
            {
                menu.Visibility = Visibility.Collapsed;
                gl.Flick += new EventHandler<FlickGestureEventArgs>(GestureListener_Flick);
            }
            else
            {
                imageFavButton.Visibility = Visibility.Collapsed;
                listBoxFavorites.Visibility = Visibility.Collapsed;
                imageConButton.Visibility = Visibility.Visible;
                listBoxContent.Visibility = Visibility.Visible;
                textFavButton.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
                textConButton.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);

                menu.Visibility = Visibility.Visible;
                gl.Flick -= new EventHandler<FlickGestureEventArgs>(GestureListener_Flick);
            }
        }
        private List<Aphorism> getFavoritesList()
        {
            List<Aphorism> aphorismList = new List<Aphorism>();
            foreach (Aphorism aphorism in Book.aphorismList)
            {
                if (aphorism.isLiked)
                    aphorismList.Add(aphorism);
            }
            return aphorismList;
        }
        private void addFavoritesToListBox(List<Aphorism> aphorismList)
        {
            listBoxFavorites.Items.Clear();
            foreach (Aphorism aphorism in aphorismList)
            {
                TextBlock tb = new TextBlock();
                tb.Text = aphorism.textList[Book.langID];
                tb.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
                tb.Width = 400;
                tb.TextWrapping = TextWrapping.Wrap;
                tb.Margin = new Thickness(0, 10, 0, 10);
                tb.Tag = getAphorismIndex(aphorism).ToString();
                tb.Tap += FavoritesAphorism_Tap;
                listBoxFavorites.Items.Add(tb);
            }
        }
        private void addContentToListBox()
        {
            listBoxContent.Items.Clear();
            foreach (Chapter chapter in Book.chapterList)
            {
                Grid grid = new Grid();
                grid.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                grid.Width = 400;
                grid.Height = 50;

                TextBlock tb = new TextBlock();
                tb.Text = chapter.chapterID.ToString() + " " + chapter.titleList[Book.langID];
                tb.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
                tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                tb.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                tb.TextAlignment = TextAlignment.Left;
                tb.FontSize = 20;
                tb.MaxWidth = 400;
                tb.MaxHeight = 50;
                tb.TextWrapping = TextWrapping.Wrap;
                tb.FontWeight = FontWeights.Bold;
                tb.Tag = chapter.chapterID.ToString();
                tb.Tap += ContentTextBlock_Tap;

                grid.Children.Add(tb);

                listBoxContent.Items.Add(grid);
            }
        }

        private void ContentGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            imageFavButton.Visibility = Visibility.Collapsed;
            listBoxFavorites.Visibility = Visibility.Collapsed;
            imageConButton.Visibility = Visibility.Visible;
            listBoxContent.Visibility = Visibility.Visible;
            textFavButton.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
            textConButton.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
        }
        private void FavoritesGrid_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            imageConButton.Visibility = Visibility.Collapsed;
            listBoxContent.Visibility = Visibility.Collapsed;
            imageFavButton.Visibility = Visibility.Visible;
            listBoxFavorites.Visibility = Visibility.Visible;
            textConButton.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.White);
            textFavButton.Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);

            addFavoritesToListBox(getFavoritesList());
        }

        private void FavoritesAphorism_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            if (tb != null)
            {
                Book.currentAphorism = Convert.ToInt32(tb.Tag);
                Book.currentChapter = Book.aphorismList[Convert.ToInt32(tb.Tag)].chapterID;
                menu.Visibility = Visibility.Collapsed;
                gl.Flick += new EventHandler<FlickGestureEventArgs>(GestureListener_Flick);
                UpdateUI();
            }
        }
        private void ContentTextBlock_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            TextBlock tb = (TextBlock)sender;
            if (tb != null)
            {
                Book.currentAphorism = getAphorismIndexByChapterID(Convert.ToInt32(tb.Tag));
                Book.currentChapter = Convert.ToInt32(tb.Tag);
                menu.Visibility = Visibility.Collapsed;
                gl.Flick += new EventHandler<FlickGestureEventArgs>(GestureListener_Flick);
                UpdateUI();
            }
        }
        private int getAphorismIndex(Aphorism aphorism)
        {
            for (int i = 0; i < Book.aphorismList.Count; i++)
            {
                if (Book.aphorismList[i].aphID == aphorism.aphID && Book.aphorismList[i].chapterID == aphorism.chapterID)
                {
                    return i;
                }
            }
            return 0;
        }
        private int getAphorismIndexByChapterID(int id)
        {
            for (int i = 0; i < Book.aphorismList.Count; i++)
            {
                if (Book.aphorismList[i].chapterID == id)
                {
                    return i;
                }
            }
            return 0;
        }
        private int getAphorismChapterIndexByID(int id)
        {
            return Book.aphorismList[id].chapterID;
        }
    }
}