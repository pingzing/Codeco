using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Codeco.Model
{
    public class IVStorage
    {
        public Dictionary<string, string> FileNameIVDict { get; set; }

        public IVStorage()
        {
            FileNameIVDict = new Dictionary<string, string>();
        }      

        public async Task LoadFromStorage()
        {
            var ivs = await FileUtilities.GetIVFile();
            FileNameIVDict = ivs ?? FileNameIVDict;
        }

        public async Task SaveToStorage()
        {
            await FileUtilities.SaveIVFile(FileNameIVDict);
        }
    }
}
