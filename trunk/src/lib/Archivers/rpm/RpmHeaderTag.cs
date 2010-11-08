/*

    Dalle - A split/join file utility library
	
	
	
    Copyright (C) 2004-2010 Alberto Fernández  <infjaf@gmail.com>
    Original java code: commons-commpress, from apache (http://commons.apache.org/compress/)
    C# translation by - Alberto Fernández  <infjaf@gmail.com>

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

namespace Dalle.Archivers.rpm
{


	public enum RpmHeaderTag
	{
		
		RPMTAG_NAME  		= 1000,	/* s */
		RPMTAG_VERSION		= 1001,	/* s */
		RPMTAG_RELEASE		= 1002,	/* s */
		RPMTAG_EPOCH   		= 1003,	/* i */
		RPMTAG_SUMMARY		= 1004,	/* s{} */
		RPMTAG_DESCRIPTION		= 1005,	/* s{} */
		RPMTAG_BUILDTIME		= 1006,	/* i */
		RPMTAG_BUILDHOST		= 1007,	/* s */
		RPMTAG_INSTALLTIME		= 1008,	/* i */
		RPMTAG_SIZE			= 1009,	/* i */
		RPMTAG_DISTRIBUTION		= 1010,	/* s */
		RPMTAG_VENDOR		= 1011,	/* s */
		RPMTAG_GIF			= 1012,	/* x */
		RPMTAG_XPM			= 1013,	/* x */
		RPMTAG_LICENSE		= 1014,	/* s */
		RPMTAG_PACKAGER		= 1015,	/* s */
		RPMTAG_GROUP		= 1016,	/* s{} */
		RPMTAG_CHANGELOG		= 1017, /* s[] internal */
		RPMTAG_SOURCE		= 1018,	/* s[] */
		RPMTAG_PATCH		= 1019,	/* s[] */
		RPMTAG_URL			= 1020,	/* s */
		RPMTAG_OS			= 1021,	/* s legacy used int */
		RPMTAG_ARCH			= 1022,	/* s legacy used int */
		RPMTAG_PREIN		= 1023,	/* s */
		RPMTAG_POSTIN		= 1024,	/* s */
		RPMTAG_PREUN		= 1025,	/* s */
		RPMTAG_POSTUN		= 1026,	/* s */
		RPMTAG_OLDFILENAMES		= 1027, /* s[] obsolete */
		RPMTAG_FILESIZES		= 1028,	/* i[] */
		RPMTAG_FILESTATES		= 1029, /* c[] */
		RPMTAG_FILEMODES		= 1030,	/* h[] */
		RPMTAG_FILEUIDS		= 1031, /* i[] internal */
		RPMTAG_FILEGIDS		= 1032, /* i[] internal */
		RPMTAG_FILERDEVS		= 1033,	/* h[] */
		RPMTAG_FILEMTIMES		= 1034, /* i[] */
		RPMTAG_FILEDIGESTS		= 1035,	/* s[] */
		RPMTAG_FILELINKTOS		= 1036,	/* s[] */
		RPMTAG_FILEFLAGS		= 1037,	/* i[] */
		RPMTAG_ROOT			= 1038, /* internal - obsolete */
		RPMTAG_FILEUSERNAME		= 1039,	/* s[] */
		RPMTAG_FILEGROUPNAME	= 1040,	/* s[] */
		RPMTAG_EXCLUDE		= 1041, /* internal - obsolete */
		RPMTAG_EXCLUSIVE		= 1042, /* internal - obsolete */
		RPMTAG_ICON			= 1043, /* x */
		RPMTAG_SOURCERPM		= 1044,	/* s */
		RPMTAG_FILEVERIFYFLAGS	= 1045,	/* i[] */
		RPMTAG_ARCHIVESIZE		= 1046,	/* i */
		RPMTAG_PROVIDENAME		= 1047,	/* s[] */
		RPMTAG_REQUIREFLAGS		= 1048,	/* i[] */
		RPMTAG_REQUIRENAME		= 1049,	/* s[] */
		RPMTAG_REQUIREVERSION	= 1050,	/* s[] */
		RPMTAG_NOSOURCE		= 1051, /* i internal */
		RPMTAG_NOPATCH		= 1052, /* i internal */
		RPMTAG_CONFLICTFLAGS	= 1053, /* i[] */
		RPMTAG_CONFLICTNAME		= 1054,	/* s[] */
		RPMTAG_CONFLICTVERSION	= 1055,	/* s[] */
		RPMTAG_DEFAULTPREFIX	= 1056, /* s internal - deprecated */
		RPMTAG_BUILDROOT		= 1057, /* s internal */
		RPMTAG_INSTALLPREFIX	= 1058, /* s internal - deprecated */
		RPMTAG_EXCLUDEARCH		= 1059, /* s[] */
		RPMTAG_EXCLUDEOS		= 1060, /* s[] */
		RPMTAG_EXCLUSIVEARCH	= 1061, /* s[] */
		RPMTAG_EXCLUSIVEOS		= 1062, /* s[] */
		RPMTAG_AUTOREQPROV		= 1063, /* s internal */
		RPMTAG_RPMVERSION		= 1064,	/* s */
		RPMTAG_TRIGGERSCRIPTS	= 1065,	/* s[] */
		RPMTAG_TRIGGERNAME		= 1066,	/* s[] */
		RPMTAG_TRIGGERVERSION	= 1067,	/* s[] */
		RPMTAG_TRIGGERFLAGS		= 1068,	/* i[] */
		RPMTAG_TRIGGERINDEX		= 1069,	/* i[] */
		RPMTAG_VERIFYSCRIPT		= 1079,	/* s */
		RPMTAG_CHANGELOGTIME	= 1080,	/* i[] */
		RPMTAG_CHANGELOGNAME	= 1081,	/* s[] */
		RPMTAG_CHANGELOGTEXT	= 1082,	/* s[] */
		RPMTAG_BROKENMD5		= 1083, /* internal - obsolete */
		RPMTAG_PREREQ		= 1084, /* internal */
		RPMTAG_PREINPROG		= 1085,	/* s */
		RPMTAG_POSTINPROG		= 1086,	/* s */
		RPMTAG_PREUNPROG		= 1087,	/* s */
		RPMTAG_POSTUNPROG		= 1088,	/* s */
		RPMTAG_BUILDARCHS		= 1089, /* s[] */
		RPMTAG_OBSOLETENAME		= 1090,	/* s[] */
		RPMTAG_VERIFYSCRIPTPROG	= 1091,	/* s */
		RPMTAG_TRIGGERSCRIPTPROG	= 1092,	/* s[] */
		RPMTAG_DOCDIR		= 1093, /* internal */
		RPMTAG_COOKIE		= 1094,	/* s */
		RPMTAG_FILEDEVICES		= 1095,	/* i[] */
		RPMTAG_FILEINODES		= 1096,	/* i[] */
		RPMTAG_FILELANGS		= 1097,	/* s[] */
		RPMTAG_PREFIXES		= 1098,	/* s[] */
		RPMTAG_INSTPREFIXES		= 1099,	/* s[] */
		RPMTAG_TRIGGERIN		= 1100, /* internal */
		RPMTAG_TRIGGERUN		= 1101, /* internal */
		RPMTAG_TRIGGERPOSTUN	= 1102, /* internal */
		RPMTAG_AUTOREQ		= 1103, /* internal */
		RPMTAG_AUTOPROV		= 1104, /* internal */
		RPMTAG_CAPABILITY		= 1105, /* i legacy - obsolete */
		RPMTAG_SOURCEPACKAGE	= 1106, /* i legacy - obsolete */
		RPMTAG_OLDORIGFILENAMES	= 1107, /* internal - obsolete */
		RPMTAG_BUILDPREREQ		= 1108, /* internal */
		RPMTAG_BUILDREQUIRES	= 1109, /* internal */
		RPMTAG_BUILDCONFLICTS	= 1110, /* internal */
		RPMTAG_BUILDMACROS		= 1111, /* internal - unused */
		RPMTAG_PROVIDEFLAGS		= 1112,	/* i[] */
		RPMTAG_PROVIDEVERSION	= 1113,	/* s[] */
		RPMTAG_OBSOLETEFLAGS	= 1114,	/* i[] */
		RPMTAG_OBSOLETEVERSION	= 1115,	/* s[] */
		RPMTAG_DIRINDEXES		= 1116,	/* i[] */
		RPMTAG_BASENAMES		= 1117,	/* s[] */
		RPMTAG_DIRNAMES		= 1118,	/* s[] */
		RPMTAG_ORIGDIRINDEXES	= 1119, /* i[] relocation */
		RPMTAG_ORIGBASENAMES	= 1120, /* s[] relocation */
		RPMTAG_ORIGDIRNAMES		= 1121, /* s[] relocation */
		RPMTAG_OPTFLAGS		= 1122,	/* s */
		RPMTAG_DISTURL		= 1123,	/* s */
		RPMTAG_PAYLOADFORMAT	= 1124,	/* s */
		RPMTAG_PAYLOADCOMPRESSOR	= 1125,	/* s */
		RPMTAG_PAYLOADFLAGS		= 1126,	/* s */
		RPMTAG_INSTALLCOLOR		= 1127, /* i transaction color when installed */
		RPMTAG_INSTALLTID		= 1128,	/* i */
		RPMTAG_REMOVETID		= 1129,	/* i */
		RPMTAG_SHA1RHN		= 1130, /* internal - obsolete */
		RPMTAG_RHNPLATFORM		= 1131,	/* s deprecated */
		RPMTAG_PLATFORM		= 1132,	/* s */
		RPMTAG_PATCHESNAME		= 1133, /* s[] deprecated placeholder (SuSE) */
		RPMTAG_PATCHESFLAGS		= 1134, /* i[] deprecated placeholder (SuSE) */
		RPMTAG_PATCHESVERSION	= 1135, /* s[] deprecated placeholder (SuSE) */
		RPMTAG_CACHECTIME		= 1136,	/* i internal - obsolete */
		RPMTAG_CACHEPKGPATH		= 1137,	/* s internal - obsolete */
		RPMTAG_CACHEPKGSIZE		= 1138,	/* i internal - obsolete */
		RPMTAG_CACHEPKGMTIME	= 1139,	/* i internal - obsolete */
		RPMTAG_FILECOLORS		= 1140,	/* i[] */
		RPMTAG_FILECLASS		= 1141,	/* i[] */
		RPMTAG_CLASSDICT		= 1142,	/* s[] */
		RPMTAG_FILEDEPENDSX		= 1143,	/* i[] */
		RPMTAG_FILEDEPENDSN		= 1144,	/* i[] */
		RPMTAG_DEPENDSDICT		= 1145,	/* i[] */
		RPMTAG_SOURCEPKGID		= 1146,	/* x */
		RPMTAG_FILECONTEXTS		= 1147,	/* s[] - obsolete */
		RPMTAG_FSCONTEXTS		= 1148,	/* s[] extension */
		RPMTAG_RECONTEXTS		= 1149,	/* s[] extension */
		RPMTAG_POLICIES		= 1150,	/* s[] selinux *.te policy file. */
		RPMTAG_PRETRANS		= 1151,	/* s */
		RPMTAG_POSTTRANS		= 1152,	/* s */
		RPMTAG_PRETRANSPROG		= 1153,	/* s */
		RPMTAG_POSTTRANSPROG	= 1154,	/* s */
		RPMTAG_DISTTAG		= 1155,	/* s */
		RPMTAG_SUGGESTSNAME		= 1156,	/* s[] extension (unimplemented) */
		RPMTAG_SUGGESTSVERSION	= 1157,	/* s[] extension (unimplemented) */
		RPMTAG_SUGGESTSFLAGS	= 1158,	/* i[] extension (unimplemented) */
		RPMTAG_ENHANCESNAME		= 1159,	/* s[] extension placeholder (unimplemented) */
		RPMTAG_ENHANCESVERSION	= 1160,	/* s[] extension placeholder (unimplemented) */
		RPMTAG_ENHANCESFLAGS	= 1161,	/* i[] extension placeholder (unimplemented) */
		RPMTAG_PRIORITY		= 1162, /* i[] extension placeholder (unimplemented) */
		RPMTAG_CVSID		= 1163, /* s (unimplemented) */
		RPMTAG_BLINKPKGID		= 1164, /* s[] (unimplemented) */
		RPMTAG_BLINKHDRID		= 1165, /* s[] (unimplemented) */
		RPMTAG_BLINKNEVRA		= 1166, /* s[] (unimplemented) */
		RPMTAG_FLINKPKGID		= 1167, /* s[] (unimplemented) */
		RPMTAG_FLINKHDRID		= 1168, /* s[] (unimplemented) */
		RPMTAG_FLINKNEVRA		= 1169, /* s[] (unimplemented) */
		RPMTAG_PACKAGEORIGIN	= 1170, /* s (unimplemented) */
		RPMTAG_TRIGGERPREIN		= 1171, /* internal (unimplemented) */
		RPMTAG_BUILDSUGGESTS	= 1172, /* internal (unimplemented) */
		RPMTAG_BUILDENHANCES	= 1173, /* internal (unimplemented) */
		RPMTAG_SCRIPTSTATES		= 1174, /* i[] scriptlet exit codes (unimplemented) */
		RPMTAG_SCRIPTMETRICS	= 1175, /* i[] scriptlet execution times (unimplemented) */
		RPMTAG_BUILDCPUCLOCK	= 1176, /* i (unimplemented) */
		RPMTAG_FILEDIGESTALGOS	= 1177, /* i[] (unimplemented) */
		RPMTAG_VARIANTS		= 1178, /* s[] (unimplemented) */
		RPMTAG_XMAJOR		= 1179, /* i (unimplemented) */
		RPMTAG_XMINOR		= 1180, /* i (unimplemented) */
		RPMTAG_REPOTAG		= 1181,	/* s (unimplemented) */
		RPMTAG_KEYWORDS		= 1182,	/* s[] (unimplemented) */
		RPMTAG_BUILDPLATFORMS	= 1183,	/* s[] (unimplemented) */
		RPMTAG_PACKAGECOLOR		= 1184, /* i (unimplemented) */
		RPMTAG_PACKAGEPREFCOLOR	= 1185, /* i (unimplemented) */
		RPMTAG_XATTRSDICT		= 1186, /* s[] (unimplemented) */
		RPMTAG_FILEXATTRSX		= 1187, /* i[] (unimplemented) */
		RPMTAG_DEPATTRSDICT		= 1188, /* s[] (unimplemented) */
		RPMTAG_CONFLICTATTRSX	= 1189, /* i[] (unimplemented) */
		RPMTAG_OBSOLETEATTRSX	= 1190, /* i[] (unimplemented) */
		RPMTAG_PROVIDEATTRSX	= 1191, /* i[] (unimplemented) */
		RPMTAG_REQUIREATTRSX	= 1192, /* i[] (unimplemented) */
		RPMTAG_BUILDPROVIDES	= 1193, /* internal */
		RPMTAG_BUILDOBSOLETES	= 1194, /* internal */
		RPMTAG_FILENAMES		= 5000, /* s[] extension */
		RPMTAG_FILEPROVIDE		= 5001, /* s[] extension */
		RPMTAG_FILEREQUIRE		= 5002, /* s[] extension */
		RPMTAG_FSNAMES		= 5003, /* s[] extension */
		RPMTAG_FSSIZES		= 5004, /* l[] extension */
		RPMTAG_TRIGGERCONDS		= 5005, /* s[] extension */
		RPMTAG_TRIGGERTYPE		= 5006, /* s[] extension */
		RPMTAG_ORIGFILENAMES	= 5007, /* s[] extension */
		RPMTAG_LONGFILESIZES	= 5008,	/* l[] */
		RPMTAG_LONGSIZE		= 5009, /* l */
		RPMTAG_FILECAPS		= 5010, /* s[] */
		RPMTAG_FILEDIGESTALGO	= 5011, /* i file digest algorithm */
	}
	
}
