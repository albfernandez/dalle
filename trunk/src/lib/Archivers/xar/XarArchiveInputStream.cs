/*

    Dalle - A split/join file utility library
	
	
	
    Copyright (C) 2010
	Alberto Fernández  <infjaf@gmail.com>

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
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Xml;

using Dalle.Archivers;
using Dalle.Utilidades;
using Dalle.Streams;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib.Zip.Compression;
using ICSharpCode.SharpZipLib.BZip2;


namespace Dalle.Archivers.xar
{


	public class XarArchiveInputStream : ArchiveInputStream
	{
		private byte[] tmpBuffer = new byte[Consts.BUFFER_LENGTH];
		private XarArchiveEntry currentEntry;
		private int entryCursor = 0;
		
		private XarHashAlgorithm tocHashAlgorithm= XarHashAlgorithm.None;
		private long tocHashOffset = 0;
		private long tocHashSize = 0;
		private ArrayList entryList = new ArrayList();
		private long heapPosition;
		public static readonly int XAR_HEADER_MAGIC = 0x78617221;
		public static readonly int XAR_HEADER_VERSION =  0;
		public static readonly int XAR_HEADER_SIZE = 28;
		private long streamLength = -1;
		
		public override long Length {
			get {
				return this.streamLength;
			}
		}
		public override ArchiveEntry GetNextEntry ()
		{
			return GetNextXarEntry ();
		}
		
		public XarArchiveEntry GetNextXarEntry ()
		{
			if (dataStream != null) {
				while (this.Read (tmpBuffer) > 0) {
				}
				dataStream.Close ();
			}
			dataStream = null;
			
			if (entryCursor >= entryList.Count) {
				return null;
			}
			this.currentEntry = (XarArchiveEntry)entryList[entryCursor++];
			if (this.currentEntry.IsDirectory) {
				return this.currentEntry;
			}
			// TODO Comprobar el offset
			Stream s = new SizeLimiterStream (this.inputStream, this.currentEntry.Length);
			if (this.currentEntry.ArchivedChecksum != null) {
				switch (this.currentEntry.HashAlgorithmArchived) {
				case XarHashAlgorithm.Md5:
					s = new HashStream (s, MD5.Create (), this.currentEntry.ArchivedChecksum);
					break;
				case XarHashAlgorithm.Sha1:
					s = new HashStream (s, SHA1.Create (), this.currentEntry.ArchivedChecksum);
					break;
				default:
					break;
				}
				
			}
			switch (this.currentEntry.Encoding) {
			case XarEncoding.None:
				dataStream = s;
				break;
			case XarEncoding.Gzip:
				dataStream = new InflaterInputStream (s, new Inflater (false));
				break;
			case XarEncoding.Bzip2:
				dataStream = new BZip2InputStream (s);
				break;
			default:
				break;
			}
			if (this.currentEntry.ExtractedChecksum != null) {
				switch (this.currentEntry.HashAlgorithmExtracted) {
				case XarHashAlgorithm.Md5:
					dataStream = new HashStream (dataStream, MD5.Create (), this.currentEntry.ExtractedChecksum);
					break;
				case XarHashAlgorithm.Sha1:
					dataStream = new HashStream (dataStream, SHA1.Create (), this.currentEntry.ExtractedChecksum);
					break;
				default:
					break;
				}				
			}

			
			return this.currentEntry;
		}
		

		public XarArchiveInputStream (Stream inputStream) : base(inputStream)
		{
			ReadHeader ();
		}
		private void ReadHeader ()
		{
			byte[] header = new byte[XAR_HEADER_SIZE];
			int leidos = this.inputStream.Read (header, 0, header.Length);
			this.Count (leidos);
			if (leidos != header.Length) {
				throw new IOException ("Invalid header, readed " + leidos + " bytes expected " + header.Length);
			}
			int magic = UtArrays.LeerInt32BE (header, 0);
			int size = UtArrays.LeerInt16BE (header, 4);
			int version = UtArrays.LeerInt16BE (header, 6);
			long toc_length_compressed = UtArrays.LeerInt64BE (header, 8);
			long toc_length_uncompressed = UtArrays.LeerInt64BE (header, 16);
			int cksum_alg = UtArrays.LeerInt32BE (header, 24);
			

			this.tocHashAlgorithm = (XarHashAlgorithm)cksum_alg;


			
			Stream s = new SizeLimiterStream (this.inputStream, toc_length_compressed);
			HashStream hs1 = null;
			switch (this.tocHashAlgorithm) {
			case XarHashAlgorithm.Sha1:
				hs1 = new HashStream (s, SHA1.Create ());
				s = hs1;
				break;
			case XarHashAlgorithm.Md5:
				hs1 = new HashStream (s, MD5.Create ());
				s = hs1;
				break;
			default:
				break;
			}
			
			
			Stream st = new InflaterInputStream (
				s,
				new Inflater (false)
			);
		
		


			this.ReadToc (st);
			st.Close ();
			this.Count ((int)toc_length_uncompressed);
			streamLength = this.Position;
			foreach (XarArchiveEntry e in entryList)
			{
				streamLength += e.Size;
			}
			
			
			if (this.tocHashAlgorithm != XarHashAlgorithm.None) 
			{
				byte[] h = new byte[this.tocHashSize];
				int br = this.inputStream.Read (h, 0, h.Length);
				this.heapPosition += br;
				string ss = Dalle.Utilidades.UtilidadesCadenas.FormatHexHash (h).ToLower ();
				if (!ss.Equals(hs1.Hash)){
					throw new IOException("Invalid toc checksum : " + hs1.Hash + " expected " + ss);
				}
			}
			
			
			
		}
		private void ReadToc (Stream st)
		{
			XmlTextReader xmlReader = new XmlTextReader (st);
			ArrayList nodeL = new ArrayList ();
			ArrayList pathList = new ArrayList ();
			XarArchiveEntry ce = null;
			
			while (xmlReader.Read ()) 
			{
				string nodeName = "";
				switch (xmlReader.NodeType)
        		{
				case XmlNodeType.Element:
					
					
					if (!xmlReader.IsEmptyElement)
					{
						nodeL.Add (xmlReader.Name);
					}
					
					nodeName = xmlReader.Name;
					Hashtable attributes = ReadAttributes(xmlReader);
					if (nodeName.Equals ("file")) {
						ce = new XarArchiveEntry ();
						ce.Id = long.Parse((string) attributes["id"]);
						entryList.Add(ce);
					}
					else if (nodeName.Equals("encoding")){
						if ("application/x-gzip".Equals(attributes["style"])){
							ce.Encoding = XarEncoding.Gzip;
						}
						else if ("application/x-bzip2".Equals(attributes["style"])){
							ce.Encoding = XarEncoding.Bzip2;		
						}
						else if("application/octet-stream".Equals(attributes["style"])){
							ce.Encoding = XarEncoding.None;		
						}
						else {
							throw new IOException ("Unsupported file encoding " + attributes["style"]);
						}							
					}
					else if (nodeName.Equals("archived-checksum")){
						if ("SHA1".Equals (attributes["style"])) {
							ce.HashAlgorithmArchived = XarHashAlgorithm.Sha1;
						} else if ("MD5".Equals (attributes["style"])) {
							ce.HashAlgorithmArchived = XarHashAlgorithm.Md5;
						}
						else {
							throw new IOException ("Unsupported archived checksum " + attributes["style"]);
						}						
					}
					else if (nodeName.Equals ("extracted-checksum")) {
						if ("SHA1".Equals (attributes["style"])) {
							ce.HashAlgorithmExtracted = XarHashAlgorithm.Sha1;
						} else if ("MD5".Equals (attributes["style"])) {
							ce.HashAlgorithmExtracted = XarHashAlgorithm.Md5;
						} else {
							throw new IOException ("Unsupported extracted checksum " + attributes["style"]);
						}
					}	
					break;
				case XmlNodeType.EndElement:
					
					nodeL.RemoveAt (nodeL.Count - 1);
					if (xmlReader.Name.Equals("file")){
						pathList.RemoveAt(pathList.Count -1);		
					}
					
					break;
				case XmlNodeType.Text:
					nodeName = (string)nodeL[nodeL.Count-1];
					string val = xmlReader.Value;
					if (nodeName.Equals("length")){
						ce.Length = long.Parse(val);
					}
					else if (nodeName.Equals("offset")){
						if (!"checksum".Equals(nodeL[nodeL.Count-2])){
							ce.Offset = long.Parse(val);		
						}
						else {
							this.tocHashOffset = long.Parse(val);		
						}
					}
					else if (nodeName.Equals ("size")) {
						if (!"checksum".Equals(nodeL[nodeL.Count-2])){
							ce.Size = long.Parse(val);
						}
						else {
							this.tocHashSize = long.Parse (val);		
						}
					}
					else if (nodeName.Equals ("archived-checksum")) {
						ce.ArchivedChecksum = val;
					}
					else if (nodeName.Equals ("extracted-checksum")) {
						ce.ExtractedChecksum = val;
					}
					else if (nodeName.Equals ("group")) {
						ce.Group = val;
					}
					else if (nodeName.Equals ("gid")) {
						ce.Gid = Int32.Parse(val);
					}
					else if (nodeName.Equals ("user")) {
						ce.User = val;
					}
					else if (nodeName.Equals ("uid")) {
						ce.Uid = Int32.Parse(val);
					}
					else if (nodeName.Equals ("")) {
						
					}
					else if (nodeName.Equals ("type")) {
						if ("directory".Equals(val)){
							ce.IsDirectory = true;
						}
						else if ("file".Equals(val) || "script".Equals(val)) {
							ce.IsDirectory = false;
						}						
						else {
							throw new IOException("Unsupported file type " + val);		
						}
					}
					else if (nodeName.Equals ("name")) {
						StringBuilder sb = new StringBuilder();
						foreach (string s in pathList)
						{
							sb.Append(s);
							sb.Append(Path.DirectorySeparatorChar);
						}
						sb.Append(val);
						pathList.Add(val);
						ce.Name = sb.ToString();
					}
					break;
				default:
					
					break;
						
				}
			}
			st.Close();
			
		}
		private Hashtable ReadAttributes (XmlTextReader xmlReader)
		{
			Hashtable attributes = new Hashtable ();
			if (xmlReader.HasAttributes)
            {
                    for (int i = 0; i < xmlReader.AttributeCount; i++)
                    {
                        xmlReader.MoveToAttribute(i);
                        attributes.Add(xmlReader.Name, xmlReader.Value);
                    }
            }
			
			return attributes;
		}
		public override int Read (byte[] buffer, int offset, int count)
		{
			int bytesReaded = base.Read (buffer, offset, count);
			if (bytesReaded > 0) {
				this.heapPosition += bytesReaded;
			}
			return bytesReaded;
		}
		
		
	}
}
/*
#define XAR_CKSUM_NONE   0
#define XAR_CKSUM_SHA1   1
#define XAR_CKSUM_MD5    2
 */
/*
#define XAR_HEADER_MAGIC 0x78617221
#define XAR_HEADER_VERSION 0
#define XAR_HEADER_SIZE sizeof(struct xar_header)

*
 * xar_header version 0
 *
struct xar_header {
    uint32_t magic;     
    uint16_t size;
    uint16_t version;
    uint64_t toc_length_compressed;
    uint64_t toc_length_uncompressed;
    uint32_t cksum_alg;
};
*/