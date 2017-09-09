using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Store;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyVideo
{

    public enum NotifyType
    {
        StatusMessage,
        ErrorMessage
    };



    //class rootPage
    //{
    //    private FileActivatedEventArgs _fileEventArgs = null;
    //    public FileActivatedEventArgs FileEvent
    //    {
    //        get { return _fileEventArgs; }
    //        set { _fileEventArgs = value; }
    //    }

    //    private ProtocolActivatedEventArgs _protocolEventArgs = null;
    //    public ProtocolActivatedEventArgs ProtocolEvent
    //    {
    //        get { return _protocolEventArgs; }
    //        set { _protocolEventArgs = value; }
    //    }

    //    internal static void NotifyUser(string p, NotifyType notifyType)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}



    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage Current;
		

        private FileActivatedEventArgs _fileEventArgs = null;
        public FileActivatedEventArgs FileEvent
        {
            get { return _fileEventArgs; }
            set { _fileEventArgs = value; }
        }

        private ProtocolActivatedEventArgs _protocolEventArgs = null;
        public ProtocolActivatedEventArgs ProtocolEvent
        {
            get { return _protocolEventArgs; }
            set { _protocolEventArgs = value; }
        }

        public async void NavigateToFilePage()
        {
            StorageFile file = (StorageFile)_fileEventArgs.Files[0];
            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            VideoPlayer.SetSource(stream, file.ContentType);
            pick_video_file.Visibility = Visibility.Collapsed;

            VideoPlayer.Play();
        }

        public void NavigateToProtocolPage()
        {
            
        }
        DispatcherTimer ticks = new DispatcherTimer();
        bool isplaying = false;
        bool ispaused = false;
        string key = "yes";

        public MainPage()
        {
            this.InitializeComponent();
            
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey(key))
            {

            }
            else
            {
                ApplicationData.Current.RoamingSettings.Values[key] = "yes";
            }

            onFileStream();
            //ratingReminderPopup();

            //play_btn.Visibility = Visibility.Collapsed;

        }

        private async System.Threading.Tasks.Task onFileStream()
        {
            try
            {
                var stream = await myHelper.file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                VideoPlayer.SetSource(stream, myHelper.file.ContentType);
                VideoPlayer.Play();
            }
            catch (Exception)
            {


            }
        }

        private async System.Threading.Tasks.Task ratingReminderPopup()
        {
            var dlg = new MessageDialog("Do you want to rate this app??", "Rating Reminder");
            dlg.Commands.Add(new UICommand("OK", null, true));
            dlg.Commands.Add(new UICommand("Cancel", null, false));
            var result = false;

            try
            {
                result = (bool)(await dlg.ShowAsync()).Id;
            }
            catch (Exception)
            {
                //	this may happen if any other modal window is shown at the moment (ie, Windows query about running application background task)
            }
            if (result == true)
            {
                await Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=" + CurrentApp.AppId));
            }
        }

        //rootPage rootPage = null;
        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {

            
            //rootPage = e.Parameter as rootPage;

            //// Display the result of the file activation if we got here as a result of being activated for a file.
            //if (rootPage.FileEvent != null)
            //{
            //    //rootPage.NotifyUser("File activation received. The number of files received is " + rootPage.FileEvent.Files.Count + ". The first received file is " + rootPage.FileEvent.Files[0].Name + ".", NotifyType.StatusMessage);
            //}
        }

        private void Play_Button_Click(object sender, RoutedEventArgs e)
        {
            if (ispaused == false && isplaying == false)
            {
                //Initial_Display.Visibility = Visibility.Collapsed;
            }
            else if (ispaused == true && isplaying == false)
            {
                PlayView();
                VideoPlayer.Position = new TimeSpan(0, 0, 0, 0, (int)DurationSlider.Value);
                VideoPlayer.Play();
            }
        }

        private void SeekToMediaPosition(object sender, RangeBaseValueChangedEventArgs e)
        {
            int slidervalue = (int)DurationSlider.Value;
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, slidervalue);
            VideoPlayer.Position = ts;
        }

        //Initializing the Timer for Slider
        private void VideoPlayer_MediaOpened(object sender, RoutedEventArgs e)
        {
            DurationSlider.Maximum = VideoPlayer.NaturalDuration.TimeSpan.TotalMilliseconds;
            ticks.Interval = TimeSpan.FromMilliseconds(1);
            ticks.Tick += ticks_Tick;
            ticks.Start();
        }

        //Updating the Slider Value of Media(Video Duration)
        void ticks_Tick(object sender, object e)
        {
            DurationSlider.Value = VideoPlayer.Position.TotalMilliseconds;
            DurationText.Text = Milliseconds_to_Minute((long)VideoPlayer.Position.TotalMilliseconds);
        }

        //Pauses the Video
        private void Pause_Button_Click(object sender, RoutedEventArgs e)
        {
            if (VideoPlayer.CurrentState.Equals(MediaElementState.Playing))
            {
                VideoPlayer.Pause();
            }
            else {
                VideoPlayer.Play();
            }
        }

        private void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            SetLocalMedia();
        }

        //Updates the Content of Volume when Slider Changed
        private void VolumeSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {

        }

        public string Milliseconds_to_Minute(long milliseconds)
        {
            int minute = (int)(milliseconds / (1000 * 60));
            int seconds = (int)(milliseconds / 1000);
            int sec = seconds % 60;
            //if (seconds > 60)
            //{
            //    seconds %= 60;
            //}
            return (minute + " : " + sec);
        }

        //View When the Video is paused
        public void PauseView()
        {
            isplaying = false;
            ispaused = true;
            //play_btn.Visibility = Visibility.Visible;
            //Pause_Button.Visibility = Visibility.Collapsed;
            //Play_ImageButton.Visibility = Visibility.Collapsed;
            //Pause_ImageButton.Visibility = Visibility.Visible;
        }

        //View When the Media is Playing
        public void PlayView()
        {
            //Initial_Display.Visibility = Visibility.Collapsed;
            isplaying = true;
            ispaused = false;
            pick_video_file.Visibility = Visibility.Collapsed;
            //play_btn.Visibility = Visibility.Collapsed;
            //Pause_Button.Visibility = Visibility.Visible;
            //Play_ImageButton.Visibility = Visibility.Collapsed;
            //Pause_ImageButton.Visibility = Visibility.Collapsed;

        }

        //Plays the Selected Video from the Directory
        private async void SetLocalMedia()
        {
            try
            {
                FileOpenPicker openPicker = new FileOpenPicker();
                openPicker.ViewMode = PickerViewMode.Thumbnail;
                openPicker.FileTypeFilter.Add(".wmv");
                openPicker.FileTypeFilter.Add(".mp4");
                openPicker.FileTypeFilter.Add(".mp3");
                StorageFile file = await openPicker.PickSingleFileAsync();
                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                VideoPlayer.SetSource(stream, file.ContentType);
                VideoPlayer.Play();
                //BitmapImage bt = new BitmapImage();
                //bt.UriSource = new Uri("ms-appx:///Images/error.png");
                //DescriptionImage.Source = bt;
                //Description.Text = "A Video Playing From Local Directory";
                PlayView();
            }
            catch (Exception)
            { }
        }

        private void VideoPlayer_RightTapped_1(object sender, RightTappedRoutedEventArgs e)
        {

        }

        private void VideoPlayer_SeekCompleted_1(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Source = null;
            pick_video_file.Visibility = Visibility.Visible;
            //DurationSlider.Visibility = Visibility.Collapsed;
            //DurationText.Visibility = Visibility.Collapsed;
        }

        private void VideoPlayer_DragOver_1(object sender, DragEventArgs e)
        {

        }

        private void VideoPlayer_Tapped_1(object sender, TappedRoutedEventArgs e)
        {

        }

    }


}
