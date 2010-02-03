/*

    Dalle - A split/join file utility library
	
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
using System.IO.Compression;
using ICSharpCode.SharpZipLib.Checksums;
using ICSharpCode.SharpZipLib.GZip;

using Dalle;
using Dalle.Formatos;
using Dalle.Utilidades;


namespace Dalle.Formatos.SplitterMerger
{


	public class SplitterMerger : Parte 
	{

		public SplitterMerger ()
		{
			nombre = "splittermerger";
			descripcion = "Splitter & Merger";
			web = "_";
			parteFicheros = false;
			compatible = false;
	
		}
		protected override  void _Partir (string fichero, string sal1, string dir, long kb){
		
		}
		
	
		protected override void _Unir (string fichero, string dirDest)
		{
			SplitterMergerInfo info = SplitterMergerInfo.GetInfoFromFile (fichero);
			if (info != null)
			{
				if (info.Compression == SplitterMergerCompression.V3_COMPRESSION)
				{
					throw new FormatNotSupportedException ("Compression in Splitter&Merger 3.0 is not supported");
				}
				
				Stream fos = UtilidadesFicheros.CreateWriter (dirDest + Path.DirectorySeparatorChar + info.OriginalFileName);
				ChecksumStream validator = null;
				if (info.ValidateCrc)
				{
					validator = new ChecksumStream (fos, new Crc32 ());
					fos = validator;
				}
				Stream smInput = new SplitterMergerStream (info);
				Stream input = smInput;
				if (info.Compression == SplitterMergerCompression.GZIP_COMPRESSION)
				{
					input = new GZipStream (input, CompressionMode.Decompress);
				}
				byte[] buffer = new byte[Consts.BUFFER_LENGTH];
				int leidos = 0;
				while ((leidos = input.Read (buffer, 0, buffer.Length)) > 0)
				{
					fos.Write (buffer, 0, leidos);
					OnProgress(smInput.Position, info.Length);
				}								
				fos.Close ();
				if (info.ValidateCrc)
				{
					if (validator.Crc.Value != info.Crc)
					{
						throw new ChecksumVerificationException ("");
					}
				}
			}
		}
		public override bool PuedeUnir (string fichero)
		{
			return (SplitterMergerInfo.GetInfoFromFile (fichero) != null);
		}
	}
}
