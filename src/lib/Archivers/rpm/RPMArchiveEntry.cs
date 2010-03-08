
using System;
using System.IO;

using Dalle.Archivers;
namespace Dalle.Archivers.rpm
{


	public class RPMArchiveEntry : ArchiveEntry
	{
		#region ArchiveEntry implementation
		public bool IsDirectory {
			get {
				return this.isDirectory;
			}
		}
		
		
		public string Name {
			get {
				return this.name;
			}
		}
		
		
		public long Size {
			get {
				return this.size;
			}
		}
		
		#endregion

		private long size = 0;
		private string name = "";
		private bool isDirectory = false;
		public RPMArchiveEntry ()
		{
		}
	}
}
