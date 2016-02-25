﻿using Codeco.Windows10.Common;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.Storage;

namespace Codeco.Windows10.Models
{
    public class BindableStorageFile : INotifyPropertyChanged, IBindableStorageFile
    {
        private const uint BYTES_PER_KB = 1024;
        private const uint BYTES_PER_MB = BYTES_PER_KB * 1024;
        private const uint BYTES_PER_GB = BYTES_PER_MB * 1024;
        private ulong? _fileSizeInBytes = null;
        private static AsyncLock bsf_lock = new AsyncLock();       

        private BindableStorageFile() { }

        public static async Task<BindableStorageFile> Create(IStorageFile file)
        {
            BindableStorageFile bsf = new BindableStorageFile();
            bsf._backingFile = file;

            var props = await bsf._backingFile.GetBasicPropertiesAsync();            
            bsf._fileSize = GetHumanReadableSize(props.Size);
           
            return bsf;
        }
        
        public string Name { get { return _backingFile.Name; }}
        public DateTime CreateDate { get { return _backingFile.DateCreated.DateTime; } }

        private bool _isRoamed = false;
        public bool IsRoamed
        {
            get { return _isRoamed; }
            set
            {
                if(_isRoamed == value)
                {
                    return;
                }
                _isRoamed = value;
                RaisePropertyChanged();
            }
        }

        private IStorageFile _backingFile;
        public IStorageFile BackingFile
        {
            get { return _backingFile; }
            set
            {
                if(value == _backingFile)
                {
                    return;
                }
                _backingFile = value;
                UpdateBoundSize(_backingFile);  
                RaisePropertyChanged();
            }
        }        

        private string _fileSize = "0 b";
        public string FileSize
        {
            get { return _fileSize; }            
            private set
            {
                if(value == _fileSize)
                {
                    return;
                }
                _fileSize = value;
                RaisePropertyChanged();
            }
        }        

        private async void UpdateBoundSize(IStorageFile _backingFile)
        {
            var props = await _backingFile.GetBasicPropertiesAsync();
            FileSize = GetHumanReadableSize(props.Size);
        }

        public async Task<ulong> GetFileSizeInBytes()
        {
            if (_fileSizeInBytes == null)
            {
                using (await bsf_lock.Acquire())
                {
                    var props = await _backingFile.GetBasicPropertiesAsync();
                    _fileSizeInBytes = props.Size;
                }
            }
            return _fileSizeInBytes.Value;
        }

        private static string GetHumanReadableSize(ulong size)
        {
            if (size < 1024)
            {
                return $"{size} B"; //bytes
            }
            else if (size > BYTES_PER_KB && size < BYTES_PER_MB)
            {
                return $"{ (double)(size / BYTES_PER_KB)} kB"; //kilobytes
            }
            else if (size > BYTES_PER_MB && size < BYTES_PER_GB)
            {
                return $"{(double)(size / BYTES_PER_MB)} mB"; //megabytes
            }
            else //size > BYTES_PER_GB
            {
                return $"{(double)(size / BYTES_PER_GB)} gB"; //gigabytes
            }
        }        

        public void NameChanged()
        {
            RaisePropertyChanged(nameof(Name));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void RaisePropertyChanged([CallerMemberName]string property = "")
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(property));
        }
    }
}