
using System;
using Dalle.Archivers;

namespace Dalle.Formatos.Astrotite.v2
{


	public class AstrotiteV2Entry : ArchiveEntry
	{

		private long size;
		private string name;
		private int numBlocks;
		#region ArchiveEntry implementation
		public bool IsDirectory {
			get { return false; }
		}


		public string Name {
			get { return name; }
		}


		public long Size {
			get { return this.size; }
			internal set { this.size = value; }
		}


		#endregion
		
		public int NumBlocks {
			get { return this.numBlocks; }			
		}
		public AstrotiteV2Entry (string name, long size, int numBlocks)
		{
			this.name = name;
			this.size = size;
			this.numBlocks = numBlocks;
		}
		
	}
}
