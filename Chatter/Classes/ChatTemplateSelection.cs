
using Android.OS;
using Chatter.Model;
using Chatter.View.Cells;
using Xamarin.Forms;

namespace Chatter.Classes
{
    class ChatTemplateSelector : DataTemplateSelector
    {
        DataTemplate incomingDataTemplate;
        DataTemplate outgoingDataTemplate;
        DataTemplate incomingDataTemplateImage;
        DataTemplate outgoingDataTemplateImage;

        DataTemplate outgoingDataTemplateReply;
        DataTemplate outgoingDataTemplateAudio;
        DataTemplate incomingDataTempleteAudio;
        DataTemplate incomingDataTempleteReply;
        DataTemplate outgoingDataTemplateReplyImage;
        DataTemplate incomingDataTempleteReplyImage;
        public ChatTemplateSelector()
        {
            this.incomingDataTemplate = new DataTemplate(typeof(IncomingViewCell));
            this.outgoingDataTemplate = new DataTemplate(typeof(OutgoingViewCell));
            this.incomingDataTemplateImage = new DataTemplate(typeof(IncomingViewCellImage));
            this.outgoingDataTemplateImage = new DataTemplate(typeof(OutgoingViewCellImage));
            this.outgoingDataTemplateAudio = new DataTemplate(typeof(OutgoingViewCellAudio));
            this.outgoingDataTemplateReply = new DataTemplate(typeof(OutgoingViewCellReply));
            this.outgoingDataTemplateReplyImage = new DataTemplate(typeof(OutgoingViewCellReplyImage));
            this.incomingDataTempleteAudio = new DataTemplate(typeof(IncomingViewCellAudio));
            this.incomingDataTempleteReply = new DataTemplate(typeof(IncomingViewCellReply));
            this.incomingDataTempleteReplyImage = new DataTemplate(typeof(IncomingViewCellReplyImage));
        }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            var messageVm = item as ChatModel;
            if (messageVm == null)
                return null;
            if (!string.IsNullOrEmpty(messageVm.reply_to_id) && 
                messageVm.reply_to_message.Contains("7b8e4") && 
                messageVm.reply_to_message.Contains("UserImages")) 
            {
                return (messageVm.sender_id == Application.Current.Properties["Id"].ToString().Replace("\"", "")) ? outgoingDataTemplateReplyImage : incomingDataTempleteReplyImage;
            }
            else if(!string.IsNullOrEmpty(messageVm.reply_to_id))
            {
                return (messageVm.sender_id == Application.Current.Properties["Id"].ToString().Replace("\"", "")) ? outgoingDataTemplateReply : incomingDataTempleteReply;
            }

            if (messageVm.message.Contains("chatter-7b8e4") && messageVm.message.Contains("UserImages"))
                return (messageVm.sender_id == Application.Current.Properties["Id"].ToString().Replace("\"", "")) ? outgoingDataTemplateImage : incomingDataTemplateImage;
            if (messageVm.message.Contains("chatter-7b8e4") && messageVm.message.Contains("AudioCollection"))
                return (messageVm.sender_id == Application.Current.Properties["Id"].ToString().Replace("\"", "")) ? outgoingDataTemplateAudio : incomingDataTempleteAudio;


            return (messageVm.sender_id == Application.Current.Properties["Id"].ToString().Replace("\"","")) ? outgoingDataTemplate : incomingDataTemplate;
        }
    }
}