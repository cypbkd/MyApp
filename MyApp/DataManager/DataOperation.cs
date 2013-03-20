using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace MyApp.DataManager
{
    public class DataOperation
    {
        static Boolean IsSaving;
        /// <summary>
        /// 此方法是用来存储普通数据文件（异步操作）
        /// </summary>
        /// <typeparam name="T">泛型类型，不定对象</typeparam>
        /// <param name="data">数据实体，需要存储在数据文件中，可以使集合类</param>
        /// <param name="fileName">文件名称</param>
        /// <returns></returns>
        public static async Task SaveAsync<T>(T data, string fileName)
        {
            if (IsSaving) return;
            IsSaving = true;
            // 获取图片文件.
            try
            {
                StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync(fileName, CreationCollisionOption.OpenIfExists);
                IRandomAccessStream raStream = await file.OpenAsync(FileAccessMode.ReadWrite);
                using (IOutputStream outStream = raStream.GetOutputStreamAt(0))
                {
                    // 序列化T类型实体.
                    DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                    serializer.WriteObject(outStream.AsStreamForWrite(), data);
                    await outStream.FlushAsync();
                }
            }
            catch
            {

            }
            finally
            {
                IsSaving = false;
            }
        }
        /// <summary>
        /// 将存储在文件中的数据反序列化（异步操作）
        /// </summary>
        /// <typeparam name="T">反序列化的对象</typeparam>
        /// <param name="filename">文件名称</param>
        /// <returns></returns>
        public static async Task<T> RestoreAsync<T>(string filename)
        {
            // 获取输入流文件.
            T sessionState = default(T);
            try
            {
                StorageFile file = null;                
                file = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename,CreationCollisionOption.OpenIfExists);
                IInputStream inStream = await file.OpenSequentialReadAsync();
                // 反序列化数据对象.
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                sessionState = (T)serializer.ReadObject(inStream.AsStreamForRead());
            }
            catch (Exception)
            {
                return sessionState;
            }
            return sessionState;
        }
        /// <summary>
        /// 将JSON字符串序列化成可识别的对象
        /// </summary>
        /// <typeparam name="T">反序列化对象</typeparam>
        /// <param name="jsonString">JSON字符串</param>
        /// <returns></returns>
        public static T JsonDeserialize<T>(string jsonString)
        {
            var ds = new DataContractJsonSerializer(typeof(T));
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)ds.ReadObject(ms);
            ms.Dispose();
            return obj;
        }
        /// <summary>
        /// 将一个对象序列化成一串字符串
        /// </summary>
        /// <param name="item">序列化的原数据类型</param>
        /// <returns>序列化后的字符串</returns>
        public static string JsonSerialize<T>(T item)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(T));
            string result = String.Empty;
            using (MemoryStream ms = new MemoryStream())
            {
                try
                {
                    serializer.WriteObject(ms, item);
                }
                catch
                {
                }
                ms.Position = 0;
                using (StreamReader reader = new StreamReader(ms))
                {
                    result = reader.ReadToEnd();
                }
            }
            return result;
        }
        /// <summary>
        /// 需要序列化XML数据对象
        /// </summary>
        /// <param name="objectToSerialize"><returns></returns>
        public static string XMLSerialize(object objectToSerialize)
        {
            string result = "";
            using (MemoryStream ms = new MemoryStream())
            {
                DataContractSerializer serializer = new DataContractSerializer(objectToSerialize.GetType());
                serializer.WriteObject(ms, objectToSerialize);
                ms.Position = 0;

                using (StreamReader reader = new StreamReader(ms))
                {
                    result = reader.ReadToEnd();
                }
            }
            return result;
        }

        /// <summary>
        /// XML数据反序列化
        /// </summary>
        /// <typeparam name="T">反序列化对象</typeparam>
        /// <param name="xmlstr"><returns></returns>
        public static T XMLDeserialize<T>(string xmlstr)
        {
            byte[] newBuffer = System.Text.Encoding.UTF8.GetBytes(xmlstr);

            if (newBuffer.Length == 0)
            {
                return default(T);
            }
            using (MemoryStream ms = new MemoryStream(newBuffer))
            {
                DataContractSerializer serializer = new DataContractSerializer(typeof(T));
                return (T)serializer.ReadObject(ms);
            }
        }
    }
}
