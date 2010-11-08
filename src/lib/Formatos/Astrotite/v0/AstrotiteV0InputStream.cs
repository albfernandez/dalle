/*

    Dalle - A split/join file utility library
    Dalle.Formatos.Astrotite.Astrotite.v0.AstrotiteV0InputStream
          Join files in astrotite format (version 0.02).
	
    Copyright (C) 2004-2010 Alberto Fernández  <infjaf@gmail.com>

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
using System.Text;

using Dalle.Archivers;
using Dalle.Streams;
namespace Dalle.Formatos.Astrotite.v0
{


	public class AstrotiteV0InputStream : ArchiveInputStream
	{
		
		private ArrayList entryList = new ArrayList();
		private int entryCursor = 0;
		private byte[] buffer = new byte[Consts.BUFFER_LENGTH];

		private int nArchivos = 0;
		public AstrotiteV0InputStream (Stream inStream) : base(inStream)
		{
			this.Init ();
		}
		private void Init ()
		{
			this.nArchivos = (int)ReadNumber ();
			long prevPos = 0;
			for (int i = 0; i < nArchivos; i++) 
			{
				string fileName = ReadText ();
				long iPos = ReadNumber ();
				if (entryList.Count > 0)
				{
					((AstrotiteV0Entry)(entryList[entryList.Count - 1])).Size = iPos - prevPos;
					prevPos = iPos;
				}
				AstrotiteV0Entry e = new AstrotiteV0Entry (fileName, -1);
				entryList.Add (e);
			}
		}
		public override ArchiveEntry GetNextEntry ()
		{
			return GetNextAstrotiteEntry ();
		}
		public AstrotiteV0Entry GetNextAstrotiteEntry ()
		{
			
			if (dataStream != null) 
			{
				while (this.Read (buffer) > 0) 
				{
				}
			}
			dataStream = null;
			
			if (entryCursor >= entryList.Count)
			{
				return null;
			}
			AstrotiteV0Entry entry = (AstrotiteV0Entry)entryList[entryCursor++];
			this.dataStream = new SizeLimiterStream (this.inputStream, entry.Size);
			return entry;
		}
		private long ReadNumber ()
		{
			int b = 0;
			StringBuilder numero = new StringBuilder();
			while ((b = inputStream.ReadByte()) != 0x0A)
			{
				if (b < 0) 
				{
					throw new IOException("Invalid file format");
				}
				numero.Append((char) b);
				this.Count(1);
			}
			this.Count(1);
			return Int64.Parse(numero.ToString());
		}
		private string ReadText ()
		{
			int b = 0;
			StringBuilder texto = new StringBuilder();
			while ((b = inputStream.ReadByte()) != 0x2A)
			{
				if (b < 0) 
				{
					throw new IOException("Invalid file format");
				}
				if (b == 0x5C) b = 0x2F;
				texto.Append((char) b);
				this.Count(1);
			}
			this.Count(1);
			return texto.ToString();
		}
	}
}
