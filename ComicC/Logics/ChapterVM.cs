using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ComicC.Logics;

public class ChapterVM : INotifyPropertyChanged
{

    private string path;
    public string Path
    {
        get => path;
        set => SetFieldAndNotifyChanged(ref path, value.ToLower());
    }

    private ChapterStatus status;
    public ChapterStatus Status
    {
        get => status;
        set => SetFieldAndNotifyChanged(ref status, value);
    }

    private Exception exception;
    public Exception Exception
    {
        get => exception;
        set => SetFieldAndNotifyChanged(ref exception, value);
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void SetFieldAndNotifyChanged<T>(ref T field, T value, bool notifyOnlyIfValueChanged = true, [CallerMemberName] string propertyName = null)
    {
        if (notifyOnlyIfValueChanged)
        {
            var same = (field, value) switch
            {
                (IEquatable<T> eq, T _) => eq.Equals(value),
                (IComparable<T> eq, T _) => eq.CompareTo(value) == 0,
                (null, null) => true,
                (null, not null) or (not null, null) => false,

                _ => field.Equals(value),
            };
            if (same)
            {
                return;
            }
        }

        field = value;
        OnPropertyChanged(propertyName);
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
