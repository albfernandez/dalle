
using System;
using System.IO;
using NUnit.Framework;
using Dalle.FileVerification.FileHashers;
using Dalle.Formatos;
namespace nunit
{


	[TestFixture()]
	public class TestSplitters
	{
		FileHasherSHA1 hasher = new FileHasherSHA1 ();
		public TestSplitters ()
		{
			Manager.Instance.Progress += new ProgressEventHandler (this.OnProgress);
		}
		public string GetBaseDir ()
		{
			return "/home/dalle/test_files/splitters/";
		}
		[TearDown()]
		[SetUp()]
		public void CleanTempDir ()
		{
			try {
				//Directory.Delete (Path.GetTempPath () + Path.DirectorySeparatorChar + "test", true);
				File.Delete (Path.GetTempPath() + Path.DirectorySeparatorChar + "aes5.txt");
			}
			catch (Exception e) 
			{
			}
		}
		[Test()]
		public void TestGnomeSplit ()
		{
			Manager.Instance.Unir (GetBaseDir () + "gnomesplit/aes5.txt.001.gsp", Path.GetTempPath ());
			ComprobarResultado (Path.GetTempPath());
		}
		
		public const string sha1sum = "7cb47081866de78c7bc5359b265c941586f04b16";
		private void ComprobarResultado (string dir)
		{
			string realHash = sha1sum;
			string realFile = dir + Path.DirectorySeparatorChar + "aes5.txt";
			Assert.IsTrue (File.Exists (realFile), "Archivo no creado [" + realFile + "]");
			string computedHash = hasher.GenerateHash (realFile);
			Assert.AreEqual (realHash, computedHash);
		}
		
		protected virtual void OnProgress (long done, long total)
		{
			double fraction = ((double)done) / ((double)total);
			Console.WriteLine ("progreso=" + done + "/" + total + "=" + fraction);
		}
	}
}
