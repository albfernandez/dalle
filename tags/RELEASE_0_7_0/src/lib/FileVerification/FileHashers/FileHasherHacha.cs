
using System.IO;

using Dalle.Utilidades;
using Dalle.Formatos.Hacha;


namespace Dalle.FileVerification.FileHashers
{
	public class FileHasherHacha : IFileHasher
	{
		public FileHasherHacha ()
		{
		}
		public string GenerateHash (string fileName)
		{
			HachaCRC crc = new HachaCRC (new FileInfo (fileName).Length);
			crc.Reset();
			UtilidadesFicheros.GenerateHash (fileName, crc);
			return crc.Value.ToString ("X").ToUpper();
		}
	}
}
