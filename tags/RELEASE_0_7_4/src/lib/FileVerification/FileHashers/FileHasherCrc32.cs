using System.IO;

using ICSharpCode.SharpZipLib.Checksums;
using Dalle.Utilidades;


namespace Dalle.FileVerification.FileHashers
{
	public class FileHasherCrc32 : IFileHasher
	{
		private Crc32 crc = new Crc32 ();
		public FileHasherCrc32 ()
		{
		}
		public string GenerateHash (string fileName)
		{
			crc.Reset ();
			UtilidadesFicheros.GenerateHash (fileName, crc);		
			string ret = crc.Value.ToString ("X").ToUpper();
			while (ret.Length < 8)
				ret = "0" + ret;
			return ret;
		}
	}
}
