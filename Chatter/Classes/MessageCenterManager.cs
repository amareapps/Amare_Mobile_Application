using Chatter.Model;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Chatter.Classes
{
    public class MessageCenterManager
    {
        /**
         *  0 = Reply Message
         *  1 = Delete Message
         *  2 = Copy to Message
         **/
        public void sendAction(ChatModel message,int category)
        {
                MessagingCenter.Send<MessageCenterManager, ChatModel>(this, category.ToString(), message);
        }
    }
}
