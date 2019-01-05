using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Corinor.Hjelpeklasser
{
   

    public class Serializer<T>
    {

        public Serializer()
        {

        }

        /// <summary>
        /// Serialize to file
        /// </summary>
        /// <param name="url"></param>
        /// <param name="obj">Remember: The object(class) to serializise have to be marked [Serializable]</param>
        /// <returns></returns>
        public bool serialize(string url, T obj)
        {
            bool res = false;

            BinaryFormatter bf = new BinaryFormatter();
            Stream str = null;
            try
            {
                str = File.Open(url, FileMode.Create);
            }
            catch (Exception)
            {
                return false;
            }

            BinaryWriter bw = new BinaryWriter(str);

            try
            {
                bf.Serialize(str, obj);
                res = true;
            }
            catch (Exception)
            {
                
            }
            finally
            {
                bw.Close();
                if (str != null) str.Close();
                if (str != null) str.Dispose();
            }
            
            return res;
        }


        public T deSerialize(string url)
        {

            if (!File.Exists(url)) throw new System.IO.IOException("File not found: " + url);

            BinaryFormatter bf = new BinaryFormatter();
            Stream str = File.Open(url, FileMode.Open);

            BinaryReader br = new BinaryReader(str);

            T obj;

            try
            {
                obj = (T)bf.Deserialize(str);
            }
            catch (ArgumentNullException e) { throw e; }
            catch (System.Runtime.Serialization.SerializationException e) { throw e; }
            catch (System.Security.SecurityException e) { throw e; }

            finally
            {
                br.Close();
                str.Close();
                str.Dispose();
            }

            return (T)obj;

        }
    }

}
