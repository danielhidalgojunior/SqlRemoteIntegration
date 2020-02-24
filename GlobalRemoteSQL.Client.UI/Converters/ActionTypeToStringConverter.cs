using GlobalRemoteSQL.Core;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace GlobalRemoteSQL.UI.Converters
{
    public class ActionTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var obj = value as ActionType?;
            string result = null;

            switch (obj.Value)
            {
                case ActionType.Received: result = "Recebido"; break;
                case ActionType.Executed: result = "Executado"; break;
                case ActionType.Returned: result = "Retornado"; break;
                case ActionType.Error: result = "Erro"; break;
            }

            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
