using FubuCore;
using Rhino.Mocks;

namespace ripple.Testing
{
    public static class DataMother
    {
        public static void WriteXmlFile(string file, string text)
        {
            text = text.Replace("'", "\"").TrimStart();
            new FileSystem().WriteStringToFile(file, text);
        }

        public static IFileSystem MockedFileSystem()
        {
            return MockRepository.GenerateMock<IFileSystem>();
        }


        public static void CreateDataFolder()
        {
            var fileSystem = new FileSystem();
            fileSystem.CleanDirectory("data");
            fileSystem.DeleteDirectory("data");
            fileSystem.Copy(FileSystem.Combine("..", "..", "data"), "data");

        }
    }
}