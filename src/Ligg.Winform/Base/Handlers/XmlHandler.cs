using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Ligg.Base.DataModel;
using Ligg.Base.DataModel.Enums;
using Ligg.Base.Extension;
using Ligg.Base.Helpers;

namespace Ligg.Base.Handlers
{
    public class XmlHandler
    {
        private readonly XmlDocument _document = new XmlDocument();
        private string _filePath;//文件路径
        private string _nodePath;//节点路径 such as: Company/Department/Employees/Employee       

        //#cons
        public XmlHandler(string filePath)
        {
            this._filePath = filePath;
            LoadFromFile(filePath);
            _filePath = filePath;
            LoadFromFile(filePath);
        }

        //#prop
        public string FilePath
        {
            get { return _filePath; }
            set
            {
                _filePath = XmlHelper.GetFilePath(value);
            }
        }

        public string NodePath
        {
            get { return _nodePath; }
            set { _nodePath = value; }
        }


        //#Method
        //##load
        private void LoadFromFile(string filePath)
        {
            try
            {
                if (!(filePath.ToLower().EndsWith(".xml") | filePath.ToLower().EndsWith(".exml")))
                {
                    if (System.IO.File.Exists(filePath + ".xml"))
                    {
                        filePath = filePath + ".xml";
                    }
                    else if (System.IO.File.Exists(filePath + ".exml"))
                    {
                        filePath = filePath + ".exml";
                    }
                }
                if (!System.IO.File.Exists(filePath)) throw new ArgumentException("path does not exist; filePath=" + filePath);
                if (FileHelper.GetFileDetailByOption(filePath, FilePathComposition.Suffix).ToLower() == "xml")
                {
                    _document.Load(filePath);
                }
                else
                {
                    //using (var sr = new StreamReader(filePath))
                    //{
                    //    string str = sr.ReadToEnd();
                    //    string deStr = EncryptionHelper.SmDecrypt(str);
                    //    _document.LoadXml(deStr);
                    //}
                }
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".LoadFromFile error: " + ex.Message);
            }
        }




        //#get
        public string GetOutXml()
        {
            return this._document.OuterXml;
        }

        public string GetNodeInnerTextByTagName(string nodeName, int i)
        {
            try
            {
                XmlNodeList nodeList = this._document.GetElementsByTagName(nodeName);
                var count = 0;
                foreach (XmlNode node in nodeList)
                {
                    if (i == count)
                        return node.InnerText;
                    count++;
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".GetNodeInnerTextByTagName Error: " + ex.Message);
            }
        }



        //##Convert
        public T ConvertToObject<T>()
        {
            try
            {
                return XmlHelper.ConvertToObject<T>(GetOutXml());
            }
            catch (Exception ex)
            {
                throw new ArgumentException("\n>> " + GetType().FullName + ".ConvertToObject error: " + ex.Message);
            }


        }

        public DataTable ConvertToDataTable()
        {
            StringReader stream = null;
            XmlTextReader reader = null;
            try
            {
                var ds = new DataSet();
                stream = new StringReader(_document.InnerXml);
                reader = new XmlTextReader(stream);
                ds.ReadXml(reader);
                return ds.Tables[0];
            }
            catch (Exception ex)
            {
                throw new ArgumentException(@"\n>> " + GetType().FullName + ".ConvertToDataSet Error: " + ex.Message);
            }
            finally
            {
                if (reader != null) reader.Close();
            }
        }


    }
}
