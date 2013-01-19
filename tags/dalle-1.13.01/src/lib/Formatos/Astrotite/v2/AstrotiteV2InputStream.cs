/*

    Dalle - A split/join file utility library
    Dalle.Formatos.Astrotite.v2.Astrotite.AstrotiteV2InputStream
          Join files in astrotite format.
	
    Copyright (C) 2003-2010 Alberto Fernández  <infjaf@gmail.com>

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
using Dalle.Utilidades;

namespace Dalle.Formatos.Astrotite.v2
{
	public class BlockHeader
	{
		public BlockHeader (string block, int size, int crc)
		{
			this.block = block;
			this.size = size;
			this.crc = crc;
		}
		private string block;
		private int size;
		private int crc;
		
		public string Block {
			get { return this.block; }
		}
		public int Size {
			get { return this.size; }
		}
		public int Crc {
			get { return this.crc; }
		}
	}
	
	
	

	public class AstrotiteV2InputStream : ArchiveInputStream
	{
		private ArrayList entryList = new ArrayList ();
		private int entryCursor = 0;
		private int nArchivos = 0;
		private byte[] tmpBuffer = new byte[Consts.BUFFER_LENGTH];
		private AstrotiteV2Entry currentEntry;
		private BlockHeader currentBlock = null;
		private int currentBlockNumber = 0;
		private bool finished = false;
		
		public override ArchiveEntry GetNextEntry ()
		{
			return GetNextAstrotiteEntry ();
		}
		public AstrotiteV2Entry GetNextAstrotiteEntry ()
		{
			
			if (dataStream != null) {
				while (this.Read (tmpBuffer) > 0) 
				{
				}
			}
			dataStream = null;
			
			if (entryCursor >= entryList.Count) {
				if (!finished) 
				{
					finished = true;
					byte[] finalbuffer = new byte[3];
					int leidos = this.inputStream.Read (finalbuffer, 0, 3);
					this.Count (leidos);
					StringBuilder tmp = new StringBuilder ();
					for (int i = 0; i < "EOF".Length; i++) {
						tmp.Append (Convert.ToChar (finalbuffer[i]));
					}
					if (!"EOF".Equals (tmp.ToString ())) {
						throw new IOException ("Unexpected EOF:[" + tmp.ToString () +"]");
					}
				}
				return null;
			}
			this.currentEntry = (AstrotiteV2Entry)entryList[entryCursor++];
			return this.currentEntry;
		}
		

		public AstrotiteV2InputStream (Stream inputStream) : base(inputStream)
		{
			this.Init ();
		}

		
		private void Init ()
		{
			byte[] initbuffer = new byte[255];
			int leidos = 0;
			leidos = this.inputStream.Read (initbuffer, 0, 22);
			if (leidos != 22)
				throw new IOException ();
			this.Count (leidos);
			// Comprobar magic
			StringBuilder tmp = new StringBuilder ();
			for (int i = 0; i < "AST2www.astroteam.tk".Length; i++) {
				tmp.Append (Convert.ToChar (initbuffer[i]));
			}
			if (!"AST2www.astroteam.tk".Equals (tmp.ToString ())) {
				throw new IOException ("Unexpected magic:" + tmp.ToString());
			}
			
			nArchivos = (int)UtArrays.LeerInt16 (initbuffer, 20);
			for (int i = 0; i < nArchivos; i++) {
				leidos = inputStream.Read (initbuffer, 0, 9);
				this.Count (leidos);
				int length = UtArrays.LeerInt32 (initbuffer, 0);
				// Astrotite genera siempre un bloque por archivo. Si el archivo está vacio, tambien
				// Pero graba un 0 en el numero de bloques.
				int blocks = UtArrays.LeerInt32 (initbuffer, 4);
				if (blocks == 0)
				{
					blocks = 1;
				}
				int namelength = initbuffer[8];
				leidos = inputStream.Read (initbuffer, 0, namelength);
				this.Count (leidos);
				string name = UtArrays.LeerTexto (initbuffer, 0, namelength);
				name = name.Replace ('\\', Path.DirectorySeparatorChar);
				AstrotiteV2Entry e = new AstrotiteV2Entry (name, length, blocks);
				entryList.Add (e);
			}
		}
		private AstrotiteCRC crc = new AstrotiteCRC ();
		public override int Read (byte[] buffer, int offset, int count)
		{
			int leidos = 0;
			if (dataStream == null) 
			{
				leidos = inputStream.Read (tmpBuffer, 0, 3);
				this.Count (leidos);
				if (leidos != 3) 
				{
					return 0;
				}
				StringBuilder sb = new StringBuilder ();
				for (int i = 0; i < 3; i++) 
				{
					sb.Append ((char)tmpBuffer[i]);
				}
				string it = sb.ToString ();
				if (!it.Equals ("SDT") && !it.Equals ("FDA")) 
				{
					throw new IOException ("Unexpected file start tag:[" + it + "]");
				}
				this.currentBlock = ReadBlockHeader ();
				this.currentBlockNumber = 1;
				crc.Reset ();
				
				dataStream = new ChecksumStream (new SizeLimiterStream (inputStream, currentBlock.Size), crc);
			}
			leidos = dataStream.Read (buffer, offset, count);
			this.Count (leidos);
			// Se termina el bloque
			if (leidos < count) {
				if (this.crc.Value != this.currentBlock.Crc) 
				{
					throw new IOException ("Invalid checksum " + this.currentEntry.Name + ":" + this.currentBlockNumber);
				}
				if (this.currentBlockNumber == this.currentEntry.NumBlocks) 
				{					
					return leidos;
				}
				this.currentBlock = ReadBlockHeader ();
				this.currentBlockNumber++;
				crc.Reset ();
				dataStream =  new ChecksumStream(new SizeLimiterStream (inputStream, currentBlock.Size), crc);
				return leidos + this.Read (buffer, offset + leidos, count - leidos);
			}
			return leidos;
		}

		private BlockHeader ReadBlockHeader ()
		{
			int leidos = this.inputStream.Read (tmpBuffer, 0, 9);
			if (leidos != 9)
			{
				throw new IOException ();
			}
			this.Count (leidos);
			
			
			string _block = UtArrays.LeerTexto (tmpBuffer, 0, 3);
			int _size = UtArrays.LeerUInt16 (tmpBuffer, 3);
			int _crc = UtArrays.LeerInt32 (tmpBuffer, 5);
			if (!_block.Equals("BLD")){
				throw new IOException("unexpected block start tag [" + _block + "]");
			}
			return new BlockHeader(_block, _size, _crc);
			



		}
	}
}

