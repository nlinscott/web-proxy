namespace RequestForwarding
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

        public static IEnumerable<T> AsEnumerable<T>(this T item)
        {
            yield return item;
        }
    }
}
