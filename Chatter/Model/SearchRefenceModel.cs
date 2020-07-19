using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using SQLite;

namespace Chatter.Model
{
    public class SearchRefenceModel
    {
        [PrimaryKey]
        public string user_id
        {
            get; set;
        }
        public string maximum_distance
        {
            get;
            set;
        } = "1";
        public string age_start
        {
            get;
            set;
        } = "18";
        public string age_end
        {
            get;
            set;
        } = "55";
        public int distance_metric
        {
            get;
            set;
        } = 0;
    }
}
