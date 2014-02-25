using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Runtime.Serialization;
using System.Windows; 

namespace AlmaCloud.Classes.test
{
    public class TwitterItem
    {
        public string UserName { get; set; }
        public string Message { get; set; }
        public string ImageSource { get; set; }
        public string CreatedAt { get; set; }
    }

    class TwitterHelper
    {
        private static Object _thisLock = new Object();
        public static T LoadSetting<T>(string fileName)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!store.FileExists(fileName))
                    return default(T);
                lock (_thisLock)
                {
                    try
                    {
                        using (var stream = store.OpenFile(fileName, FileMode.Open, FileAccess.Read))
                        {
                            var serializer = new DataContractSerializer(typeof(T));
                            return (T)serializer.ReadObject(stream);
                        }
                    }
                    catch (SerializationException se)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(
                            () => MessageBox.Show(String.Format("Serialize file error {0}:{1}", se.Message, fileName)));
                        return default(T);
                    }
                    catch (Exception e)
                    {
                        Deployment.Current.Dispatcher.BeginInvoke(
                            () => MessageBox.Show(String.Format("Load file error {0}:{1}", e.Message, fileName)));
                        return default(T);
                    }
                }
            }
        }
        public static void SaveSetting<T>(string fileName, T dataToSave)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                lock (_thisLock)
                {
                    try
                    {
                        using (var stream = store.CreateFile(fileName))
                        {
                            var serializer = new DataContractSerializer(typeof(T));
                            serializer.WriteObject(stream, dataToSave);
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(String.Format("Save file error {0}:{1}", e.Message, fileName));
                        return;
                    }
                }
            }
        }
        public static void DeleteFile(string fileName)
        {
            using (var store = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (store.FileExists(fileName))
                    store.DeleteFile(fileName);
            }
        }
        public static DateTime ParseDateTime(string date)
        {
            var dayOfWeek = date.Substring(0, 3).Trim();
            var month = date.Substring(4, 3).Trim();
            var dayInMonth = date.Substring(8, 2).Trim();
            var time = date.Substring(11, 9).Trim();
            var offset = date.Substring(20, 5).Trim();
            var year = date.Substring(25, 5).Trim();
            var dateTime = string.Format("{0}-{1}-{2} {3}", dayInMonth, month, year, time);
            var ret = DateTime.Parse(dateTime).ToLocalTime();
            return ret;
        }
        public static void ShowMessage(string message)
        {
            Deployment.Current.Dispatcher.BeginInvoke(() => MessageBox.Show(message));
        }
    }
}
