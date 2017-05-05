using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace DriverChat.tools
{
    public class BoolToVisibility : IValueConverter
    {
        public object Convert(object value, Type TargetType, object parameter, string language)
        {
            bool check = (bool)value;
            return check ? Visibility.Visible : Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type TargetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
