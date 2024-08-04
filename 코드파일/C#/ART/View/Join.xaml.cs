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
    public partial class Join : Page
    {
        public Join()
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

        private void btn_joinback_Click(object sender, RoutedEventArgs e)
        {
            Uri uri = new Uri("/View/Main.xaml", UriKind.Relative);
            NavigationService.Navigate(uri);
        }

        private void btn_idcheck_Click(object sender, RoutedEventArgs e)
        {
            Main.data.Key = (int)KEY.ID_CHECK;
            Main.data.ID = join_id.Text.ToString();

            int result = Main.CppSV.ID_Check();
            if (result == (int)KEY.OK)
            {
                Main.check.ID_Check = true;
                MessageBox.Show("사용가능한 아이디 입니다.", "아이디 중복확인 메세지");
            }
            else
            {
                Main.check.ID_Check = false;
                MessageBox.Show("이미 사용 중인 아이디 입니다.", "아이디 중복확인 메세지");
                join_id.Clear();
            }
        }

        private void Init_JoinInfo()
        {
            User user = new User();
            List<User> users = new List<User>();


            user.PW = join_pw1.Password.ToString();
            user.Name = join_name.Text.ToString();
            user.PhoneNum = join_phone.Text.ToString();

            users.Add(user);

            Main.data.Key = (int)KEY.JOIN;
            Main.data.ID = join_id.Text.ToString();
            Main.data.user = users;
        }

        private void btn_joinCheck_Click(object sender, RoutedEventArgs e)
        {
            Init_JoinInfo();

            int result = Main.CppSV.User_Info();
            if (result == (int)KEY.OK)
            {
                MessageBox.Show("회원가입이 완료되었습니다.", "회원가입 안내 메세지");

                Uri uri = new Uri("/View/Main.xaml", UriKind.Relative);
                NavigationService.Navigate(uri);
            }
            else
            {
                MessageBox.Show("오류가 발생했습니다. 다시 시도해주시길 바랍니다.", "회원가입 안내 메세지");
            }
        }

    }
}
