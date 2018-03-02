using Codeco.Windows10.Services;

namespace Codeco.Windows10.ViewModels.DesignViewModels
{
    public class MainViewModelDesign : MainViewModel
    {
        public MainViewModelDesign(IFileService fileService, INavigationServiceEx navService) : base(fileService, navService)
        {
        }
    }
}
