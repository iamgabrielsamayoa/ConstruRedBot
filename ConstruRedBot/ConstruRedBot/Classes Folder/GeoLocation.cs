using ConstruRedBot.Classes_Folder.Database;
using System;
using System.Collections.Generic;
using System.Text;
using Telegram.Bot.Types;

namespace ConstruRedBot.Classes_Folder
{
    class GeoLocation
    {
        public int Location_Id { get; set; }
        public string botId { get; set; }
        public float longitude { get; set; }
        public float latitude { get; set; }
        public DateTime Date { get; set; }

        public void SavesLocation(GeoLocation UserLocation)
        {
            string SqlQuery = @"insert into tb_locationCons ([botid],[longitude],[latitude])
            values (" + UserLocation.botId + "," + UserLocation.longitude + "," + UserLocation.latitude + ")";
            new ClsConection().RawQuery(SqlQuery);
        }
    }
}
