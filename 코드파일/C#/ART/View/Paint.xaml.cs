using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ART.Model;

namespace ART.View
{
    public partial class Paint : Page
    {
        public Paint()
        {
            InitializeComponent();
            Init_ArtistList();
        }

        private void Init_ArtistList()
        {
            List<ArtistInfo> artistList = new List<ArtistInfo>();
            artistList.Clear();

            Main.data.Key = (int)KEY.ARTIST;
            int result = Main.CppSV.ReadArtistInfo();

            Task.Run(async () =>
            {
                if (result == (int)KEY.OK)
                {
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        foreach (var item in Main.data.ArtistList)
                        {
                            ArtistInfo artistInfo = new ArtistInfo();

                            artistInfo.No = item.No;
                            artistInfo.KorArtist = item.KorArtist;
                            artistInfo.Genre = item.Genre;

                            artistList.Add(artistInfo);
                        }
                        listview_painter.ItemsSource = artistList;
                        listview_painter.Items.Refresh();
                    }));
                }
                else
                {
                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        MessageBox.Show("오류가 발생했습니다. 다시 시도해주시길 바랍니다..");
                        Uri uri = new Uri("/View/Main.xaml", UriKind.Relative);
                        NavigationService.Navigate(uri);
                    }));

                }
            });
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/Main.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }
    }
}
