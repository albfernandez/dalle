// created on 13/12/2004 at 23:07




using System.Security.Cryptography;
using System.IO;


namespace Dalle.FileVerification.FileHashers
{
	public class FileHasherSHA1 : IFileHasher
	{
		public FileHasherSHA1 ()
		{
		}
		public string GenerateHash (string fileName)
		{
			HashAlgorithm h = SHA1.Create();
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
