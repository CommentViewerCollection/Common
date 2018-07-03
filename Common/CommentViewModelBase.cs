using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel;
using System.Windows.Media;
using System.Windows;
using SitePlugin;
using System.Text.RegularExpressions;
using System.Windows.Input;
using GalaSoft.MvvmLight.CommandWpf;
using System.Diagnostics;

namespace Common
{
    public abstract class CommentViewModelBase : ICommentViewModel
    {
        public ICommand CommentCopyCommand { get; }
        public ICommand OpenUrlCommand { get; }
        public ICommand UsernameCopyCommand { get; }
        public ICommand NicknameCopyCommand { get; }
        public virtual IEnumerable<IMessagePart> NameItems
        {
            get
            {
                if (!string.IsNullOrEmpty(User.Nickname))
                {
                    return new List<IMessagePart> { MessagePartFactory.CreateMessageText(User.Nickname) };
                }
                else
                {
                    return _nameItems;
                }
            }
            protected set
            {
                _nameItems = value;
            }
        }

        private IEnumerable<IMessagePart> _nameItems;
        public virtual IEnumerable<IMessagePart> MessageItems { get; protected set; }
        public virtual string Info { get; protected set; }

        public virtual string Id { get; protected set; }

        public abstract string UserId { get; }
        public IUser User { get; }

        public bool IsInfo { get; protected set; }

        public bool IsFirstComment { get; protected set; }

        public string PostTime { get; protected set; } = "-";

        public virtual IMessageImage Thumbnail { get; protected set; }

        public FontFamily FontFamily
        {
            get
            {
                if (IsFirstComment)
                    return _options.FirstCommentFontFamily;
                else
                    return _options.FontFamily;
            }
        }

        public FontStyle FontStyle
        {
            get
            {
                if (IsFirstComment)
                    return _options.FirstCommentFontStyle;
                else
                    return _options.FontStyle;
            }
        }

        public FontWeight FontWeight
        {
            get
            {
                if (IsFirstComment)
                    return _options.FirstCommentFontWeight;
                else
                    return _options.FontWeight;
            }
        }

        public int FontSize
        {
            get
            {
                if (IsFirstComment)
                    return _options.FirstCommentFontSize;
                else
                    return _options.FontSize;
            }
        }
        public TextWrapping UserNameWrapping
        {
            get
            {
                if (_options.IsUserNameWrapping)
                    return TextWrapping.Wrap;
                else
                    return TextWrapping.NoWrap;
            }
        }

        public virtual SolidColorBrush Foreground
        {
            get
            {
                if (IsInfo)
                {
                    return new SolidColorBrush(_options.InfoForeColor);
                }
                else
                {
                    return new SolidColorBrush(_options.ForeColor);
                }
            }
        }

        public virtual SolidColorBrush Background
        {
            get
            {
                if (IsInfo)
                {
                    return new SolidColorBrush(_options.InfoBackColor);
                }
                else
                {
                    return new SolidColorBrush(_options.BackColor);
                }
            }
        }

        public virtual bool IsVisible { get; protected set; } = true;

        public ICommentProvider CommentProvider { get; protected set; }
        /// <summary>
        /// 文字列から@ニックネームを抽出する
        /// 文字列中に@が複数ある場合は一番最後のものを採用する
        /// 数字だけのニックネームは不可
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        protected string ExtractNickname(string text)
        {
            var matches = Regex.Matches(text, "(?:@|＠)(\\S+)", RegexOptions.Singleline);
            if (matches.Count > 0)
            {
                foreach(Match match in matches.Cast<Match>().Reverse())
                {
                    var val = match.Groups[1].Value;
                    if (!int.TryParse(val, out _))
                    {
                        return val;
                    }
                }
            }
            return null;
        }
        private readonly ICommentOptions _options;
        public CommentViewModelBase(ICommentOptions options, IUser user, ICommentProvider commentProvider, bool isFirstComment)
        {
            _options = options;
            _options.PropertyChanged += Options_PropertyChanged;
            User = user;
            user.PropertyChanged += (s, e) =>
            {
                switch (e.PropertyName)
                {
                    case nameof(user.IsNgUser):
                        SetVisibility(user);
                        break;
                    case nameof(user.BackColorArgb):
                        RaisePropertyChanged(nameof(Background));
                        break;
                    case nameof(user.ForeColorArgb):
                        RaisePropertyChanged(nameof(Foreground));
                        break;
                    case nameof(user.Nickname):
                        RaisePropertyChanged(nameof(NameItems));
                        RaisePropertyChanged(nameof(HasNickname));
                        break;
                }
            };
            SetVisibility(user);
            IsFirstComment = isFirstComment;
            CommentProvider = commentProvider;

            OpenUrlCommand = new RelayCommand(OpenUrl);
            CommentCopyCommand = new RelayCommand(CopyComment);
            UsernameCopyCommand = new RelayCommand(CopyUsername);
            NicknameCopyCommand = new RelayCommand(CopyNickname);
        }
        private void SetVisibility(IUser user)
        {
            IsVisible = !user.IsNgUser;
        }
        private void Options_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(_options.ForeColor):
                    RaisePropertyChanged(nameof(Foreground));
                    break;
                case nameof(_options.BackColor):
                    RaisePropertyChanged(nameof(Background));
                    break;
                case nameof(_options.FontFamily):
                    RaisePropertyChanged(nameof(FontFamily));
                    break;
                case nameof(_options.FontStyle):
                    RaisePropertyChanged(nameof(FontStyle));
                    break;
                case nameof(_options.FontWeight):
                    RaisePropertyChanged(nameof(FontWeight));
                    break;
                case nameof(_options.FontSize):
                    RaisePropertyChanged(nameof(FontSize));
                    break;
                case nameof(_options.IsUserNameWrapping):
                    RaisePropertyChanged(nameof(UserNameWrapping));
                    break;
            }
        }
        /// <summary>
        /// 文字列からURLを抽出する
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static List<string> ExtractUrl(string str)
        {
            const string urlPattern = @"(?:h?ttps?://|www\.)\S+";
            var matches = Regex.Matches(str, urlPattern, RegexOptions.Compiled | RegexOptions.Singleline);
            var list = new List<string>();
            foreach (Match match in matches)
            {
                list.Add(match.Groups[0].Value);
            }
            return list;
        }
        private void OpenUrl()
        {
            var text = MessageItems.ToText();
            var list = ExtractUrl(text);
            if (list.Count > 0)
            {
                Process.Start(list[0]);
            }
        }
        public bool ContainsUrl
        {
            get
            {
                var text = MessageItems.ToText();
                var list = ExtractUrl(text);
                return list.Count > 0;
            }
        }
        private void CopyComment()
        {
            var items = MessageItems;

            var strs = items.Where(a => a is IMessageText).Cast<IMessageText>().Select(b => b.Text);
            var str = string.Join("", strs);
            SetClipboard(str);
        }
        public bool HasNickname => !string.IsNullOrEmpty(User.Nickname);
        private void CopyNickname()
        {
            var nick = User.Nickname;
            if (string.IsNullOrEmpty(nick))
                return;

            SetClipboard(nick);
        }
        private static void SetClipboard(string str)
        {
            try
            {
                Clipboard.SetText(str);
            }
            catch (System.Runtime.InteropServices.COMException ex)
            {
                Debug.WriteLine(ex.Message);
                //SetInfo("クリップボードのオープンに失敗しました。", InfoType.Error);
            }
        }

        private void CopyUsername()
        {
            var items = User.Name;

            var strs = items.Where(a => a is IMessageText).Cast<IMessageText>().Select(b => b.Text);
            var str = string.Join("", strs);
            SetClipboard(str);
        }
        #region INotifyPropertyChanged
        [NonSerialized]
        private System.ComponentModel.PropertyChangedEventHandler _propertyChanged;
        /// <summary>
        /// 
        /// </summary>
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged
        {
            add { _propertyChanged += value; }
            remove { _propertyChanged -= value; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        protected void RaisePropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            _propertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }

        public Task AfterCommentAdded()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
