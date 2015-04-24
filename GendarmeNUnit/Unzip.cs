using ICSharpCode.SharpZipLib.Zip;
using System.IO;

namespace GiskardSolutions.GendarmeNUnit
{
    internal class Unzip
    {
        public Unzip(Stream zipStream, string dest)
        {
            _zipStream = zipStream;
            _destination = dest;
        }

        public void Extract()
        {
            using (ZipInputStream s = new ZipInputStream(_zipStream))
            {
                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);

                    if (!string.IsNullOrWhiteSpace(directoryName))
                    {
                        Directory.CreateDirectory(Path.Combine(_destination, directoryName));
                    }

                    if (!string.IsNullOrWhiteSpace(fileName))
                    {
                        using (FileStream streamWriter = File.Create(Path.Combine(_destination, theEntry.Name)))
                        {
                            int size = 2048;
                            var data = new byte[size];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                    streamWriter.Write(data, 0, size);
                                else
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private readonly Stream _zipStream;
        private readonly string _destination;
    }
}
