using OpenCvSharp;
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
using System.IO;
using OpenCvSharp.WpfExtensions;

namespace ART.View
{

    public partial class Search : Page
    {
        public static PyServer pySV = new PyServer();
        public VideoCapture video;
        public Mat frame;
        private bool isRunning;

        public Search()
        {
            InitializeComponent();
            StartCamera();
        }

        private void btn_searchback_Click(object sender, RoutedEventArgs e)
        {
            frame?.Dispose();
            video?.Release();

            Uri uri = new Uri("/View/Main.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        public void StartCamera()
        {
            isRunning = true;
            Task.Run(async () =>
            {
                video = new VideoCapture(0);

                while (isRunning)
                {
                    frame = new Mat();
                    video.Read(frame);

                    if (frame.Empty())
                    {
                        continue;
                    }

                    await Dispatcher.BeginInvoke(new Action(() =>
                    {
                        img_camera.Source = OpenCvSharp.WpfExtensions.WriteableBitmapConverter.ToWriteableBitmap(frame);
                    }));

                    if (Cv2.WaitKey(33) > 0)
                    {
                        isRunning = false;
                    }
                }

                frame?.Dispose();
                video?.Release();
            });
        }

        private void btn_result_Click(object sender, RoutedEventArgs e)
        {

            if (frame == null || frame.Empty())
            {
                MessageBox.Show("프레임 비어있음.");
            }

            BitmapSource bitmapSource = BitmapSourceConverter.ToBitmapSource(frame);
            Task.Run(() =>
            {
                Dispatcher.BeginInvoke(() =>
                {
                    BitmapImage bimg = pySV.ConvertBitmapSourceToBitmapImage(bitmapSource);

                    int result = pySV.Send_Image(bimg);

                    if (result == (int)KEY.OK)
                    {
                        painter_name.Content = Main.data.Value;
                        result = Main.CppSV.ArtInfo();

                        if (result == (int)KEY.OK)
                        {
                            Init_ArtInfo();
                        }
                        else
                        {
                            MessageBox.Show("오류가 발생했습니다. 다시 시도해주시길 바랍니다.");
                        }
                    }
                });
            });

        }

        private void Init_ArtInfo()
        {
            Dispatcher.BeginInvoke(() =>
            {
                painter_name.Content = Main.data.ArtistList[0].KorArtist;
                painter_years.Content = Main.data.ArtistList[0].Years;
                painter_genre.Content = Main.data.ArtistList[0].Genre;
                painter_nation.Content = Main.data.ArtistList[0].Nationality;
            });
        }

    }
    
}

