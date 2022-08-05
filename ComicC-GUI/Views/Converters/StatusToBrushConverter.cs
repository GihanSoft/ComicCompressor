using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

using ComicC.Logics;

namespace ComicC.Views.Converters;

[ValueConversion(typeof(ChapterVM.ChapterStatus), typeof(Brush))]
public class StatusToBrushConverter : IValueConverter
{
    public static readonly StatusToBrushConverter Instance = new();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var status = (ChapterVM.ChapterStatus)value;

        return status switch
        {
            ChapterVM.ChapterStatus.Started => new SolidColorBrush(Colors.LightYellow),
            ChapterVM.ChapterStatus.Succeed => new SolidColorBrush(Colors.LightGreen),
            ChapterVM.ChapterStatus.Failed => new SolidColorBrush(Colors.PaleVioletRed),
            _ => App.Current.TryFindResource("MahApps.Brushes.ThemeBackground") ?? new SolidColorBrush(Colors.White)
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return null;
    }
}
