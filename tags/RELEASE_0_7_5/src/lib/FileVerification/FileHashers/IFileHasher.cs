

namespace Dalle.FileVerification.FileHashers
{
	public interface IFileHasher
	{
		string GenerateHash (string fileName);
	}
}
