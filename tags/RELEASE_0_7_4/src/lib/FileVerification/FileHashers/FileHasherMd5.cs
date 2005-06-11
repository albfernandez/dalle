

using System.Security.Cryptography;
using System.IO;


namespace Dalle.FileVerification.FileHashers
{
	public class FileHasherMd5 : IFileHasher
	{
		public FileHasherMd5 ()
		{
		}
		public string GenerateHash (string fileName)
		{
			HashAlgorithm h = MD5.Create();
			byte[] res = h.ComputeHash (File.OpenRead (fileName));
			string ret = "";
			foreach (byte b in res){
				string tmp = b.ToString ("x");
				while (tmp.Length < 2)
					tmp = "0" + tmp;
				ret += tmp;
			}
			 
			return ret.ToLower();
		}
	}
}
