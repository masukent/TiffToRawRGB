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

    enum PixelFormat
    {
        Default,
        R16,
        RGB16,
    }

    class Program
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <param name="pixelFormat"></param>
        /// <returns></returns>
        static int Convert(ref byte[] src, ref byte[] dest, int width, int height, PixelFormat pixelFormat)
        {
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    int pos = (y * width) + x;

                    int srcStride = 6;
                    int destStride = 2;

                    // copy 2bytes each
                    for (int d = 0; d < 2; ++d)
                    {
                        dest[pos * destStride + d] = src[(pos * srcStride) + d];
                    }

                }
            }


            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="output"></param>
        static int Convert(String input, String output, PixelFormat pixelFormat)
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
                        stride = (img.PixelWidth * 2); // 16bit = 2

                        img.CopyPixels(data, stride, 0);
                    }
                    else if (img.Format == PixelFormats.Rgb48)
                    {
                        data = new byte[img.PixelWidth * img.PixelHeight * 6];
                        stride = (img.PixelWidth * 6); // 48bit = 6

                        img.CopyPixels(data, stride, 0);

                        // Change rgb to r.
                        if(pixelFormat == PixelFormat.R16)
                        {
                            var newData = new byte[img.PixelWidth * img.PixelHeight * 2];

                            Convert(ref data, ref newData, img.PixelWidth, img.PixelHeight, pixelFormat);

                            data = newData;
                        }
                    }

                    using (var writer = new BinaryWriter(new FileStream(output, FileMode.Create)))
                    {
                        writer.Write(data, 0, data.Length);
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
        static int ParseArgs(string[] args, ref string inputFile, ref string outputFile, ref PixelFormat pixelFormat)
        {
            if (args.Length < 4)
            {
                Console.WriteLine("[ERROR] Invalid args.");
                Console.WriteLine("        TiffToRawRGB.exe -i <input> -o <output>");
                Console.WriteLine("        option -f R16 or RGB16");
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

                    case "-f":
                        ++argIdx;
                        var format = args[argIdx].ToUpper();
                        Enum.TryParse(format, out pixelFormat);
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
            PixelFormat pixelFormat = PixelFormat.Default;

            string inputFile = string.Empty;
            string outputFile = string.Empty;

            // Parse commandline arguments.
            if ( (ret = ParseArgs(args, ref inputFile, ref outputFile, ref pixelFormat) ) != 0 )
            {
                return ret;
            }
            
            // Convert tiff file
            if ( (ret = Convert(inputFile, outputFile, pixelFormat)) != 0 )
            {
                Console.WriteLine("Failure");
                return ret;
            }

            Console.WriteLine("[Output] " + outputFile);
            Console.WriteLine("         Format:" + pixelFormat);

            Console.WriteLine("Successfully");
            return 0;
        }
    }
}
