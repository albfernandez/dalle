
using Dalle.FileVerification.FileHashers;

namespace Dalle.FileVerification
{

	public class SFVElement {
	
		private string file;
		private string hash;
		private IFileHasher hasher;
		private string realHash;
	
		public SFVElement (string file, string hash, IFileHasher hasher)
		{
			this.file = file;
			this.hash = hash;
			this.hasher = hasher;
		}
		
		public string FileName {
			get { return file; }
		}
		public string ExpectedHash {
			get { return hash; }
		}
		public string RealHash {
			get { return realHash; }
		}
		public string GenerateHash ()
		{
			realHash = hasher.GenerateHash (file);
			return realHash;
		}
		public bool IsOk ()
		{
			realHash = GenerateHash();
			return (realHash.ToUpper() == hash.ToUpper());
		}
	}
}
