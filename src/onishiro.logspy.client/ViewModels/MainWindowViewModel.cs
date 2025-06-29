namespace onishiro.logspy.client.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public LogViewModel LogViewModel { get; set; }
    public MainWindowViewModel()
    {
        LogViewModel = new LogViewModel();
    }
}
