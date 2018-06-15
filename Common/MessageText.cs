using System;
using SitePlugin;
namespace Common
{
    public static class MessagePartFactory
    {
        public static IMessageText CreateMessageText(string text)
        {
            return MessageText.New(text);
        }
    }
    public class MessageText : IMessageText
    {
        public string Text { get; }
        public static MessageText New(string text)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            return new MessageText(text);
#pragma warning restore CS0618 // Type or member is obsolete
        }
        [Obsolete("Use MessagePartFactory.CreateMessageText() instead")]
        public MessageText(string text)//TODO:移行が済み次第classをinternalにし、obsoleteを解除する（2018/06/15）
        {
            Text = text;
        }
        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            if (obj is MessageText text)
            {
                return this.Text.Equals(text.Text);
            }
            return false;
        }
        public override string ToString() => Text;
        public override int GetHashCode()
        {
            return Text.GetHashCode();
        }
    }
}
