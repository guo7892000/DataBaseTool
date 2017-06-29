using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Breezee.WorkHelper.Tool
{
    /// <summary>
    /// WPF的消息提示类
    /// </summary>
    public class WPFMsgHelper
    {
        #region 变量
        static string _strTitel = "温馨提示";
        static string _strTitelRemile = "温馨提醒";
        #endregion

        #region 提示信息
        public static void ShowInfo(string Msg)
        {
            ShowInfo(Msg, _strTitel); 
        }

        public static void ShowInfo(string Msg, string title)
        {
            MessageBox.Show(Msg, title, MessageBoxButton.OK,MessageBoxImage.Information);
        }

        //public static void ShowInfo(IDictionary<string, string> idic, string title, string longMsg)
        //{
        //    MessageBox.Show(idic, title, longMsg);
        //}

        //public static void ShowInfo(string Msg, string title, string longMsg)
        //{
        //    MessageBox.Show(Msg, title, longMsg);
        //}

        //public static MessageBoxResult ShowInfo(string message, string title, MessageBoxButton myButtons, string longMsg)
        //{
        //    return MessageBox.Show(message, title, myButtons, longMsg);
        //}

        //public static MessageBoxResult ShowInfo(string message, string title, MessageBoxButton myButtons, MyIcon myIcon, string longMsg)
        //{
        //    return MessageBox.Show(message, title, myButtons, myIcon, longMsg);
        //}
        #endregion

        #region 提示错误
        public static void ShowErr(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        public static void ShowErr(string message, Exception ex)
        {
            MessageBox.Show(message + ex.Message);
        }

        public static void ShowErr(string Msg)
        {
            ShowErr(Msg, _strTitel);
        }

        public static void ShowErr(string Msg, string title)
        {
            MessageBox.Show(Msg, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
        #endregion

        #region 提示OkCancel选择
        public static MessageBoxResult ShowOkCancel(string Msg)
        {
            return ShowQuestion(Msg, _strTitelRemile, MessageBoxButton.OKCancel);
        }
        #endregion

        #region 提示YesNo选择
        public static MessageBoxResult ShowYesNo(string Msg)
        {
            return ShowQuestion(Msg, _strTitelRemile, MessageBoxButton.YesNo);
        }
        #endregion

        #region 提示自定义选择
        public static MessageBoxResult ShowQuestion(string Msg, MessageBoxButton btn)
        {
            return ShowQuestion(Msg, _strTitelRemile, btn);
        }

        public static MessageBoxResult ShowQuestion(string Msg, string title, MessageBoxButton btn)
        {
            return MessageBox.Show(Msg, title, btn, MessageBoxImage.Question);
        }
        #endregion
    }
}
