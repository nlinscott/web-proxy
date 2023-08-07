namespace OpenApiValidator
{
    internal static class ExtensionMethods
    {
        public static void Action<T>(this IEnumerable<T> src, Action<T> action)
        {
            foreach(T item in src)
            {
                action(item);
            }
        }
    }
}
