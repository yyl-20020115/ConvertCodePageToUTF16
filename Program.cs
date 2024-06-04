using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;

namespace ConvertCodePageToUTF16;

public class Program
{
    public static string GetResultPathName(string path, string ext = ".txt")
    {
        var folder = Path.GetDirectoryName(path);
        var name = Path.GetFileName(path);
        return Path.Combine(folder ?? ".", name + ext);
    }
    public static int Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var path = ".";
        var encoding_name = "GB2312";
        var filter = "*.*";
        switch (args.Length)
        {
            case 0:
                Console.WriteLine("ConvertCodePageToUTF16 [path_of_file_or_folder] [codepage/encoding] [filter]");
                return -1;
            case 1:
                path = args[0];
                break;
            case 2:
                path = args[0];
                encoding_name = args[1];
                break;
            case 3:
                path = args[0];
                encoding_name = args[1];
                filter = args[2];
                break;
        }
        var is_file = false;

        if (Directory.Exists(path))
        {
            is_file = false;
        }
        else if (File.Exists(path))
        {
            is_file = true;
        }
        else
        {
            Console.WriteLine($"Path {path} does not exists!");
            return -1;
        }

        var encoding = Encoding.GetEncoding(encoding_name);
        if (encoding == null)
        {
            Console.WriteLine($"Encoding {encoding_name} does not exists!");
            return -1;
        }
        var utf16le = Encoding.Unicode;
        if (is_file)
        {
            var _path = path;
            var output_path = GetResultPathName(_path, ".txt");
            {
                using var reader = new StreamReader(_path, encoding, true);
                using var writer = new StreamWriter(output_path, false, utf16le);
                string? line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    writer.WriteLine(line);
                }
            }

            File.Delete(_path + ".bak");

            File.Move(_path, _path + ".bak");
            File.Move(output_path, _path);
        }
        else
        {
            var _paths = Directory.GetFiles(path, filter, SearchOption.AllDirectories);
            foreach (var _path in _paths)
            {
                var output_path = GetResultPathName(_path, ".txt");
                {
                    using var reader = new StreamReader(_path, encoding, true);
                    using var writer = new StreamWriter(output_path, false, utf16le);
                    string? line = null;
                    while ((line = reader.ReadLine()) != null)
                    {
                        writer.WriteLine(line);
                    }
                }
                File.Delete(_path + ".bak");
                File.Move(_path, _path + ".bak");
                File.Move(output_path, _path);

            }
        }

        return 0;
    }
}
