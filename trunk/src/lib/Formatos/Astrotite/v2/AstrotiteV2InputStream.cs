
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
			Console.WriteLine (tmp.ToString ());
			if (!"AST2www.astroteam.tk".Equals (tmp.ToString ())) {
				throw new IOException ();
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
					throw new IOException ();
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
				throw new IOException();
			}
			return new BlockHeader(_block, _size, _crc);
			



		}
	}
}


/*
 * 
 *
 *
 *

 
			foreach (Descript des in listaFicheros) 
			{
				Console.WriteLine ("Procsando " + des.name);
				byte[] initialMark = new byte[3];
				astReader.Read (initialMark, 0, initialMark.Length);
				string iMark = UtArrays.LeerTexto (initialMark, 0);
				
				if (!"SDT".Equals (iMark) && !"FDA".Equals (iMark))
				{
					throw new FileFormatException ();
				}

				Stream writer = UtilidadesFicheros.CreateWriter (dirDest + Path.DirectorySeparatorChar + des.name);

				int l = 0;
				long bloquesLeidos = 0;
				while (bloquesLeidos < des.blocks)
				{					
					Block block = readBlock (astReader);
					crc.Reset ();					
					int quedan1 = block.size;
					while (quedan1 > 0) 
					{
						l = astReader.Read(initbuffer, 0, Dalle.Consts.BUFFER_LENGTH < quedan1 ? Dalle.Consts.BUFFER_LENGTH: quedan1 );
						writer.Write (initbuffer, 0, l);						
						
						if ((long)block.crc != 0xFFFFFFFF && block.crc != 0){
							crc.Update(initbuffer, 0,  l);
						}
						quedan1 -= l;				
						leidos += l;
						OnProgress (leidos, totales);
					}					
					
					if ((long)block.crc != 0xFFFFFFFF && block.crc != 0 && (long) block.crc != crc.Value){
						throw new Dalle.Formatos.ChecksumVerificationException();
					}	
					bloquesLeidos++;
				}
				writer.Close();							
			}
			astReader.Close();			
		}
		private Block readBlock(FileStream astReader){	
			byte[] info = new byte[9];		
			astReader.Read(info, 0, 9);
			Block block = new Block();
			block.block = UtArrays.LeerTexto (info, 0, 3);
			block.size  = UtArrays.LeerUInt16 (info, 3);
			block.crc   = UtArrays.LeerInt32 (info, 5);
			return block;
		}


*/



/*
 * 
 byte[] initbuffer = new byte[255];
			Descript d;
			astReader.Read (initbuffer, 0, 9);
			d.length = UtArrays.LeerInt32 (initbuffer, 0);
			// Astrotite genera siempre un bloque por archivo. Si el archivo está vacio, tambien
			// Pero graba un 0 en el numero de bloques.
			d.blocks = UtArrays.LeerInt32 (initbuffer, 4);
			if (d.blocks == 0)
			{
				d.blocks = 1;
			}
			d.namelength = initbuffer[8];
			astReader.Read (initbuffer, 0, d.namelength);
			d.name = UtArrays.LeerTexto (initbuffer, 0, d.namelength);
			d.name = d.name.Replace ('\\', Path.DirectorySeparatorChar);
			return d;
 
 */