using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using System.Web;
using MongoDB.Bson;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Dynamic;
using System.Reflection;
using System.CodeDom.Compiler;

namespace NCore
{
    public static class Tools
    {
        //public static MvcHtmlString Obj2HtmlStr(object o)
        //{
        //    return MvcHtmlString.Create(Newtonsoft.Json.JsonConvert.SerializeObject(o));
        //}

        static Tools()
        {
            if (License.Validate("Global") == false) throw new Exception("ERR_INVALID_LICENSE");
        }

        private static char[] delims = null;
        private static Dictionary<string, string> _ctMappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {
        #region Big freaking list of mime types
        {".323", "text/h323"},
        {".3g2", "video/3gpp2"},
        {".3gp", "video/3gpp"},
        {".3gp2", "video/3gpp2"},
        {".3gpp", "video/3gpp"},
        {".7z", "application/x-7z-compressed"},
        {".aa", "audio/audible"},
        {".AAC", "audio/aac"},
        {".aaf", "application/octet-stream"},
        {".aax", "audio/vnd.audible.aax"},
        {".ac3", "audio/ac3"},
        {".aca", "application/octet-stream"},
        {".accda", "application/msaccess.addin"},
        {".accdb", "application/msaccess"},
        {".accdc", "application/msaccess.cab"},
        {".accde", "application/msaccess"},
        {".accdr", "application/msaccess.runtime"},
        {".accdt", "application/msaccess"},
        {".accdw", "application/msaccess.webapplication"},
        {".accft", "application/msaccess.ftemplate"},
        {".acx", "application/internet-property-stream"},
        {".AddIn", "text/xml"},
        {".ade", "application/msaccess"},
        {".adobebridge", "application/x-bridge-url"},
        {".adp", "application/msaccess"},
        {".ADT", "audio/vnd.dlna.adts"},
        {".ADTS", "audio/aac"},
        {".afm", "application/octet-stream"},
        {".ai", "application/postscript"},
        {".aif", "audio/x-aiff"},
        {".aifc", "audio/aiff"},
        {".aiff", "audio/aiff"},
        {".air", "application/vnd.adobe.air-application-installer-package+zip"},
        {".amc", "application/x-mpeg"},
        {".application", "application/x-ms-application"},
        {".art", "image/x-jg"},
        {".asa", "application/xml"},
        {".asax", "application/xml"},
        {".ascx", "application/xml"},
        {".asd", "application/octet-stream"},
        {".asf", "video/x-ms-asf"},
        {".ashx", "application/xml"},
        {".asi", "application/octet-stream"},
        {".asm", "text/plain"},
        {".asmx", "application/xml"},
        {".aspx", "application/xml"},
        {".asr", "video/x-ms-asf"},
        {".asx", "video/x-ms-asf"},
        {".atom", "application/atom+xml"},
        {".au", "audio/basic"},
        {".avi", "video/x-msvideo"},
        {".axs", "application/olescript"},
        {".bas", "text/plain"},
        {".bcpio", "application/x-bcpio"},
        {".bin", "application/octet-stream"},
        {".bmp", "image/bmp"},
        {".c", "text/plain"},
        {".cab", "application/octet-stream"},
        {".caf", "audio/x-caf"},
        {".calx", "application/vnd.ms-office.calx"},
        {".cat", "application/vnd.ms-pki.seccat"},
        {".cc", "text/plain"},
        {".cd", "text/plain"},
        {".cdda", "audio/aiff"},
        {".cdf", "application/x-cdf"},
        {".cer", "application/x-x509-ca-cert"},
        {".chm", "application/octet-stream"},
        {".class", "application/x-java-applet"},
        {".clp", "application/x-msclip"},
        {".cmx", "image/x-cmx"},
        {".cnf", "text/plain"},
        {".cod", "image/cis-cod"},
        {".config", "application/xml"},
        {".contact", "text/x-ms-contact"},
        {".coverage", "application/xml"},
        {".cpio", "application/x-cpio"},
        {".cpp", "text/plain"},
        {".crd", "application/x-mscardfile"},
        {".crl", "application/pkix-crl"},
        {".crt", "application/x-x509-ca-cert"},
        {".cs", "text/plain"},
        {".csdproj", "text/plain"},
        {".csh", "application/x-csh"},
        {".csproj", "text/plain"},
        {".css", "text/css"},
        {".csv", "text/csv"},
        {".cur", "application/octet-stream"},
        {".cxx", "text/plain"},
        {".dat", "application/octet-stream"},
        {".datasource", "application/xml"},
        {".dbproj", "text/plain"},
        {".dcr", "application/x-director"},
        {".def", "text/plain"},
        {".deploy", "application/octet-stream"},
        {".der", "application/x-x509-ca-cert"},
        {".dgml", "application/xml"},
        {".dib", "image/bmp"},
        {".dif", "video/x-dv"},
        {".dir", "application/x-director"},
        {".disco", "text/xml"},
        {".dll", "application/x-msdownload"},
        {".dll.config", "text/xml"},
        {".dlm", "text/dlm"},
        {".doc", "application/msword"},
        {".docm", "application/vnd.ms-word.document.macroEnabled.12"},
        {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
        {".dot", "application/msword"},
        {".dotm", "application/vnd.ms-word.template.macroEnabled.12"},
        {".dotx", "application/vnd.openxmlformats-officedocument.wordprocessingml.template"},
        {".dsp", "application/octet-stream"},
        {".dsw", "text/plain"},
        {".dtd", "text/xml"},
        {".dtsConfig", "text/xml"},
        {".dv", "video/x-dv"},
        {".dvi", "application/x-dvi"},
        {".dwf", "drawing/x-dwf"},
        {".dwp", "application/octet-stream"},
        {".dxr", "application/x-director"},
        {".eml", "message/rfc822"},
        {".emz", "application/octet-stream"},
        {".eot", "application/octet-stream"},
        {".eps", "application/postscript"},
        {".etl", "application/etl"},
        {".etx", "text/x-setext"},
        {".evy", "application/envoy"},
        {".exe", "application/octet-stream"},
        {".exe.config", "text/xml"},
        {".fdf", "application/vnd.fdf"},
        {".fif", "application/fractals"},
        {".filters", "Application/xml"},
        {".fla", "application/octet-stream"},
        {".flr", "x-world/x-vrml"},
        {".flv", "video/x-flv"},
        {".fsscript", "application/fsharp-script"},
        {".fsx", "application/fsharp-script"},
        {".generictest", "application/xml"},
        {".gif", "image/gif"},
        {".group", "text/x-ms-group"},
        {".gsm", "audio/x-gsm"},
        {".gtar", "application/x-gtar"},
        {".gz", "application/x-gzip"},
        {".h", "text/plain"},
        {".hdf", "application/x-hdf"},
        {".hdml", "text/x-hdml"},
        {".hhc", "application/x-oleobject"},
        {".hhk", "application/octet-stream"},
        {".hhp", "application/octet-stream"},
        {".hlp", "application/winhlp"},
        {".hpp", "text/plain"},
        {".hqx", "application/mac-binhex40"},
        {".hta", "application/hta"},
        {".htc", "text/x-component"},
        {".htm", "text/html"},
        {".html", "text/html"},
        {".htt", "text/webviewhtml"},
        {".hxa", "application/xml"},
        {".hxc", "application/xml"},
        {".hxd", "application/octet-stream"},
        {".hxe", "application/xml"},
        {".hxf", "application/xml"},
        {".hxh", "application/octet-stream"},
        {".hxi", "application/octet-stream"},
        {".hxk", "application/xml"},
        {".hxq", "application/octet-stream"},
        {".hxr", "application/octet-stream"},
        {".hxs", "application/octet-stream"},
        {".hxt", "text/html"},
        {".hxv", "application/xml"},
        {".hxw", "application/octet-stream"},
        {".hxx", "text/plain"},
        {".i", "text/plain"},
        {".ico", "image/x-icon"},
        {".ics", "application/octet-stream"},
        {".idl", "text/plain"},
        {".ief", "image/ief"},
        {".iii", "application/x-iphone"},
        {".inc", "text/plain"},
        {".inf", "application/octet-stream"},
        {".inl", "text/plain"},
        {".ins", "application/x-internet-signup"},
        {".ipa", "application/x-itunes-ipa"},
        {".ipg", "application/x-itunes-ipg"},
        {".ipproj", "text/plain"},
        {".ipsw", "application/x-itunes-ipsw"},
        {".iqy", "text/x-ms-iqy"},
        {".isp", "application/x-internet-signup"},
        {".ite", "application/x-itunes-ite"},
        {".itlp", "application/x-itunes-itlp"},
        {".itms", "application/x-itunes-itms"},
        {".itpc", "application/x-itunes-itpc"},
        {".IVF", "video/x-ivf"},
        {".jar", "application/java-archive"},
        {".java", "application/octet-stream"},
        {".jck", "application/liquidmotion"},
        {".jcz", "application/liquidmotion"},
        {".jfif", "image/pjpeg"},
        {".jnlp", "application/x-java-jnlp-file"},
        {".jpb", "application/octet-stream"},
        {".jpe", "image/jpeg"},
        {".jpeg", "image/jpeg"},
        {".jpg", "image/jpeg"},
        {".js", "application/x-javascript"},
        {".jsx", "text/jscript"},
        {".jsxbin", "text/plain"},
        {".latex", "application/x-latex"},
        {".library-ms", "application/windows-library+xml"},
        {".lit", "application/x-ms-reader"},
        {".loadtest", "application/xml"},
        {".lpk", "application/octet-stream"},
        {".lsf", "video/x-la-asf"},
        {".lst", "text/plain"},
        {".lsx", "video/x-la-asf"},
        {".lzh", "application/octet-stream"},
        {".m13", "application/x-msmediaview"},
        {".m14", "application/x-msmediaview"},
        {".m1v", "video/mpeg"},
        {".m2t", "video/vnd.dlna.mpeg-tts"},
        {".m2ts", "video/vnd.dlna.mpeg-tts"},
        {".m2v", "video/mpeg"},
        {".m3u", "audio/x-mpegurl"},
        {".m3u8", "audio/x-mpegurl"},
        {".m4a", "audio/m4a"},
        {".m4b", "audio/m4b"},
        {".m4p", "audio/m4p"},
        {".m4r", "audio/x-m4r"},
        {".m4v", "video/x-m4v"},
        {".mac", "image/x-macpaint"},
        {".mak", "text/plain"},
        {".man", "application/x-troff-man"},
        {".manifest", "application/x-ms-manifest"},
        {".map", "text/plain"},
        {".master", "application/xml"},
        {".mda", "application/msaccess"},
        {".mdb", "application/x-msaccess"},
        {".mde", "application/msaccess"},
        {".mdp", "application/octet-stream"},
        {".me", "application/x-troff-me"},
        {".mfp", "application/x-shockwave-flash"},
        {".mht", "message/rfc822"},
        {".mhtml", "message/rfc822"},
        {".mid", "audio/mid"},
        {".midi", "audio/mid"},
        {".mix", "application/octet-stream"},
        {".mk", "text/plain"},
        {".mmf", "application/x-smaf"},
        {".mno", "text/xml"},
        {".mny", "application/x-msmoney"},
        {".mod", "video/mpeg"},
        {".mov", "video/quicktime"},
        {".movie", "video/x-sgi-movie"},
        {".mp2", "video/mpeg"},
        {".mp2v", "video/mpeg"},
        {".mp3", "audio/mpeg"},
        {".mp4", "video/mp4"},
        {".mp4v", "video/mp4"},
        {".mpa", "video/mpeg"},
        {".mpe", "video/mpeg"},
        {".mpeg", "video/mpeg"},
        {".mpf", "application/vnd.ms-mediapackage"},
        {".mpg", "video/mpeg"},
        {".mpp", "application/vnd.ms-project"},
        {".mpv2", "video/mpeg"},
        {".mqv", "video/quicktime"},
        {".ms", "application/x-troff-ms"},
        {".msi", "application/octet-stream"},
        {".mso", "application/octet-stream"},
        {".mts", "video/vnd.dlna.mpeg-tts"},
        {".mtx", "application/xml"},
        {".mvb", "application/x-msmediaview"},
        {".mvc", "application/x-miva-compiled"},
        {".mxp", "application/x-mmxp"},
        {".nc", "application/x-netcdf"},
        {".nsc", "video/x-ms-asf"},
        {".nws", "message/rfc822"},
        {".ocx", "application/octet-stream"},
        {".oda", "application/oda"},
        {".odc", "text/x-ms-odc"},
        {".odh", "text/plain"},
        {".odl", "text/plain"},
        {".odp", "application/vnd.oasis.opendocument.presentation"},
        {".ods", "application/oleobject"},
        {".odt", "application/vnd.oasis.opendocument.text"},
        {".one", "application/onenote"},
        {".onea", "application/onenote"},
        {".onepkg", "application/onenote"},
        {".onetmp", "application/onenote"},
        {".onetoc", "application/onenote"},
        {".onetoc2", "application/onenote"},
        {".orderedtest", "application/xml"},
        {".osdx", "application/opensearchdescription+xml"},
        {".p10", "application/pkcs10"},
        {".p12", "application/x-pkcs12"},
        {".p7b", "application/x-pkcs7-certificates"},
        {".p7c", "application/pkcs7-mime"},
        {".p7m", "application/pkcs7-mime"},
        {".p7r", "application/x-pkcs7-certreqresp"},
        {".p7s", "application/pkcs7-signature"},
        {".pbm", "image/x-portable-bitmap"},
        {".pcast", "application/x-podcast"},
        {".pct", "image/pict"},
        {".pcx", "application/octet-stream"},
        {".pcz", "application/octet-stream"},
        {".pdf", "application/pdf"},
        {".pfb", "application/octet-stream"},
        {".pfm", "application/octet-stream"},
        {".pfx", "application/x-pkcs12"},
        {".pgm", "image/x-portable-graymap"},
        {".pic", "image/pict"},
        {".pict", "image/pict"},
        {".pkgdef", "text/plain"},
        {".pkgundef", "text/plain"},
        {".pko", "application/vnd.ms-pki.pko"},
        {".pls", "audio/scpls"},
        {".pma", "application/x-perfmon"},
        {".pmc", "application/x-perfmon"},
        {".pml", "application/x-perfmon"},
        {".pmr", "application/x-perfmon"},
        {".pmw", "application/x-perfmon"},
        {".png", "image/png"},
        {".pnm", "image/x-portable-anymap"},
        {".pnt", "image/x-macpaint"},
        {".pntg", "image/x-macpaint"},
        {".pnz", "image/png"},
        {".pot", "application/vnd.ms-powerpoint"},
        {".potm", "application/vnd.ms-powerpoint.template.macroEnabled.12"},
        {".potx", "application/vnd.openxmlformats-officedocument.presentationml.template"},
        {".ppa", "application/vnd.ms-powerpoint"},
        {".ppam", "application/vnd.ms-powerpoint.addin.macroEnabled.12"},
        {".ppm", "image/x-portable-pixmap"},
        {".pps", "application/vnd.ms-powerpoint"},
        {".ppsm", "application/vnd.ms-powerpoint.slideshow.macroEnabled.12"},
        {".ppsx", "application/vnd.openxmlformats-officedocument.presentationml.slideshow"},
        {".ppt", "application/vnd.ms-powerpoint"},
        {".pptm", "application/vnd.ms-powerpoint.presentation.macroEnabled.12"},
        {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
        {".prf", "application/pics-rules"},
        {".prm", "application/octet-stream"},
        {".prx", "application/octet-stream"},
        {".ps", "application/postscript"},
        {".psc1", "application/PowerShell"},
        {".psd", "application/octet-stream"},
        {".psess", "application/xml"},
        {".psm", "application/octet-stream"},
        {".psp", "application/octet-stream"},
        {".pub", "application/x-mspublisher"},
        {".pwz", "application/vnd.ms-powerpoint"},
        {".qht", "text/x-html-insertion"},
        {".qhtm", "text/x-html-insertion"},
        {".qt", "video/quicktime"},
        {".qti", "image/x-quicktime"},
        {".qtif", "image/x-quicktime"},
        {".qtl", "application/x-quicktimeplayer"},
        {".qxd", "application/octet-stream"},
        {".ra", "audio/x-pn-realaudio"},
        {".ram", "audio/x-pn-realaudio"},
        {".rar", "application/octet-stream"},
        {".ras", "image/x-cmu-raster"},
        {".rat", "application/rat-file"},
        {".rc", "text/plain"},
        {".rc2", "text/plain"},
        {".rct", "text/plain"},
        {".rdlc", "application/xml"},
        {".resx", "application/xml"},
        {".rf", "image/vnd.rn-realflash"},
        {".rgb", "image/x-rgb"},
        {".rgs", "text/plain"},
        {".rm", "application/vnd.rn-realmedia"},
        {".rmi", "audio/mid"},
        {".rmp", "application/vnd.rn-rn_music_package"},
        {".roff", "application/x-troff"},
        {".rpm", "audio/x-pn-realaudio-plugin"},
        {".rqy", "text/x-ms-rqy"},
        {".rtf", "application/rtf"},
        {".rtx", "text/richtext"},
        {".ruleset", "application/xml"},
        {".s", "text/plain"},
        {".safariextz", "application/x-safari-safariextz"},
        {".scd", "application/x-msschedule"},
        {".sct", "text/scriptlet"},
        {".sd2", "audio/x-sd2"},
        {".sdp", "application/sdp"},
        {".sea", "application/octet-stream"},
        {".searchConnector-ms", "application/windows-search-connector+xml"},
        {".setpay", "application/set-payment-initiation"},
        {".setreg", "application/set-registration-initiation"},
        {".settings", "application/xml"},
        {".sgimb", "application/x-sgimb"},
        {".sgml", "text/sgml"},
        {".sh", "application/x-sh"},
        {".shar", "application/x-shar"},
        {".shtml", "text/html"},
        {".sit", "application/x-stuffit"},
        {".sitemap", "application/xml"},
        {".skin", "application/xml"},
        {".sldm", "application/vnd.ms-powerpoint.slide.macroEnabled.12"},
        {".sldx", "application/vnd.openxmlformats-officedocument.presentationml.slide"},
        {".slk", "application/vnd.ms-excel"},
        {".sln", "text/plain"},
        {".slupkg-ms", "application/x-ms-license"},
        {".smd", "audio/x-smd"},
        {".smi", "application/octet-stream"},
        {".smx", "audio/x-smd"},
        {".smz", "audio/x-smd"},
        {".snd", "audio/basic"},
        {".snippet", "application/xml"},
        {".snp", "application/octet-stream"},
        {".sol", "text/plain"},
        {".sor", "text/plain"},
        {".spc", "application/x-pkcs7-certificates"},
        {".spl", "application/futuresplash"},
        {".src", "application/x-wais-source"},
        {".srf", "text/plain"},
        {".SSISDeploymentManifest", "text/xml"},
        {".ssm", "application/streamingmedia"},
        {".sst", "application/vnd.ms-pki.certstore"},
        {".stl", "application/vnd.ms-pki.stl"},
        {".sv4cpio", "application/x-sv4cpio"},
        {".sv4crc", "application/x-sv4crc"},
        {".svc", "application/xml"},
        {".swf", "application/x-shockwave-flash"},
        {".t", "application/x-troff"},
        {".tar", "application/x-tar"},
        {".tcl", "application/x-tcl"},
        {".testrunconfig", "application/xml"},
        {".testsettings", "application/xml"},
        {".tex", "application/x-tex"},
        {".texi", "application/x-texinfo"},
        {".texinfo", "application/x-texinfo"},
        {".tgz", "application/x-compressed"},
        {".thmx", "application/vnd.ms-officetheme"},
        {".thn", "application/octet-stream"},
        {".tif", "image/tiff"},
        {".tiff", "image/tiff"},
        {".tlh", "text/plain"},
        {".tli", "text/plain"},
        {".toc", "application/octet-stream"},
        {".tr", "application/x-troff"},
        {".trm", "application/x-msterminal"},
        {".trx", "application/xml"},
        {".ts", "video/vnd.dlna.mpeg-tts"},
        {".tsv", "text/tab-separated-values"},
        {".ttf", "application/octet-stream"},
        {".tts", "video/vnd.dlna.mpeg-tts"},
        {".txt", "text/plain"},
        {".u32", "application/octet-stream"},
        {".uls", "text/iuls"},
        {".user", "text/plain"},
        {".ustar", "application/x-ustar"},
        {".vb", "text/plain"},
        {".vbdproj", "text/plain"},
        {".vbk", "video/mpeg"},
        {".vbproj", "text/plain"},
        {".vbs", "text/vbscript"},
        {".vcf", "text/x-vcard"},
        {".vcproj", "Application/xml"},
        {".vcs", "text/plain"},
        {".vcxproj", "Application/xml"},
        {".vddproj", "text/plain"},
        {".vdp", "text/plain"},
        {".vdproj", "text/plain"},
        {".vdx", "application/vnd.ms-visio.viewer"},
        {".vml", "text/xml"},
        {".vscontent", "application/xml"},
        {".vsct", "text/xml"},
        {".vsd", "application/vnd.visio"},
        {".vsi", "application/ms-vsi"},
        {".vsix", "application/vsix"},
        {".vsixlangpack", "text/xml"},
        {".vsixmanifest", "text/xml"},
        {".vsmdi", "application/xml"},
        {".vspscc", "text/plain"},
        {".vss", "application/vnd.visio"},
        {".vsscc", "text/plain"},
        {".vssettings", "text/xml"},
        {".vssscc", "text/plain"},
        {".vst", "application/vnd.visio"},
        {".vstemplate", "text/xml"},
        {".vsto", "application/x-ms-vsto"},
        {".vsw", "application/vnd.visio"},
        {".vsx", "application/vnd.visio"},
        {".vtx", "application/vnd.visio"},
        {".wav", "audio/wav"},
        {".wave", "audio/wav"},
        {".wax", "audio/x-ms-wax"},
        {".wbk", "application/msword"},
        {".wbmp", "image/vnd.wap.wbmp"},
        {".wcm", "application/vnd.ms-works"},
        {".wdb", "application/vnd.ms-works"},
        {".wdp", "image/vnd.ms-photo"},
        {".webarchive", "application/x-safari-webarchive"},
        {".webtest", "application/xml"},
        {".wiq", "application/xml"},
        {".wiz", "application/msword"},
        {".wks", "application/vnd.ms-works"},
        {".WLMP", "application/wlmoviemaker"},
        {".wlpginstall", "application/x-wlpg-detect"},
        {".wlpginstall3", "application/x-wlpg3-detect"},
        {".wm", "video/x-ms-wm"},
        {".wma", "audio/x-ms-wma"},
        {".wmd", "application/x-ms-wmd"},
        {".wmf", "application/x-msmetafile"},
        {".wml", "text/vnd.wap.wml"},
        {".wmlc", "application/vnd.wap.wmlc"},
        {".wmls", "text/vnd.wap.wmlscript"},
        {".wmlsc", "application/vnd.wap.wmlscriptc"},
        {".wmp", "video/x-ms-wmp"},
        {".wmv", "video/x-ms-wmv"},
        {".wmx", "video/x-ms-wmx"},
        {".wmz", "application/x-ms-wmz"},
        {".wpl", "application/vnd.ms-wpl"},
        {".wps", "application/vnd.ms-works"},
        {".wri", "application/x-mswrite"},
        {".wrl", "x-world/x-vrml"},
        {".wrz", "x-world/x-vrml"},
        {".wsc", "text/scriptlet"},
        {".wsdl", "text/xml"},
        {".wvx", "video/x-ms-wvx"},
        {".x", "application/directx"},
        {".xaf", "x-world/x-vrml"},
        {".xaml", "application/xaml+xml"},
        {".xap", "application/x-silverlight-app"},
        {".xbap", "application/x-ms-xbap"},
        {".xbm", "image/x-xbitmap"},
        {".xdr", "text/plain"},
        {".xht", "application/xhtml+xml"},
        {".xhtml", "application/xhtml+xml"},
        {".xla", "application/vnd.ms-excel"},
        {".xlam", "application/vnd.ms-excel.addin.macroEnabled.12"},
        {".xlc", "application/vnd.ms-excel"},
        {".xld", "application/vnd.ms-excel"},
        {".xlk", "application/vnd.ms-excel"},
        {".xll", "application/vnd.ms-excel"},
        {".xlm", "application/vnd.ms-excel"},
        {".xls", "application/vnd.ms-excel"},
        {".xlsb", "application/vnd.ms-excel.sheet.binary.macroEnabled.12"},
        {".xlsm", "application/vnd.ms-excel.sheet.macroEnabled.12"},
        {".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"},
        {".xlt", "application/vnd.ms-excel"},
        {".xltm", "application/vnd.ms-excel.template.macroEnabled.12"},
        {".xltx", "application/vnd.openxmlformats-officedocument.spreadsheetml.template"},
        {".xlw", "application/vnd.ms-excel"},
        {".xml", "text/xml"},
        {".xmta", "application/xml"},
        {".xof", "x-world/x-vrml"},
        {".XOML", "text/plain"},
        {".xpm", "image/x-xpixmap"},
        {".xps", "application/vnd.ms-xpsdocument"},
        {".xrm-ms", "text/xml"},
        {".xsc", "application/xml"},
        {".xsd", "text/xml"},
        {".xsf", "text/xml"},
        {".xsl", "text/xml"},
        {".xslt", "text/xml"},
        {".xsn", "application/octet-stream"},
        {".xss", "application/xml"},
        {".xtp", "application/octet-stream"},
        {".xwd", "image/x-xwindowdump"},
        {".z", "application/x-compress"},
        {".zip", "application/x-zip-compressed"},
        #endregion
        };

        //public static String GetSession(string sessionname, string defaultValue = "")
        //{
        //    if (System.Web.HttpContext.Current.Session[sessionname] == null)
        //    {
        //        return defaultValue.ToString();
        //    }
        //    else
        //    {
        //        return System.Web.HttpContext.Current.Session[sessionname].ToString();
        //    }
        //}

        //public static string GetQuery(string sessionname, string defaultvalue = "")
        //{
        //    if (System.Web.HttpContext.Current.Request[sessionname] == null)
        //    {
        //        return defaultvalue.ToString();
        //    }
        //    else
        //    {
        //        return System.Web.HttpContext.Current.Request[sessionname].ToString();
        //    }
        //}

        public static ResultInfo SendMail(string config, string subject, string body,
            string[] toMails = null, string[] ccMails = null, string[] attachmentFiles = null,
            //bool sendAsHtml = false, 
            Func<string, string> fnTranslate = null,
            bool sendAsHtml = true)
        {
            var fromMail = ConfigurationManager.AppSettings["SMTP_From_" + config];
            var hostName = ConfigurationManager.AppSettings["SMTP_Host_" + config];
            var port = Tools.ToInt32(ConfigurationManager.AppSettings["SMTP_Port_" + config]);
            var userName = ConfigurationManager.AppSettings["SMTP_UserName_" + config];
            var password = ConfigurationManager.AppSettings["SMTP_Password_" + config];
            var useSSL = ConfigurationManager.AppSettings["SMTP_UseSSL_" + config].ToLower() == "true";
            return SendMail(fromMail, subject, body, hostName, port, userName, password, useSSL, toMails, ccMails, attachmentFiles,
              //sendAsHtml, 
              fnTranslate, sendAsHtml);
        }

        public static ResultInfo SendMail(string fromMail, string subject, string body,
            string host, int port,
            string userName, string password,
            bool useSSL = false,
            string[] toMails = null, string[] ccMails = null, string[] attachmentFiles = null,
            Func<string, string> fnTranslate = null,
            bool sendAsHtml = false
            )
        {
            return ResultInfo.Execute(() => {
                var fromAddress = new System.Net.Mail.MailAddress(fromMail, fromMail);
                //var msg = new System.Net.Mail.MailMessage(fromAddress,fromAddress);
                var msg = new MailMessage();
                msg.From = fromAddress;
                if (toMails != null)
                    foreach (var toAddress in toMails)
                    {
                        var ma = new MailAddress(toAddress);
                        msg.To.Add(ma);
                    }

                if (ccMails != null)
                    foreach (var cc in ccMails)
                    {
                        var ma = new MailAddress(cc);
                        msg.CC.Add(ma);
                    }

                if (fnTranslate != null)
                {
                    subject = fnTranslate(subject);
                    body = fnTranslate(body);
                }
                msg.Subject = subject;
                msg.Body = body;
                if (sendAsHtml) msg.IsBodyHtml = true;
                if (attachmentFiles != null)
                    foreach (var attch in attachmentFiles)
                    {
                        Attachment attFile = new Attachment(attch);
                        msg.Attachments.Add(attFile);
                    }
                System.Net.Mail.SmtpClient client = null;
                try
                {
                    using (client = new System.Net.Mail.SmtpClient(host, port))
                    {
                        client.ServicePoint.MaxIdleTime = 1;
                        client.ServicePoint.ConnectionLimit = 1;
                        //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        if (useSSL) client.EnableSsl = true;
                        if (String.IsNullOrEmpty(userName) == false)
                        {
                            client.UseDefaultCredentials = false;
                            //var credCache = new System.Net.CredentialCache();
                            //credCache.Add(host,port,"",new System.Net.NetworkCredential(userName, password));
                            //client.Credentials = credCache;
                            var cred = new System.Net.NetworkCredential(userName, password);
                            client.Credentials = cred;
                        }
                        client.Send(msg);
                        msg.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    msg.Dispose();
                    throw ex;
                }
                return "OK";
            });
        }

        //public static string GetContentType(string ext)
        //{
        //    string ct = "";
        //    if (ext.StartsWith(".") == false)
        //    {
        //        ext = "." + ext;
        //    }
        //    ext = ext.ToLower();
        //    if (_ctMappings.Keys..Contains(ext))
        //    {
        //        ct = _ctMappings[ext];
        //    }
        //    return ct;
        //}

        //public static char[] SplitDelimeter
        //{
        //    get
        //    {
        //        if (delims == null)
        //        {
        //            if (ConfigurationManager.AppSettings.AllKeys.Equals("Delimeters") != null)
        //            {
        //                delims = ConfigurationManager.AppSettings["Delimeters"]..ToArray();
        //            }
        //        }
        //        if (delims == null) delims = new char[] { '|' };
        //        return delims;
        //    }

        //    set
        //    {
        //        delims = value;
        //    }
        //}

        public static bool IsBool(object val)
        {
            bool ret = false;
            try
            {
                Convert.ToBoolean(val);
            }
            catch (Exception e)
            {
                ret = false;
            }
            return ret;
        }

        public static bool IsNumber(object val)
        {
            bool ret = true;
            try
            {
                Convert.ToDouble(val);
            }
            catch (Exception e)
            {
                ret = false;
            }
            return ret;
        }

        public static bool IsDate(object val)
        {
            bool ret = true;
            try
            {
                Convert.ToDateTime(val);
            }
            catch (Exception e)
            {
                ret = false;
            }
            return ret;
        }

        private static Random _random;

        public static int GetRandomSeed(int limit = 0)
        {
            if (_random == null) _random = new Random();
            return limit == 0 ? _random.Next() : _random.Next(limit);
        }

        public static string GenerateRandomString(int length, bool onlynumber = false, string format = "{0}")
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random(GetRandomSeed(10000));
            var result = new string(
                Enumerable.Repeat(chars, length)
                          .Select(s => onlynumber == false ?
                                s[random.Next(s.Length)] :
                                s[random.Next(10) + s.Length - 10]
                              )
                          .ToArray());
            return String.Format(format, result);
        }

        public static string GetFirstLeft(string source, int leftchar)
        {
            if (source == null) source = "";
            string ret = source;
            if (source.Length >= leftchar)
            {
                ret = source.Substring(0, leftchar);
            }
            return ret;
        }

        public static DateTime GetFirstDateOfMonth(DateTime d)
        {
            return Tools.ToUTC(new DateTime(d.Year, d.Month, 1));
        }

        public static DateTime GetEndDateOfMonth(DateTime d, bool DateOnly = false)
        {
            return Tools.ToDateTime(
                GetFirstDateOfMonth(d).AddMonths(1).AddMilliseconds(-1).ToString(),
                DateOnly);
        }

        public static string ReadFlatFile(string file, bool isHDFS = false)
        {
            StreamReader sr = new StreamReader(file);
            string srOut = sr.ReadToEnd();
            sr.Close();
            return srOut;
        }

        public static ResultInfo RunCommand(string command)
        {
            ResultInfo ri = new ResultInfo();
            try
            {
                ProcessStartInfo p = new ProcessStartInfo("cmd.exe", "/c " + command);
                p.WindowStyle = ProcessWindowStyle.Hidden;
                p.UseShellExecute = false;
                p.RedirectStandardOutput = true;
                p.RedirectStandardError = true;

                var process = new Process();
                process.StartInfo = p;
                process.Start();
                ri.Data = process.StandardOutput.ReadToEnd();
            }
            catch (Exception e)
            {
                ri.PushException(e);
            }
            ri.CalcLapseTime();
            return ri;
        }

        public static object Coelesce(object[] objects)
        {
            return objects.FirstOrDefault(d => d != null);
        }

        public static ExpandoObject DictToDynamic(Dictionary<string, object> source)
        {
            var eo = new ExpandoObject();
            var eoColl = (ICollection<KeyValuePair<string, object>>)eo;
            foreach (var kvp in source)
            {
                eoColl.Add(kvp);
            }
            dynamic eoDynamic = eo;
            return eoDynamic;
        }

        //public static JsonResult ToJson(object data, JsonRequestBehavior jsonRequestBehavior = JsonRequestBehavior.AllowGet)
        //{
        //    return new JsonResult()
        //    {
        //        Data = data,
        //        JsonRequestBehavior = jsonRequestBehavior,
        //        MaxJsonLength = Int32.MaxValue
        //    };
        //}

        public static string ToJson(object data)
        {
            return JsonConvert.SerializeObject(data);
        }

        public static string PushException(Exception ex, bool includeTrace = true)
        {
            string Message = "";
            string Trace = "";
            if (ex == null)
            {
                Message = "";
                Trace = "";
            }
            else
            {
                if (ex.InnerException == null)
                {
                    Message = ex.Message;
                    Trace = ex.StackTrace;
                }
                else
                {
                    PushException(ex.InnerException);
                }
            }
            string ret = Message;
            if (includeTrace) ret += " " + Trace;
            return ret;
        }
        public static T GetHighest<T>(object[] objs)
        {
            return objs.Max(d => (T)d);
        }

        public static T GetLowest<T>(object[] objs)
        {
            return objs.Min(d => (T)d);
        }

        public static DateTime ToUTC(DateTime d)
        {
            return new DateTime(d.Ticks, DateTimeKind.Utc);
        }

        public static DateTime ToUTC(DateTime d, bool dateOnly)
        {
            var dt = new DateTime(d.Ticks, DateTimeKind.Utc);
            if (dateOnly) dt = dt.Date;
            return dt;
        }

        public static DateTime? ToUTC(DateTime? d, DateTime? def = null)
        {
            if (def == null) def = (DateTime?)DateTime.Now;
            DateTime d1 = (DateTime)(d == null ? DateTime.Now : d);
            return (DateTime?)new DateTime(d1.Ticks, DateTimeKind.Utc);
        }


        public static Decimal Div(Decimal d0, Decimal d1, Decimal def = 0, Decimal max = 0)
        {
            var ret = d1 == 0 ? def : d0 / d1;
            if (max != 0 && ret > max) ret = max;
            return ret;
        }


        public static Double Div(Double d0, Double d1, Double def = 0, Double max = 0)
        {
            var ret = d1 == 0 ? def : d0 / d1;
            if (max != 0 && ret > max) ret = max;
            return ret;
        }

        public static DateTime FirstDateOfMonth(DateTime d)
        {
            var dt = new DateTime(d.Year, d.Month, 1, 0, 0, 0, DateTimeKind.Utc);
            return dt;
        }

        public static String DateTime2String(DateTime dt, bool includeTime = false)
        {
            dt = Tools.ToUTC(dt);
            return includeTime ?
                dt.ToString("yyyyMMddhhmmss") :
                dt.ToString("yyyyMMdd");
        }

        public static int DateTime2Int(DateTime dt, bool includeTime = false)
        {
            dt = Tools.ToUTC(dt);
            return (int)Convert.ToInt64(includeTime ?
                dt.ToString("yyyyMMddhhmmss") :
                dt.ToString("yyyyMMdd"));
        }

        public static DateTime Int2DateTime(int DateInt)
        {

            return String2DateTime(String.Format("{0}", DateInt));
        }

        public static DateTime String2DateTime(String DateString)
        {
            DateString = DateString.Trim();
            int Yr = Convert.ToInt32(DateString.Substring(0, 4));
            int Mth = Convert.ToInt32(DateString.Substring(4, 2));
            int Date = Convert.ToInt32(DateString.Substring(6, 2));
            return Tools.ToUTC(new DateTime(Yr, Mth, Date));
        }

        public static DateTime EndDateOfMonth(DateTime d, bool onlyDatePart = true)
        {
            var dt = onlyDatePart == true ?
                FirstDateOfMonth(d).AddMonths(1).AddDays(-1) :
                FirstDateOfMonth(d).AddMonths(1).AddMilliseconds(-1);
            return dt;
        }

        public static DateTime GetNearestDay(DateTime dt, DayOfWeek dow)
        {
            DateTime ret = dt;
            ret = Tools.ToDateTime(String.Format("{0:dd-MMM-yyyy}", ret), true);
            if (ret.DayOfWeek != dow)
            {
                int intDow = (int)ret.DayOfWeek;
                int intExp = (int)dow;
                ret = ret.AddDays(-(intDow - intExp));
            }
            return ret;
        }

        public static DateTime TimeStampStr2Time(String t)
        {
            DateTime ret = DateTime.Now;
            if (t.Equals("") == false)
            {
                int yr, mo, day, hour, min, sec;
                yr = t.Length >= 4 ? Convert.ToInt32(t.Substring(0, 4)) : 1900;
                mo = t.Length >= 6 ? Convert.ToInt32(t.Substring(4, 2)) : 1;
                day = t.Length >= 8 ? Convert.ToInt32(t.Substring(6, 2)) : 1;
                hour = t.Length >= 10 ? Convert.ToInt32(t.Substring(8, 2)) : 0;
                min = t.Length >= 12 ? Convert.ToInt32(t.Substring(10, 2)) : 0;
                sec = t.Length >= 14 ? Convert.ToInt32(t.Substring(12, 2)) : 0;
                ret = new DateTime(yr, mo, day, hour, min, sec, DateTimeKind.Utc);
            }
            return ret;
        }

        public static Decimal ToDecimal(string s, decimal def = 0)
        {
            decimal ret = def;
            try
            {
                ret = Convert.ToDecimal(s);
            }
            catch (Exception e)
            {
                ret = def;
            }
            return ret;
        }

        public static Int32 ToInt32(string s, Int32 def = 0)
        {
            Int32 ret = def;
            try
            {
                ret = Convert.ToInt32(s);
            }
            catch (Exception e)
            {
                ret = def;
            }
            return ret;
        }

        public static int ToInt(string s, int def = 0)
        {
            int ret = def;
            try
            {
                ret = (int)Convert.ToInt64(s);
            }
            catch (Exception e)
            {
                ret = def;
            }
            return ret;
        }

        public static Int16 ToInt16(string s, Int16 def = 0)
        {
            Int16 ret = def;
            try
            {
                ret = Convert.ToInt16(s);
            }
            catch (Exception e)
            {
                ret = def;
            }
            return ret;
        }

        public static Int64 ToInt64(string s, Int64 def = 0)
        {
            Int64 ret = def;
            try
            {
                ret = Convert.ToInt64(s);
            }
            catch (Exception e)
            {
                ret = def;
            }
            return ret;
        }

        public static Double ToDouble(string s, Double def = 0)
        {
            Double ret = def;
            try
            {
                ret = Convert.ToDouble(s);
            }
            catch (Exception e)
            {
                ret = def;
            }
            return ret;
        }

        public static DateTime DefaultDate
        {
            get
            {
                return new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            }
        }
        public static DateTime ToDateTime(string s, bool dateOnly = false)
        {
            DateTime ret = Tools.DefaultDate;
            try
            {
                ret = Convert.ToDateTime(s);
                if (dateOnly) ret = ret.Date;
            }
            catch (Exception e)
            {
                ret = Tools.DefaultDate;
            }
            return new DateTime(ret.Ticks, DateTimeKind.Utc);
        }

        public static bool ToBool(string s, bool def = false)
        {
            bool ret = def;
            try
            {
                ret = Convert.ToBoolean(s);
            }
            catch (Exception e)
            {
                ret = def;
            }
            return ret;
        }

        //public static ResultInfo RunCode(string code, string[] references = null, string[] parms = null)
        //{
        //    return RunCode(new string[] { code }, references, parms);
        //}

        private static string _codeBinPath;
        public static string BinaryPath
        {
            get { return _codeBinPath; }
            set { _codeBinPath = value; }
        }

        //public static Assembly CompileCode(string[] codes,
        //    string[] references = null,
        //    string binaryPath = null)
        //{

        //    CompilerParameters CompilerParams = new CompilerParameters();
        //    string outputDirectory = Directory.GetCurrentDirectory();

        //    CompilerParams.GenerateInMemory = true;
        //    CompilerParams.TreatWarningsAsErrors = false;
        //    CompilerParams.GenerateExecutable = false;
        //    CompilerParams.CompilerOptions = "/optimize";

        //    if (SystemLibraries.Length > 0) CompilerParams.ReferencedAssemblies.AddRange(SystemLibraries);
        //    foreach (var custom in CustomLibraries)
        //    {
        //        var lib = Path.Combine(BinaryPath, custom);
        //        CompilerParams.ReferencedAssemblies.Add(lib);
        //    }

        //    if (references != null)
        //    {
        //        foreach (var custom in references)
        //        {
        //            var lib = Path.Combine(BinaryPath, custom);
        //            CompilerParams.ReferencedAssemblies.Add(lib);
        //        }
        //    }

        //    CSharpCodeProvider provider = new CSharpCodeProvider();
        //    CompilerResults compile = provider.CompileAssemblyFromSource(CompilerParams, codes);

        //    if (compile.Errors.HasErrors)
        //    {
        //        string text = "Compile error: ";
        //        foreach (CompilerError ce in compile.Errors)
        //        {
        //            text += "rn" + ce.ToString();
        //        }
        //        throw new Exception(text);
        //    }
        //    return compile.CompiledAssembly;
        //}

        private static string[] _systemLibraries;
        public static string[] SystemLibraries
        {
            get
            {
                if (_systemLibraries == null) _systemLibraries = new string[] { "System.dll" };
                return _systemLibraries;
            }
            set { _systemLibraries = value; }
        }

        private static string[] _customLibraries;
        public static string[] CustomLibraries
        {
            get
            {
                if (_customLibraries == null) _customLibraries = new string[] { };
                return _customLibraries;
            }
            set { _customLibraries = value; }
        }


        public static object RunAssembly(Assembly compile,
            string type = "ECIS.CodeExtention", string method = "Main", object[] parms = null)
        {
            object ret = null;
            Module module = compile.GetModules()[0];
            Type mt = null;
            MethodInfo methInfo = null;

            if (module != null)
            {
                mt = module.GetType(type);
            }

            if (mt != null)
            {
                methInfo = mt.GetMethod(method);
            }

            if (methInfo != null)
            {
                ret = methInfo.Invoke(null, parms);
            }
            return ret;
        }

        //public static ResultInfo RunCode(string[] codes,
        //    string[] references = null,
        //    object[] parms = null,
        //    string binaryPath = null)
        //{
        //    ResultInfo ri = new ResultInfo();
        //    try
        //    {
        //        Assembly compiledAssembly = CompileCode(codes, references, binaryPath);
        //        ri.Data = RunAssembly(compiledAssembly, parms: parms);
        //    }
        //    catch (Exception e)
        //    {
        //        ri.PushException(e);
        //    }
        //    ri.CalcLapseTime();
        //    return ri;
        //}
    }
}
