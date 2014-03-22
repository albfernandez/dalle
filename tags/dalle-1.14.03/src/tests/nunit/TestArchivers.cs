/*

	Dalle - A split/join file utility library
	Dalle.NUnit.TestArvhiers - NUnit test for archive formats
		
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
using System.Collections;
using System.IO;
using NUnit.Framework;
using Dalle;
using Dalle.Formatos;
using Dalle.FileVerification.FileHashers;

namespace Dalle.Nunit
{


	[TestFixture()]
	public class TestArchivers
	{
		Hashtable h = null;
		FileHasherSHA1 hasher = new FileHasherSHA1();

		public TestArchivers ()
		{
			Manager.Instance.Progress += new ProgressEventHandler (this.OnProgress);
		}
		public string GetBaseDir ()
		{
			return "/home/dalle/test_files/archivers/";
		}
		[TearDown()]
		public void CleanTempDir ()
		{
			try {
				Directory.Delete (Path.GetTempPath () + Path.DirectorySeparatorChar + "test", true);
			} catch (Exception) {
			}
			try {
				Directory.Delete (Path.GetTempPath () + Path.DirectorySeparatorChar + "usr", true);
			} catch (Exception) {
			}
			
		}


		
		[Test()]
		public void TestAr ()
		{
			Manager.Instance.Unir (GetBaseDir () + "test.ar", Path.GetTempPath () + Path.DirectorySeparatorChar + "test");
			ComprobarResultado (Path.GetTempPath () + Path.DirectorySeparatorChar + "test", false);
		}
		[Test()]
		public void TestAstrotite ()
		{
			Manager.Instance.Unir (GetBaseDir () + "test.ast", Path.GetTempPath ());
			ComprobarResultado ();
		}
		[Test()]
		public void TestAstrotite2 ()
		{
			Manager.Instance.Unir (GetBaseDir () + "test.ast2", Path.GetTempPath ());
			ComprobarResultado ();
		}
		[Test()]
		public void TestAxman ()
		{
			Manager.Instance.Unir (GetBaseDir () + "axman/axman.zip.1.axman", Path.GetTempPath () + Path.DirectorySeparatorChar + "test");
			ComprobarResultado (Path.GetTempPath () + Path.DirectorySeparatorChar + "test", false);
		}
		[Test()]
		public void TestCpioOldBin ()
		{
			Manager.Instance.Unir (GetBaseDir () + "test.bin.cpio", Path.GetTempPath ());
			ComprobarResultado ();
		}
		[Test()]
		public void TestCpioOldPortable ()
		{
			Manager.Instance.Unir (GetBaseDir () + "test.odc.cpio", Path.GetTempPath ());
			ComprobarResultado ();
		}
		[Test()]
		public void TestCpioNewPortable ()
		{
			Manager.Instance.Unir (GetBaseDir () + "test.newc.cpio", Path.GetTempPath ());
			ComprobarResultado ();
		}
		[Test()]
		public void TestCpioNewPortableCrc ()
		{
			Manager.Instance.Unir (GetBaseDir () + "test.crc.cpio", Path.GetTempPath ());
			ComprobarResultado ();
		}
		[Test()]
		public void TestTar ()
		{
			Manager.Instance.Unir (GetBaseDir () + "test.tar", Path.GetTempPath ());
			ComprobarResultado ();
		}
		[Test()]
		public void TestTarBz2 ()
		{
			Manager.Instance.Unir (GetBaseDir () + "test.tar.bz2", Path.GetTempPath ());
			ComprobarResultado ();
		}
		[Test()]
		public void TestTarGz ()
		{
			Manager.Instance.Unir (GetBaseDir () + "test.tar.gz", Path.GetTempPath ());
			ComprobarResultado ();
		}
		[Test()]
		public void TestTarLZMA ()
		{
			Manager.Instance.Unir (GetBaseDir () + "test.tar.lzma", Path.GetTempPath ());
			ComprobarResultado ();
		}
		[Test()]
		public void TestZip ()
		{
			Manager.Instance.Unir (GetBaseDir () + "test.zip", Path.GetTempPath ());
			ComprobarResultado ();
		}
		[Test()]
		public void TestXarSC ()
		{
			Manager.Instance.Unir (GetBaseDir () + "test.xar", Path.GetTempPath ());
			ComprobarResultado ();
		}
		[Test()]
		public void TestXarBz2 ()
		{
			Manager.Instance.Unir (GetBaseDir () + "test.bz.xar", Path.GetTempPath ());
			ComprobarResultado ();
		}
		[Test()]
		public void TestXarGz ()
		{
			Manager.Instance.Unir (GetBaseDir () + "test.gz.xar", Path.GetTempPath ());
			ComprobarResultado ();
		}
		[Test()]
		public void TestDeb ()
		{
			Manager.Instance.Unir (GetBaseDir () + "test_2010.02.13-1_all.deb", Path.GetTempPath ());
			ComprobarResultado ();
		}
		[Test()]
		public void TestCamouflage ()
		{
			Manager.Instance.Unir (GetBaseDir () + "camouflage.png", Path.GetTempPath () + Path.DirectorySeparatorChar + "test");
			ComprobarResultado (Path.GetTempPath () + Path.DirectorySeparatorChar + "test", false, false);
		}
		
		
		
		private void ComprobarResultado ()
		{
			ComprobarResultado (Path.GetTempPath ());
		}
		
		
		private void ComprobarResultado (string dir)
		{
			ComprobarResultado (dir,true, true);
		}
		
		private void ComprobarResultado (string dir, bool fullPath)
		{
		
		}
		private void ComprobarResultado (string dir, bool fullPath, bool emptyFiles)
		{
			foreach (string key in h.Keys) 
			{
				string realHash = (string)h[key];
				string realFile = dir + Path.DirectorySeparatorChar + key;
				if (!fullPath) 
				{
					realFile = dir + Path.DirectorySeparatorChar + key.Substring (key.LastIndexOf (Path.DirectorySeparatorChar));
				}
				if (realFile.EndsWith ("vacio.txt") && !emptyFiles) 
				{
					continue;
				}
				
				Assert.IsTrue (File.Exists (realFile), "Archivo no creado " + key + "[" + realFile + "]");				
				string computedHash = hasher.GenerateHash (realFile);				
				Assert.AreEqual (realHash, computedHash);				
			}
		}
		
		
		
		[SetUp()]
		public void SetUp ()
		{
			CleanTempDir ();
			h = new Hashtable ();
			h.Add("test/testdir/testfiles/aes.txt", "134e7c465126409a9e8fb15fdaada00c0b4715d7");
			h.Add("test/testdir/testfiles/random.img", "6c4b9579231edd9c4977e59f66633307e1f37722");
			h.Add ("test/testdir/testfiles/vacio.txt", "da39a3ee5e6b4b0d3255bfef95601890afd80709");
			h.Add ("test/testdir/testfiles/zero.img", "7d76d48d64d7ac5411d714a4bb83f37e3e5b8df6");
			h.Add ("test/testdir/testfiles2/lcc.pdf", "1428cbed65d44735b17803992ece46ec140771b8");
			h.Add ("test/testdir/testfiles2/modules/squashfs/Makefile", "12c592c4c16e4a6e218fe6d1443f6828ee6b67df");
			h.Add ("test/testdir/testfiles2/modules/squashfs/debian/changelog", "c99fd86931fbdcfb92fbfd4e82b4bbf2d748ff33");
			h.Add ("test/testdir/testfiles2/modules/squashfs/debian/compat", "5d9474c0309b7ca09a182d888f73b37a8fe1362c");
			h.Add ("test/testdir/testfiles2/modules/squashfs/debian/control.modules.in", "bebc9b7a7594dfd378a1a18046d6811bfed177d0");
			h.Add ("test/testdir/testfiles2/modules/squashfs/debian/copyright", "d7bdf642268e61f3ec85804b7a3c875b9c47d33e");
			h.Add ("test/testdir/testfiles2/modules/squashfs/debian/rules", "94da8862d568fa9a5919911a70115b83c6917aa0");
			h.Add ("test/testdir/testfiles2/modules/squashfs/inode.c", "6352b4d386fda3f523c94dc850c3942aa04a7f3d");
			h.Add ("test/testdir/testfiles2/modules/squashfs/squashfs.h", "223834852a254eb6fb2d0af2757dc2968226f9db");
			h.Add ("test/testdir/testfiles2/modules/squashfs/squashfs2_0.c", "bf06ea8abf01af8acc0c752f4b3f6259b92d0757");
			h.Add ("test/testdir/testfiles2/modules/squashfs/squashfs_fs.h", "dfcf3582b05827844b25a1c67e8321b0246e3f84");
			h.Add ("test/testdir/testfiles2/modules/squashfs/squashfs_fs_i.h", "70b7c75ef822eab77c05f0bdfe825612f51b7c9d");
			h.Add ("test/testdir/testfiles2/modules/squashfs/squashfs_fs_sb.h", "e2f3e1ccc222ed99e45b4ad1b5883abccb7b449e");
			h.Add ("test/testdir/testfiles2/squashfs.tar.bz2", "83240763c718c2d1cc289d4de0ec79c2ebf8f1ca");			
			
			
		}	
		protected virtual void OnProgress (long done, long total)
		{
			double fraction = ((double)done) / ((double)total);
			Console.WriteLine ("progreso=" + done + "/" + total + "=" + fraction);
		}
	}
}
/* 
  
  
  
  
  
  
 
 


*/

/*
134e7c465126409a9e8fb15fdaada00c0b4715d7  test/testdir/testfiles/aes.txt
6c4b9579231edd9c4977e59f66633307e1f37722  test/testdir/testfiles/random.img
da39a3ee5e6b4b0d3255bfef95601890afd80709  test/testdir/testfiles/vacio.txt
7d76d48d64d7ac5411d714a4bb83f37e3e5b8df6  test/testdir/testfiles/zero.img
1428cbed65d44735b17803992ece46ec140771b8  test/testdir/testfiles2/lcc.pdf
12c592c4c16e4a6e218fe6d1443f6828ee6b67df  test/testdir/testfiles2/modules/squashfs/Makefile
c99fd86931fbdcfb92fbfd4e82b4bbf2d748ff33  test/testdir/testfiles2/modules/squashfs/debian/changelog
5d9474c0309b7ca09a182d888f73b37a8fe1362c  test/testdir/testfiles2/modules/squashfs/debian/compat
bebc9b7a7594dfd378a1a18046d6811bfed177d0  test/testdir/testfiles2/modules/squashfs/debian/control.modules.in
d7bdf642268e61f3ec85804b7a3c875b9c47d33e  test/testdir/testfiles2/modules/squashfs/debian/copyright
94da8862d568fa9a5919911a70115b83c6917aa0  test/testdir/testfiles2/modules/squashfs/debian/rules
6352b4d386fda3f523c94dc850c3942aa04a7f3d  test/testdir/testfiles2/modules/squashfs/inode.c
223834852a254eb6fb2d0af2757dc2968226f9db  test/testdir/testfiles2/modules/squashfs/squashfs.h
bf06ea8abf01af8acc0c752f4b3f6259b92d0757  test/testdir/testfiles2/modules/squashfs/squashfs2_0.c
dfcf3582b05827844b25a1c67e8321b0246e3f84  test/testdir/testfiles2/modules/squashfs/squashfs_fs.h
70b7c75ef822eab77c05f0bdfe825612f51b7c9d  test/testdir/testfiles2/modules/squashfs/squashfs_fs_i.h
e2f3e1ccc222ed99e45b4ad1b5883abccb7b449e  test/testdir/testfiles2/modules/squashfs/squashfs_fs_sb.h
83240763c718c2d1cc289d4de0ec79c2ebf8f1ca  test/testdir/testfiles2/squashfs.tar.bz2
 


*/