using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TiffToRawRGB
{
    class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        static int Convert(String input, String output)
        {

            try
            {
                using (var stream = new FileStream(input, FileMode.Open))
                {
                    var decoder = new TiffBitmapDecoder(stream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.Default);

                    var img = decoder.Frames[0];

                    int stride = 0;
                    byte[] data = null;

                    if (img.Format == PixelFormats.Gray16)
                    {
                        data = new byte[img.PixelWidth * img.PixelHeight * 2];
                        stride = (img.PixelWidth * 2);
                    }
                    else if (img.Format == PixelFormats.Rgb48)
                    {
                        data = new byte[img.PixelWidth * img.PixelHeight * 6];
                        stride = (img.PixelWidth * 6);
                    }

                    img.CopyPixels(data, stride, 0);
                    using (var writer = new BinaryWriter(new FileStream(output, FileMode.Create)))
                    {
                        writer.Write(data, 0, data.Length);
                        Console.WriteLine("[Output] " + output);
                    }
                }
            }
            catch
            {
                Console.WriteLine("Enter a try-catch section.");
                return 1;
            }

            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <param name="inputFile"></param>
        /// <param name="outputFile"></param>
        /// <returns></returns>
        static int ParseArgs(string[] args, ref string inputFile, ref string outputFile)
        {
            if (args.Length != 4)
            {
                Console.WriteLine("[ERROR] Invalid args.");
                Console.WriteLine("        TiffToRawRGB.exe -i <input> -o <output>");
                return 1;
            }

            for (int argIdx = 0; argIdx < args.Length;)
            {
                switch(args[argIdx])
                {
                    case "-i":
                        ++argIdx;
                        inputFile = args[argIdx];
                        break;

                    case "-o":
                        ++argIdx;
                        outputFile = args[argIdx];
                        break;

                    default:
                        ++argIdx;
                        break;
                }
            }

            if (!System.IO.File.Exists(inputFile))
            {
                Console.WriteLine("[ERROR] The input file does not exist.");
                Console.WriteLine("        " + inputFile);
                return 1;
            }

            if(System.IO.Path.GetExtension(inputFile) != ".tif" && System.IO.Path.GetExtension(inputFile) != ".tiff")
            {
                Console.WriteLine("[ERROR] The input file isn't tiff extensin.");
                Console.WriteLine("        " + inputFile);
                return 1;
            }

            return 0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        static int Main(string[] args)
        {
            int ret = 0;
            string inputFile = string.Empty;
            string outputFile = string.Empty;

            // Parse commandline arguments.
            if ( (ret = ParseArgs(args, ref inputFile, ref outputFile) ) != 0 )
            {
                return ret;
            }
            
            // Convert tiff file
            if ( (ret = Convert(inputFile, outputFile)) != 0 )
            {
                Console.WriteLine("Failure");
                return ret;
            }

            Console.WriteLine("Successfully");
            return 0;
        }
    }
}
