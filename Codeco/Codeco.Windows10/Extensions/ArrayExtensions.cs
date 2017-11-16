using System.Linq;

namespace Codeco.Windows10.Extensions
{
    public static class ArrayExtensions
    {
        public static void Deconstruct<T>(this T[] array, out T first, out T second, out T third, out T[] rest)
        {
            first = array[0];
            second = array[1];
            third = array[2];
            rest = array.Skip(3).ToArray();
        }
    }
}
