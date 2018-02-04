using HeroExplorer.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HeroExplorer
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public ObservableCollection<Character> MarvelCharacters { get; set; }
        public ObservableCollection<Comic> MarvelComics { get; set; }

        public MainPage()
        {
            this.InitializeComponent();
            MarvelCharacters = new ObservableCollection<Character>();
            MarvelComics = new ObservableCollection<Comic>();

        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ProgressRingOn();
            int counter = 0;
            while (MarvelCharacters.Count < 10 && counter < 10)
            {
                counter++;
                AttributionTextBlock.Text = await MarvelFacade.PopulateMarvelCharactersAsync(MarvelCharacters);
            }
            //AttributionTextBlock.Text += counter.ToString();
            ProgressRingOff();

        }

        private void ProgressRingOn()
        {
            MyProgressRing.IsActive = true;
            MyProgressRing.Visibility = Visibility.Visible;
        }
        private void ProgressRingOff()
        {
            MyProgressRing.IsActive = false;
            MyProgressRing.Visibility = Visibility.Collapsed;
        }

        private async void MasterListView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ProgressRingOn();

            var selectedCharacter = (Character)e.ClickedItem;

            DetailNameTextBlock.Text = selectedCharacter.name;
            DetailDescriptionTextBlock.Text = selectedCharacter.description;

            var largeImage = new BitmapImage();
            Uri uri = new Uri(selectedCharacter.thumbnail.large, UriKind.Absolute);
            largeImage.UriSource = uri;

            DetailImage.Source = largeImage;

            MarvelComics.Clear();
            ComicsDetailsClear();

            await MarvelFacade.PopulateMarvelComicsAsync(selectedCharacter.id, MarvelComics);

            ProgressRingOff();

        }
        private void ComicsDetailsClear()
        {
            ComicDetailDescriptionTextBlock.Text = "";
            ComicDetailNameTextBlock.Text = "";
            ComicDetailImage.Source = null;
        }
        private void ComicsGridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            ProgressRingOn();

            var selectedComic = (Comic)e.ClickedItem;

            ComicDetailNameTextBlock.Text = (string.IsNullOrEmpty(selectedComic.title)) ? "" : selectedComic.title;

            ComicDetailDescriptionTextBlock.Text = (string.IsNullOrEmpty(selectedComic.description)) ? "" : selectedComic.description;

            var largeImage = new BitmapImage();
            Uri uri = new Uri(selectedComic.thumbnail.large, UriKind.Absolute);
            largeImage.UriSource = uri;

            ComicDetailImage.Source = largeImage;



            ProgressRingOff();
        }
    }
}
