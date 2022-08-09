using System;
using System.Windows;

namespace ComicC;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    public static new App Current => Application.Current as App ??
        throw new InvalidOperationException($"Current Application is not {nameof(App)}");
}
