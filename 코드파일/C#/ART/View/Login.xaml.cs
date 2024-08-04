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
    public partial class Login : Page
    {
        public Login()
        {
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            var textBox = sender as TextBox;
            textBox.Clear();
        }

        private void Password_GotFocus(object sender, RoutedEventArgs e)
        {
            var passwordBox = sender as PasswordBox;
            passwordBox.Clear();
        }

        private void Init_LoinInfo()
        {
            User user = new User();
            List<User> users = new List<User>();

            user.PW = login_pw.Password.ToString();
            users.Add(user);

            Main.data.Key = (int)KEY.LOGIN;
            Main.data.ID = login_id.Text.ToString();
            Main.data.user = users;
        }

        private void btn_loginCheck_Click(object sender, RoutedEventArgs e)
        {
            Init_LoinInfo();

            int result = Main.CppSV.User_Info();
            if (result == (int)KEY.OK)
            {
                Main.check.Login_Check = true;
                MessageBox.Show("로그인이 완료되었습니다.", "로그인 안내 메세지");

                Uri uri = new Uri("/View/Main.xaml", UriKind.Relative);
                NavigationService.Navigate(uri);
            }
            else if(result == (int)KEY.NO)
            {
                Main.check.Login_Check = false;
                MessageBox.Show("일치하는 정보가 없습니다.", "로그인 안내 메세지");
            }
            else
            {
                Main.check.Login_Check = false;
                MessageBox.Show("오류가 발생했습니다. 다시 시도해주시길 바랍니다.", "로그인 안내 메세지");
            }
        }

        private void btn_loginback_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/Main.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        private void btn_join_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("회원가입을 진행하시겠습니까?", "회원가입 안내 메세지"
                , MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                Uri uri = new Uri("/View/Join.xaml", UriKind.Relative);
                NavigationService.Navigate(uri);
            }
        }
    }
}
