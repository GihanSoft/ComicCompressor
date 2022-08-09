using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ComicC.Logics;

public class ChapterVM : INotifyPropertyChanged
{
    private ChapterStatus status;
    private Exception exception;
    private string path;

    public event PropertyChangedEventHandler PropertyChanged;

    public string Path
    {
        get => path;
        set
        {
            path = value;
            OnPropertyChanged();
        }
    }

    public ChapterStatus Status
    {
        get => status;
        set
        {
            status = value;
            OnPropertyChanged();
        }
    }

    public Exception Exception
    {
        get => exception;
        set
        {
            exception = value;
            OnPropertyChanged();
        }
    }

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public enum ChapterStatus
    {
        None = 0,
        Started = 1,

        Succeed = 2,
        Failed = -2,
    }
}
