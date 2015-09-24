using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CodeStore8UI.Model
{
    public class SaltStorage
    {
        public Dictionary<string, string> FileNameSaltDict { get; set; } = new Dictionary<string, string>();

        public SaltStorage() { }

        public SaltStorage(Dictionary<string, string> newDict)
        {
            FileNameSaltDict = newDict;
        }        

        public async Task LoadFromStorage()
        {
            FileNameSaltDict = await FileUtilities.GetSaltFile();
        }

        public async Task SaveToStorage()
        {
            await FileUtilities.SaveSaltFile(FileNameSaltDict);
        }
    }
}
