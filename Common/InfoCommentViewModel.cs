﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GalaSoft.MvvmLight;
using SitePlugin;
namespace Common
{
    public class InfoCommentViewModel : CommentViewModelBase, IInfoCommentViewModel
    {
        public override string UserId => "-";
        private readonly static IUser _user = new UserTest("-") { Nickname = "-" };
        public InfoType Type { get; }

        public InfoCommentViewModel(ICommentOptions options, string message, InfoType type)
            : base(options, _user, null, false)
        {
            IsInfo = true;
            MessageItems = new List<IMessagePart>
            {
                MessagePartFactory.CreateMessageText(message),
            };
            Type = type;
        }
        [Obsolete]
        public InfoCommentViewModel(ICommentOptions options, string message)
            : base(options, _user, null, false)
        {
            IsInfo = true;
            MessageItems = new List<IMessagePart>
            {
                MessagePartFactory.CreateMessageText(message),
            };
            Type =  InfoType.Debug;
        }
    }
    /// <summary>
    /// 情報コメントのインタフェース
    /// </summary>
    public interface IInfoCommentViewModel : ICommentViewModel
    {
        InfoType Type { get; }
    }
    /// <summary>
    /// 情報の種類。
    /// デバッグ情報や軽微なエラー情報が必要無い場合があるため分類する。
    /// </summary>
    /// <remarks>大小比較ができるように</remarks>
    public enum InfoType
    {
        /// <summary>
        /// 無し
        /// </summary>
        None,
        /// <summary>
        /// 致命的なエラーがあった場合だけ。必要最小限の情報
        /// </summary>
        Error,
        /// <summary>
        /// 
        /// </summary>
        Notice,
        /// <summary>
        /// 例外全て
        /// </summary>
        Debug,
    }
    public static class InfoTypeRelatedOperations
    {
        /// <summary>
        /// 文字列をInfoTypeに変換する。
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        /// <remarks>InfoTypeをEnumではなくclassにしてこのメソッドもそこに含めたほうが良いかも</remarks>
        public static InfoType ToInfoType(string s)
        {
            if (!Enum.TryParse(s, out InfoType type))
            {
                type = InfoType.Notice;
            }
            return type;
        }
    }
}
