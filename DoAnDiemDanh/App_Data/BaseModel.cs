namespace DoAnDiemDanh.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using Core;
    using System.Configuration;


    public partial class BaseModel
    {

        public FACE_RECOGNITION_V2Entities Entity;
        public string serverName = @"WIN-2CK0VK3I2R4";
        //public string serverName = ConfigurationManager.AppSettings["serverName"];

        public string databaseName = Constant.DatabaseName;
       // string sqlconnectstr = ConfigurationManager.ConnectionStrings["MyDatabase"].ToString();
        public BaseModel()
        {
            Entity = new FACE_RECOGNITION_V2Entities("metadata = res://*/Models.FaceRecognition.csdl|res://*/Models.FaceRecognition.ssdl|res://*/Models.FaceRecognition.msl;provider=System.Data.SqlClient;provider connection string=" + '"' + "data source=WIN-2CK0VK3I2R4;initial catalog=" + databaseName + "; user id =ngoi;password=admin123;MultipleActiveResultSets=True;App=EntityFramework" + '"');
                                                     
            //"metadata=res://*/Models.MyModel.csdl|res://*/Models.MyModel.ssdl|res://*/Models.MyModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;data source="+serverName+";initial catalog="+databaseName+";integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;" +"providerName = 'System.Data.EntityClient'"
            //"metadata = res://*/Models.MyModel.csdl|res://*/Models.MyModel.ssdl|res://*/Models.MyModel.msl;provider=System.Data.SqlClient;provider connection string=&quot;" + "data source=DESKTOP-6M3QGDT;initial catalog="+ databaseName+ "; integrated security=True;MultipleActiveResultSets=True;App=EntityFramework&quot;"
            //   " Data Source = DESKTOP - 6M3QGDT; Initial Catalog = master; integrated security = True; MultipleActiveResultSets = True; App = EntityFramework"

        }
    }
}
