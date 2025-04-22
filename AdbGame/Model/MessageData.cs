
using AdbGame.Converter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdbGame.Model
{
    /// <summary>
    /// 用于页面展示的消息数据
    /// </summary>
    public class MessageData 
    {
        public MessageData(string Content, DateTime dateTime, string Channel, MessageType Type = MessageType.Info, string Title = "")
        {
            this.Id = Channel;
            this.Content = Content;
            this.Type = Type;
            this.Time = dateTime;
            this.Title = Title;
        }

        /// <summary>
        /// 时间
        /// </summary>
        private DateTime _time;

        public DateTime Time
        {
            get { return _time; }
            set
            {
                _time = value;
            }
        }


        /// <summary>
        /// 消息类型
        /// </summary>
        private MessageType _type;

        public MessageType Type
        {
            get { return _type; }
            set
            {
                _type = value;
            }
        }
        /// <summary>
        /// Id
        /// </summary>
        private string _id = string.Empty;

        public string Id
        {
            get { return _id; }
            set
            {
                _id = value;
            }
        }

        /// <summary>
        /// 标题
        /// </summary>
        private string _title = string.Empty;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
            }
        }

        /// <summary>
        /// 消息内容
        /// </summary>
        private string _content = string.Empty;

        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
                //RaisePropertyChanged(() => Content);
            }
        }
    }

    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum MessageType
    {
        [Description("消息")]
        Info = 0,
        [Description("调试")]
        Debug = 1,
        [Description("警告")]
        Warn = 2,
        [Description("错误")]
        Error = 3,
        [Description("致命")]
        Fatal = 4
    }
}
