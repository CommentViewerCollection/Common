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
    internal class MessageText : IMessageText
    {
        public string Text { get; }
        public static MessageText New(string text)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            return new MessageText(text);
#pragma warning restore CS0618 // Type or member is obsolete
        }
        public MessageText(string text)
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
