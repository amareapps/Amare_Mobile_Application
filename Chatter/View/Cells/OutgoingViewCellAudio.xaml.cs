﻿using Android.OS;
using Chatter.Classes;
using Chatter.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Chatter.View.Cells
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OutgoingViewCellAudio : ViewCell
    {
        public ChatModel VM => ((ChatModel)BindingContext);
        MessageCenterManager notifier = new MessageCenterManager();
        public OutgoingViewCellAudio()
        {
            InitializeComponent();
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            var player = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            string url = VM.message;
            WebClient wc = new WebClient();
            Stream fileStream = wc.OpenRead(url);
            player.Load(fileStream);
            player.Play();
        }

        private void imagePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            notifier.sendAction(VM, imagePicker.SelectedIndex);
        }

        private void MultiGestureView_LongPressed(object sender, EventArgs e)
        {
            imagePicker.Focus();
        }
    }
}