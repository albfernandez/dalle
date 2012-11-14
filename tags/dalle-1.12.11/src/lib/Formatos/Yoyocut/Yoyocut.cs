/*

	Dalle - A split/join file utility library
	Dalle.Formatos.HJSplit.HJSplit - 
		Split and Join files in HJSplit format.
	
    Copyright (C) 2003-2010  Alberto Fernández <infjaf@gmail.com>

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
using System.Security.Cryptography;

using Dalle.Formatos;
using Dalle.Utilidades;
using Dalle.Streams;

namespace Dalle.Formatos.Yoyocut
{
	public class Yoyocut : Parte
	{
		public Yoyocut ()
		{
			nombre = "yoyocut2";
			descripcion = "YoyoCut 2";
			web = "http://siteayoyo.free.fr/YoyoCut/";
			parteFicheros = false;
			compatible = false;
		}
		protected override void _Unir (string fichero, string dirDest)
		{
			YoyocutInfo info = YoyocutInfo.GetFromFile (fichero);
			if (info == null)
			{
				throw new FileFormatException ("");
			}
			byte[] buffer = new byte[Consts.BUFFER_LENGTH];
			int leidos = 0;
			long transferidos = 0;
			OnProgress (0, info.Length);
			Stream fos = UtilidadesFicheros.CreateWriter (dirDest + Path.DirectorySeparatorChar + info.OriginalFileName);
			HashAlgorithm md5 = null;
			if (info.IsMd5)
			{
				md5 = MD5.Create ();
			}
			for (int i = 1; i <= info.Fragments; i++) {
				Stream fis = File.OpenRead (info.GetFragmentName (i));
				fis.Seek (info.GetOffset (i), SeekOrigin.Begin);
				Stream cis = fis;
				if (info.IsExe && i == 1) 
				{
					cis = new SizeLimiterStream (cis, info.DataSizeInExe);
				}
				while ((leidos = cis.Read (buffer, 0, (int)Math.Min (buffer.Length, info.Length - transferidos))) > 0)
				{
					fos.Write (buffer, 0, leidos);
					transferidos += leidos;
					if (md5 != null) 
					{
						md5.TransformBlock (buffer, 0, leidos, buffer, 0);
					}
					
					OnProgress (transferidos, info.Length);
				}
				fis.Close ();
			}
			fos.Close ();
			String md5Hash = null;
			if (md5 != null) {
				md5.TransformFinalBlock (buffer, 0, 0);
				byte[] res = md5.Hash;
				md5Hash = UtilidadesCadenas.FormatHexHash (res).ToLower ();
				if (!md5Hash.Equals (info.Md5)) 
				{
					throw new ChecksumVerificationException ("checksum verification failed", fichero);
				}
			}
		}
		
		protected override void _Partir (string fichero, string sal1, string dir, long kb)
		{
			throw new System.NotImplementedException ();
		}

		public override bool PuedeUnir (string fichero)
		{
			if (!File.Exists (fichero)) {
				return false;
			}
			if (fichero.ToLower ().EndsWith (".001.exe") || fichero.ToLower ().EndsWith (".001.yct")) {
				return YoyocutInfo.GetFromFile (fichero) != null;
			}
			return false;
		}
	}	

}

