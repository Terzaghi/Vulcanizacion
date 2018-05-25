using System.Collections.Generic;

namespace RequestManager.Utils
{
    /// <summary>
    /// Comparador de orden descendente
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class ReverseComparer<T> : IComparer<T>
    {
        private readonly IComparer<T> original;

        public ReverseComparer(IComparer<T> original)
        {
            this.original = original;
        }

        public int Compare(T left, T right)
        {
            return original.Compare(right, left);
        }
    }
}
