using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codeco.Windows10.Models
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
            var ivs = await FileUtilities.GetIVFileContentsAsync();
            FileNameIVDict = ivs ?? FileNameIVDict;
        }

        public async Task SaveToStorage()
        {
            await FileUtilities.SaveIVFile(FileNameIVDict);
        }
    }
}
