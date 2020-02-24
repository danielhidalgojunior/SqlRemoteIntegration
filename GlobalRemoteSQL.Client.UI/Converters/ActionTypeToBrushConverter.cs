using GlobalRemoteSQL.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace GlobalRemoteSQL.UI.Converters
{
    public class ActionTypeToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var obj = value as ActionType?;
            SolidColorBrush result = null;

            switch (obj.Value)
            {
                case ActionType.Received: result = Brushes.LightBlue; break;
                case ActionType.Executed: result = Brushes.LightGreen; break;
                case ActionType.Returned: result = Brushes.LightSalmon; break;
                case ActionType.Error: result = Brushes.Red; break;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
