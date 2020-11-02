using System.IO;
using System.Reflection;

namespace Osupport.Test.Resources
{
    internal static class Resource
    {
        private static readonly Assembly _assembly = typeof(Resource).Assembly;

        public static string GetString(string resource)
        {
            var stream = _assembly.GetManifestResourceStream($"{_assembly.GetName().Name}.Resources.{resource}");
            var streamReader = new StreamReader(stream!);
            return streamReader.ReadToEnd();
        }
    }
}
