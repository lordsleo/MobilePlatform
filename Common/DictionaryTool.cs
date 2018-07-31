//
//文件名：    Dictionary.aspx.cs
//功能描述：  字典工具
//创建时间：  2015/07/17
//作者：      
//修改时间：  暂无
//修改描述：  暂无
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ServiceInterface.Common
{
    public class DictionaryTool
    {    
        //获取应用名称对应的iport数据库构建注册项目---对应的code_assembly
        public static string GetCodeAssembly(string appName)
        {
            foreach (string _Key in DictionaryData._CODE_ASSEMBLY_DICTIONARY.Keys)
            {
                if (appName == _Key)
                {
                    string codeAssembly = DictionaryData._CODE_ASSEMBLY_DICTIONARY[_Key];
                    return codeAssembly;
                }
            }
            return string.Empty;
        }

        //获取应用名称对应的用户类型
        public static string GetUserType(string appName)
        {
            foreach (string _Key in DictionaryData._USER_TYPE_DICTIONARY.Keys)
            {
                if (appName == _Key)
                {
                    string userType = DictionaryData._USER_TYPE_DICTIONARY[_Key];
                    return userType;
                }
            }
            return string.Empty;
        }

        //获取应用名称对应的内部网公司ID
        public static string GetNbwCompanyId(string appName)
        {
            foreach (string _Key in DictionaryData._NBW_COMPANYID_DICTIONARY.Keys)
            {
                if (appName == _Key)
                {
                    string nbwCompanyId = DictionaryData._NBW_COMPANYID_DICTIONARY[_Key];
                    return nbwCompanyId;
                }
            }
            return string.Empty;
        }

        //获取应用名称与推送数据库关键字对应字典表
        public static string GetKeyDataBase(string appName)
        {
            foreach (string _Key in DictionaryData._PUSH_KEY_DATA_BASE_DICTIONARY.Keys)
            {
                if (appName == _Key)
                {
                    string pushKeyDataBase = DictionaryData._PUSH_KEY_DATA_BASE_DICTIONARY[_Key];
                    return pushKeyDataBase;
                }
            }
            return string.Empty;
        }

        //获取应用名称与推送数据库表名对应字典表
        public static string GetDBTable(string appName)
        {
            foreach (string _Key in DictionaryData._DB_TABLE_DICTIONARY.Keys)
            {
                if (appName == _Key)
                {
                    string nbwCompanyId = DictionaryData._DB_TABLE_DICTIONARY[_Key];
                    return nbwCompanyId;
                }
            }
            return string.Empty;
        }

        //应用名称与Iport关键字对应字典表
        public static string GetIportKeyDataBase(string appName)
        {
            foreach (string _Key in DictionaryData._IPORT_KEY_DATA_BASE_DICTIONARY.Keys)
            {
                if (appName == _Key)
                {
                    string iportKeyDataBase = DictionaryData._IPORT_KEY_DATA_BASE_DICTIONARY[_Key];
                    return iportKeyDataBase;
                }
            }
            return string.Empty;
        }

        //应用名称与移动设备关键字对应字典表
        public static string GetDeviceKeyDataBase(string appName)
        {
            foreach (string _Key in DictionaryData._DEVICE_KEY_DATA_BASE_DICTIONARY.Keys)
            {
                if (appName == _Key)
                {
                    string deviceKeyDataBase = DictionaryData._DEVICE_KEY_DATA_BASE_DICTIONARY[_Key];
                    return deviceKeyDataBase;
                }
            }
            return string.Empty;
        }
    
    }
}

