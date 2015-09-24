using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CodeStore8UI.Model
{
    public class SaltStorage
    {
        public Dictionary<string, string> FileNameSaltDict { get; set; }

        public SaltStorage()
        {
            FileNameSaltDict = new Dictionary<string, string>();
        }      

        public async Task LoadFromStorage()
        {
            var salts = await FileUtilities.GetSaltFile();
            FileNameSaltDict = salts ?? FileNameSaltDict;
        }

        public async Task SaveToStorage()
        {
            await FileUtilities.SaveSaltFile(FileNameSaltDict);
        }
    }
}
