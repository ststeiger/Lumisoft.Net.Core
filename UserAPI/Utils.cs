using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace LumiSoft.MailServer.API.UserAPI
{
    /// <summary>
    /// Provides internal helper methods.
    /// </summary>
    internal class Utils
    {
        #region method CompressDataSet

        /// <summary>
        /// Decompresses specified DataSet data what is compressed with GZIP.
        /// </summary>
        /// <param name="source">Stream to decompress.</param>
        /// <returns>Returns decompressed DataSet.</returns>
        public static DataSet DecompressDataSet(Stream source)
        {
            source.Position = 0;
                        
            GZipStream gzip = new GZipStream(source,CompressionMode.Decompress);

            MemoryStream retVal = new MemoryStream();
            byte[] buffer = new byte[8000];
            int readedCount = gzip.Read(buffer,0,buffer.Length);
            while(readedCount > 0){
                // Store current zipped data block
                retVal.Write(buffer,0,readedCount);

                // Read next data block
                readedCount = gzip.Read(buffer,0,buffer.Length);
            }

            retVal.Position = 0;
            DataSet ds = new DataSet();
            ds.ReadXml(retVal);

            return ds;
        }

        #endregion
    }
}
