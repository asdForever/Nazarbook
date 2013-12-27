using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Resources;

namespace AlmaCloud.Classes
{
    public class TextWorker
    {
        public void addToLocalStorage(string fileName)
        {
            //take file from json folder
            Uri uri = new Uri("files/" + fileName, UriKind.Relative);

            // Obtain the virtual store for the application.
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                StreamResourceInfo sri = App.GetResourceStream(uri);
                StreamReader sr = new StreamReader(sri.Stream);
                string text = sr.ReadToEnd();
                sr.Close();

                IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();

                // Specify the file path and options.
                isf.CreateDirectory("AppData");
                fileName = "AppData\\" + fileName;
                using (var isoFileStream = new IsolatedStorageFileStream(fileName, FileMode.OpenOrCreate, isf))
                {
                    //Write the data
                    using (var isoFileWriter = new StreamWriter(isoFileStream))
                    {
                        isoFileWriter.Write(text.ToString());
                    }
                }
            }
        }
        public void getDataFromLocalStorage(string fileName, int langID)
        {
            // take data from fileName
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
                try
                {
                    // Specify the file path and options.
                    using (var isoFileStream = new IsolatedStorageFileStream("AppData\\" + fileName, FileMode.Open, isf))
                    {
                        // Read the data.
                        using (var isoFileReader = new StreamReader(isoFileStream))
                        {
                            string temp = isoFileReader.ReadToEnd();
                            temp = temp.Replace("\r\n\r\n", "");
                            switch (langID)
                            {
                                case 0:
                                    GlobalVariables.tempForKazText = temp;
                                    break;
                                case 1:
                                    GlobalVariables.tempForRusText = temp;
                                    break;
                                case 2:
                                    GlobalVariables.tempForEngText = temp;
                                    break;
                            }
                        }
                    }
                }

                catch
                {
                    // Handle the case when the user attempts to click the Read button first.
                    MessageBox.Show("Need to create directory and the file \"" + fileName + "\" first.");
                }
            }
        }
        public void getAphorismList(int chapterID)
        {
            string[] kazTextArr = GlobalVariables.tempForKazText.Split(new string[] { "<>" }, System.StringSplitOptions.None);
            string[] rusTextArr = GlobalVariables.tempForRusText.Split(new string[] { "<>" }, System.StringSplitOptions.None);
            string[] engTextArr = GlobalVariables.tempForEngText.Split(new string[] { "<>" }, System.StringSplitOptions.None);

            for (int i = 0; i < kazTextArr.Length; i++)
            {
                if (isCorrectString(kazTextArr[i]) && isCorrectString(rusTextArr[i]) && isCorrectString(engTextArr[i]))
                {
                    Aphorism aphorism = new Aphorism();
                    aphorism.aphID = i;
                    aphorism.chapterID = chapterID;
                    aphorism.textList.Add(kazTextArr[i]);
                    aphorism.textList.Add(rusTextArr[i]);
                    aphorism.textList.Add(engTextArr[i]);
                    aphorism.isLiked = false;
                    aphorism.isSharedT = false;
                    aphorism.isSharedF = false;
                    Book.aphorismList.Add(aphorism);
                }
            }
        }

        public bool isCorrectString(string text)
        {
            try
            {
                if (!String.IsNullOrEmpty(text) && text != "\r\n")
                {
                    return true;
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                MessageBox.Show("IndexOutOfRangeException during isCorrectString: " + ex.Message, "Error", MessageBoxButton.OK);
            }
            return false;
        }
        public string getPageNumberByAphorismID(int aphorismID)
        {
            string result = "";
            if (aphorismID < 9)
                result = "0" + (aphorismID + 1).ToString();
            else
                result = (aphorismID + 1).ToString();
            return result;
        }
        public double getFontSizeByTextLength(string text)
        {
            if (text.Length < 118)
                return 26;
            if (text.Length < 177)
                return 23;
            if (text.Length < 236)
                return 19;
            else
                return 18;
        }
    }
    public class JsonParse
    {
        public void getDataFromJSON()
        {
            // take data from fileName
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();
                if (isf.FileExists("AppData\\aphorismList.json"))
                {
                    try
                    {
                        // Specify the file path and options.
                        using (var isoFileStream = new IsolatedStorageFileStream("AppData\\aphorismList.json", FileMode.Open, isf))
                        {
                            // Read the data.
                            using (var isoFileReader = new StreamReader(isoFileStream))
                            {
                                string jsonString = isoFileReader.ReadToEnd();
                                Book.aphorismList.Clear();

                                while (jsonString.Contains("]]"))
                                    jsonString = jsonString.Substring(0, jsonString.Length - 1);

                                Book.aphorismList.AddRange(JsonConvert.DeserializeObject<List<Aphorism>>(jsonString));
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Exception during getDataFromJSON: " + ex.Message, "Error", MessageBoxButton.OK);
                    }
                }
                else
                {
                    TextWorker tw = new TextWorker();
                    //kz
                    tw.addToLocalStorage("kaz1.txt");
                    tw.addToLocalStorage("kaz2.txt");
                    tw.addToLocalStorage("kaz3.txt");
                    tw.addToLocalStorage("kaz4.txt");
                    //rus
                    tw.addToLocalStorage("rus1.txt");
                    tw.addToLocalStorage("rus2.txt");
                    tw.addToLocalStorage("rus3.txt");
                    tw.addToLocalStorage("rus4.txt");
                    //eng
                    tw.addToLocalStorage("eng1.txt");
                    tw.addToLocalStorage("eng2.txt");
                    tw.addToLocalStorage("eng3.txt");
                    tw.addToLocalStorage("eng4.txt");


                    tw.getDataFromLocalStorage("kaz1.txt", 0);
                    tw.getDataFromLocalStorage("rus1.txt", 1);
                    tw.getDataFromLocalStorage("eng1.txt", 2);
                    tw.getAphorismList(1);
                    tw.getDataFromLocalStorage("kaz2.txt", 0);
                    tw.getDataFromLocalStorage("rus2.txt", 1);
                    tw.getDataFromLocalStorage("eng2.txt", 2);
                    tw.getAphorismList(2);
                    tw.getDataFromLocalStorage("kaz3.txt", 0);
                    tw.getDataFromLocalStorage("rus3.txt", 1);
                    tw.getDataFromLocalStorage("eng3.txt", 2);
                    tw.getAphorismList(3);
                    tw.getDataFromLocalStorage("kaz4.txt", 0);
                    tw.getDataFromLocalStorage("rus4.txt", 1);
                    tw.getDataFromLocalStorage("eng4.txt", 2);
                    tw.getAphorismList(4);
                }
            }
        }
        public void saveToLocalStorage()
        {
            // Obtain the virtual store for the application.
            if (!System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication();

                // Specify the file path and options.
                isf.CreateDirectory("AppData");
                using (var isoFileStream = new IsolatedStorageFileStream("AppData\\aphorismList.json", FileMode.OpenOrCreate, isf))
                {
                    //Write the data
                    using (var isoFileWriter = new StreamWriter(isoFileStream))
                    {
                        isoFileWriter.Write(JsonConvert.SerializeObject(Book.aphorismList));
                    }
                }
            }
        }
    }
}
