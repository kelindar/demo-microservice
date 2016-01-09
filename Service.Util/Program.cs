using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Emitter;
using Emitter.Messages;

namespace Service.Util
{
    class Program
    {
        static readonly StringBuilder Builder = new StringBuilder();

        static void Main(string[] args)
        {
            var secretKey = "jSWTTnIfdH0BJ0zSGPGGIZkucwAQEMp4";

            Console.WriteLine("Generating keys for emitter.io channels...");
            Builder.AppendLine("namespace Service");
            Builder.AppendLine("{");
            Builder.AppendLine("\tpublic class Key");
            Builder.AppendLine("\t{");

            using (var emitter = Connection.Establish("192.168.56.102", null))
            {
                emitter.GenerateKey(secretKey, "avatar-request", EmitterKeyType.ReadWrite, WriteKey);
                emitter.GenerateKey(secretKey, "avatar-response", EmitterKeyType.ReadWrite, WriteKey);
                emitter.GenerateKey(secretKey, "sentiment-request", EmitterKeyType.ReadWrite, WriteKey);
                emitter.GenerateKey(secretKey, "sentiment-response", EmitterKeyType.ReadWrite, WriteKey);
                emitter.GenerateKey(secretKey, "user-ping", EmitterKeyType.ReadWrite, WriteKey);
                emitter.GenerateKey(secretKey, "user-notify", EmitterKeyType.ReadWrite, WriteKey);
                emitter.GenerateKey(secretKey, "messages", EmitterKeyType.ReadWrite, WriteKey);

                Console.WriteLine("Press any key to write the file...");
                Console.ReadKey();
            }

            Builder.AppendLine("\t}");
            Builder.AppendLine("}");
            File.WriteAllText("../../Security.cs", Builder.ToString());
        }

        /// <summary>
        /// Append the key.
        /// </summary>
        static void WriteKey( KeygenResponse k)
        {
            // Format .NET friendly name
            var keyName = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(
                k.Channel.Replace("-", " ")
                ).Replace(" ", "");

            Builder.AppendFormat("\t\tpublic static readonly string {0} = \"{1}\";", keyName, k.Key);
            Builder.AppendLine();
            Console.WriteLine("{0} = {1}", keyName, k.Key);
        }
    }
}
