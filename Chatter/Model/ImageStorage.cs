using Android.Media;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace Chatter.Model
{
    public class ImageStorage
    {
        [PrimaryKey]
        public string id
        {
            get;
            set;
        }
        public string image
        {
            get;
            set;
        }
        public string username
        {
            get;
            set;
        }
        public string location
        {
            get;
            set;
        }
        public string about
        {
            get;
            set;
        }
        public string school
        {
            get;
            set;
        }
        public string birthdate{
            get;
            set;
        }
        public string gender
        {
            get;
            set;
        }
        public string distance 
        { 
            get; 
            set; 
        }

    }
}
