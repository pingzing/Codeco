using Codeco.CrossPlatform.Models;
using GalaSoft.MvvmLight;

namespace Codeco.CrossPlatform.ViewModels
{
    public class SimpleFileInfoViewModel : ViewModelBase
    {
        private string _name;
        public string Name
        {
            get => _name;
            set => Set(ref _name, value);
        }

        private string _path;
        public string Path
        {
            get => _path;
            set => Set(ref _path, value);
        }

        private FileLocation _fileLocation;
        public FileLocation FileLocation
        {
            get => _fileLocation;
            set
            {
                Set(ref _fileLocation, value);
                RaisePropertyChanged(nameof(SwitchLocationText));
            }
        }

        public string SwitchLocationText => FileLocation == FileLocation.Local ? "Switch to Roamed" : "Switch to Local";

        public SimpleFileInfoViewModel() { }

        public SimpleFileInfoViewModel(SimpleFileInfo backingModel)
        {
            Name = backingModel.Name;
            Path = backingModel.Path;
            FileLocation = backingModel.FileLocation;
        }

        public SimpleFileInfo AsBasicModel()
        {
            return new SimpleFileInfo
            {
                FileLocation = this.FileLocation,
                Name = this.Name,
                Path = this.Path
            };
        }
    }
}
