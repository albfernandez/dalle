/*

	Dalle - A split/join file utility library
	Dalle.NUnit.TestSplitters - NUnit test for splitter formats
		
    Copyright (C) 2003-2010 Alberto Fernández <infjaf@gmail.com>

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.


*/

using System;
using System.IO;
using NUnit.Framework;
using Dalle.FileVerification.FileHashers;
using Dalle.Formatos;
namespace Dalle.Nunit
{


	[TestFixture()]
	public class TestSplitters
	{
		
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
				File.Delete (Path.GetTempPath () + Path.DirectorySeparatorChar + "aes5.txt");
			}
			catch (Exception) 
			{
			}
		}
		
		[Test()]
		public void TestAxmanNormal() 
		{
			Manager.Instance.Unir(GetBaseDir() + "axman/normal/cortado.1.axman", Path.GetTempPath());
			ComprobarResultado (Path.GetTempPath());
		}
		[Test()]
		public void TestAxmanZip ()
		{
			Manager.Instance.Unir (GetBaseDir () + "axman/zip/cortado.zip.1.axman", Path.GetTempPath ());
			ComprobarResultado (Path.GetTempPath ());
		}
		
		[Test()]
		public void TestGnomeSplit ()
		{
			Manager.Instance.Unir (GetBaseDir () + "gnomesplit/aes5.txt.001.gsp", Path.GetTempPath ());
			ComprobarResultado (Path.GetTempPath());
		}
		[Test()]
		public void TestGzip ()
		{
			Manager.Instance.Unir (GetBaseDir () + "aes5.txt.gz", Path.GetTempPath ());
			ComprobarResultado (Path.GetTempPath ());
		}
		[Test()]
		public void TestLzma ()
		{
			Manager.Instance.Unir (GetBaseDir () + "aes5.txt.lzma", Path.GetTempPath ());
			ComprobarResultado (Path.GetTempPath ());
		}
		[Test()]
		public void TestBzip2 ()
		{
			Manager.Instance.Unir (GetBaseDir () + "aes5.txt.bz2", Path.GetTempPath ());
			ComprobarResultado (Path.GetTempPath ());
		}
		
		
		public const string sha1sum = "7cb47081866de78c7bc5359b265c941586f04b16";
		private void ComprobarResultado (string dir)
		{
			string realHash = sha1sum;
			string realFile = dir + Path.DirectorySeparatorChar + "aes5.txt";
			Assert.IsTrue (File.Exists (realFile), "Archivo no creado [" + realFile + "]");
			FileHasherSHA1 hasher = new FileHasherSHA1 ();
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
