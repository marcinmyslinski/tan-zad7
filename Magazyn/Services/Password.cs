using System.IO;

namespace Magazyn.Services
{
    public interface IPassword
    {
        public string GetPassword();
    }
    public class Password: IPassword
    {
        public static string pass;
        private string path = @"services\password.txt";

        public Password()
        {
            try
            {
                using (var reader = new StreamReader(path))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        pass = line;
                    }
                }
            }
            catch (FileNotFoundException e)
            {
                File.Create(path).Close();

            }
        }

        public string GetPassword()
        {
            return pass;
        }

    }
}

